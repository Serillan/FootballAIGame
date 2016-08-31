using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel;
using System.Threading;
using System.Web.Http;
using System.Web.UI.WebControls;
using FootballAIGameWeb.GameServerService;
using FootballAIGameWeb.Models;
using Microsoft.AspNet.Identity;

namespace FootballAIGameWeb.Controllers.Api
{
    /// <summary>
    /// Web API controller that exposes web services for using the application.
    /// Web browser uses AJAX calls to call these services in accordance to
    /// the user actions.
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    [Authorize]
    public class GameController : ApiController
    {

        /// <summary>
        /// Gets the current connected player.
        /// </summary>
        /// <param name="context">The application database context.</param>
        /// <returns>The current connected player.</returns>
        private Player GetCurrentPlayer(ApplicationDbContext context)
        {
            var userId = User.Identity.GetUserId();
            var user = context.Users
                .Include(u => u.Player)
                .Single(u => u.Id == userId);
            var player = user.Player;

            return player;
        }

        /// <summary>
        /// Declines the specified challenge. If the specified challenge doesn't
        /// exist, it does nothing.
        /// </summary>
        /// <param name="id">The challenge identifier.</param>
        /// <returns>Ok http response.</returns>
        [HttpDelete]
        public IHttpActionResult Decline(string id)
        {
            using (var context = new ApplicationDbContext())
            {
                var challenge = context.Challenges
                    .Include(c => c.ChallengingPlayer)
                    .SingleOrDefault(c => c.ChallengingPlayer.Name == id);

                if (challenge != null)
                {
                    challenge.ChallengingPlayer.PlayerState = PlayerState.Idle;
                    context.Challenges.Remove(challenge);
                    context.SaveChanges();
                }
            }

            return Ok();
        }

        /// <summary>
        /// Cancels the current player challenge.
        /// </summary>
        /// <returns>Ok http response.</returns>
        [HttpDelete]
        public IHttpActionResult CancelChallenge()
        {
            using (var context = new ApplicationDbContext())
            {
                var player = GetCurrentPlayer(context);

                var challenge = context.Challenges
                    .Include(c => c.ChallengingPlayer)
                    .SingleOrDefault(c => c.ChallengingPlayer.UserId == player.UserId);

                context.Challenges.Remove(challenge);

                player.PlayerState = PlayerState.Idle;
                context.SaveChanges();
                return Ok();
            }
        }

        /// <summary>
        /// Selects the specified AI.
        /// </summary>
        /// <param name="id">The AI name.</param>
        /// <returns>Ok http response.</returns>
        [HttpPut]
        public IHttpActionResult SelectAi(string id)
        {
            using (var context = new ApplicationDbContext())
            {
                var player = GetCurrentPlayer(context);

                player.SelectedAi = id;

                context.SaveChanges();
            }

            return Ok();
        }

        /// <summary>
        /// If the specified AI is not currently selected, select it. Otherwise
        /// unselect it.
        /// </summary>
        /// <param name="id">The AI name.</param>
        /// <returns>Ok http response.</returns>
        [HttpPut]
        public IHttpActionResult ToggleAi(string id)
        {
            using (var context = new ApplicationDbContext())
            {
                var player = GetCurrentPlayer(context);
                player.SelectedAi = player.SelectedAi == id ? null : id;

                context.SaveChanges();
            }

            return Ok();
        }

        /// <summary>
        /// Challenges the player.
        /// </summary>
        /// <param name="id">The player name.</param>
        /// <returns>Ok http response if the challenge was succesfully created;
        /// otherwise returns BadRequest response with a corresponding error message.</returns>
        [HttpPost]
        public IHttpActionResult ChallengePlayer(string id)
        {
            using (var context = new ApplicationDbContext())
            {
                var player = GetCurrentPlayer(context);

                var challengeInDb = context.Challenges
                    .Include(c => c.ChallengingPlayer)
                    .Include(c => c.ChallengedPlayer)
                    .SingleOrDefault(c =>
                        (c.ChallengingPlayer.Name == id
                         && c.ChallengedPlayer.UserId == player.UserId)
                        || c.ChallengingPlayer.UserId == player.UserId);

                if (player.PlayerState != PlayerState.Idle)
                    return BadRequest("Player is not idle!");

                // new challenge
                if (challengeInDb == null)
                {
                    var opponent = context.Players.SingleOrDefault(p => p.Name == id);
                    if (opponent == null)
                    {
                        return BadRequest("Opponent with the given name was not found.");
                    }
                    if (opponent.UserId == player.UserId)
                    {
                        return BadRequest("Cannot challenge yourself!");
                    }

                    var challenge = new Challenge
                    {
                        ChallengingPlayer = player,
                        ChallengedPlayer = opponent
                    };
                    context.Challenges.Add(challenge);
                    player.PlayerState = PlayerState.WaitingForOpponentToAcceptChallenge;
                    context.SaveChanges();
                    return Ok();
                }

                // only one concurrent challenge allowed
                if (challengeInDb.ChallengingPlayer == player)
                {
                    return BadRequest("Only one concurrent challenge is allowed.");
                }

                // accept challenge
                if (challengeInDb.ChallengingPlayer.Name == id &&
                    challengeInDb.ChallengedPlayer == player &&
                    challengeInDb.ChallengingPlayer.PlayerState == PlayerState.WaitingForOpponentToAcceptChallenge)
                {
                    try
                    {
                        using (var gameServer = new GameServerService.GameServerServiceClient())
                        {
                            var msg = gameServer.StartGame(player.Name, player.SelectedAi,
                                challengeInDb.ChallengingPlayer.Name, challengeInDb.ChallengingPlayer.SelectedAi);

                            if (msg != "ok")
                            {
                                context.SaveChanges();
                                return BadRequest(msg);
                            }
                        }

                        challengeInDb.ChallengingPlayer.PlayerState = PlayerState.PlayingMatch;
                        challengeInDb.ChallengedPlayer.PlayerState = PlayerState.PlayingMatch;
                        context.Challenges.Remove(challengeInDb);
                        context.SaveChanges();
                        return Ok();
                    }
                    catch (CommunicationObjectFaultedException) // server if offline 
                    {
                        return BadRequest("Game Server is offline.");
                    }
                }

                return BadRequest();
            }
        }

        /// <summary>
        /// Starts looking for a random match.
        /// </summary>
        /// <returns>Ok http response if looking for random match has succesfully started;
        /// otherwise returns BadRequest response with a corresponding error message.</returns>
        [HttpPost]
        public IHttpActionResult StartRandomMatch()
        {
            Player player = null;
            using (var context = new ApplicationDbContext())
            {
                try
                {
                    using (var gameServer = new GameServerService.GameServerServiceClient())
                    {
                        player = GetCurrentPlayer(context);

                        if (player.PlayerState != PlayerState.Idle)
                            return BadRequest("Invalid player state.");

                        player.PlayerState = PlayerState.LookingForOpponent;
                        context.SaveChanges(); // must be done before calling service!

                        string msg;
                        if ((msg = gameServer.WantsToPlay(player.Name, player.SelectedAi)) != "ok")
                        {
                            player.PlayerState = PlayerState.Idle;
                            context.SaveChanges();
                            return BadRequest(msg);
                        }
                    }
                }
                catch (CommunicationObjectFaultedException ex)
                {
                    player.PlayerState = PlayerState.Idle;
                    context.SaveChanges();
                    return BadRequest("Game Server is offline.");
                }
            }

            return Ok();
        }

        /// <summary>
        /// Gets the state of the current player.
        /// </summary>
        /// <returns>Ok http response with the player state in it's body.</returns>
        [HttpGet]
        public IHttpActionResult GetPlayerState()
        {
            using (var context = new ApplicationDbContext())
            {
                var player = GetCurrentPlayer(context);
                return Ok(player.PlayerState);
            }
        }

        /// <summary>
        /// Gets the current player last match result.
        /// </summary>
        /// <returns>The http ok response with the match Id and the result string
        /// in it's body if the current player has played at least one match; 
        /// otherwise returns HttpNotFound response. </returns>
        [HttpGet]
        public IHttpActionResult GetLastMatchResult()
        {
            using (var context = new ApplicationDbContext())
            {
                var player = GetCurrentPlayer(context);

                var match = context.Matches
                    .Include(m => m.Player1)
                    .Include(m => m.Player2)
                    .Where(m => m.Player1.UserId == player.UserId || m.Player2.UserId == player.UserId)
                    .OrderByDescending(m => m.Time)
                    .FirstOrDefault();

                if (match == null)
                    return NotFound();

                var isWinner = match.Winner == 1 && match.Player1.UserId == player.UserId ||
                               match.Winner == 2 && match.Player2.UserId == player.UserId;
                var result = match.Winner == 0 ? "draw" : isWinner ? "win" : "loose";

                return Ok(new { result, matchId = match.Id });
            }
        }

        /// <summary>
        /// Gets the names of challengers that are challenging current player.
        /// </summary>
        /// <returns>Ok Http response with opponent names in it's body.</returns>
        [HttpGet]
        public IHttpActionResult GetChallengersNames()
        {
            using (var context = new ApplicationDbContext())
            {
                var player = GetCurrentPlayer(context);

                var challengersNames = context.Challenges
                    .Include(c => c.ChallengingPlayer)
                    .Include(c => c.ChallengedPlayer)
                    .Where(c => c.ChallengedPlayer.UserId == player.UserId)
                    .Select(c => c.ChallengingPlayer.Name) // only names
                    .ToList();


                return Ok(challengersNames);
            }
        }

        /// <summary>
        /// Gets the active AIs names that are connected to the game server
        /// with the current player name.
        /// </summary>
        /// <returns>Ok Http response with player's active AI names in it's body.</returns>
        [HttpGet]
        public IHttpActionResult GetActiveAis()
        {
            using (var context = new ApplicationDbContext())
            {
                var player = GetCurrentPlayer(context);
                var activeAIs = player.ActiveAis?.Split(';').ToList() ?? new List<string>();
                return Ok(activeAIs);
            }
        }

        /// <summary>
        /// Gets the match data.
        /// Match data is binary representation of game states from all simulation steps. <para />
        /// For each step there is a ball position with a movement vector and 22 players positions and movement vectors. <para />
        /// Everything is encoded as float. So each step takes 23*4 = 92 floats = 368 Bytes
        /// </summary>
        /// <param name="id">The match identifier.</param>
        /// <returns>The http ok response with the match data in binary form in it's body if the match with
        /// the specified identifier exists; otherwise returns HttpNotFound response. </returns>
        [AllowAnonymous]
        [HttpGet]
        public IHttpActionResult GetMatchData(int id)
        {
            using (var context = new ApplicationDbContext())
            {
                var match = context.Matches.SingleOrDefault(m => m.Id == id);
                if (match == null)
                    return NotFound();

                var data = match.MatchData;

                var result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new ByteArrayContent(data);
                result.Content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/octet-stream"); // binary data

                return ResponseMessage(result);
            }
        }

        /// <summary>
        /// Cancels looking for opponent. Does nothing it the player is not currently looking for opponent.
        /// </summary>
        /// <returns>Ok http response.</returns>
        [HttpPut]
        public IHttpActionResult CancelLooking()
        {
            using (var context = new ApplicationDbContext())
            {
                var player = GetCurrentPlayer(context);

                try
                {
                    using (var gameServer = new GameServerService.GameServerServiceClient())
                    {
                        gameServer.CancelLooking(player.Name);
                    }
                } catch (CommunicationObjectFaultedException) { }

                if (player.PlayerState == PlayerState.LookingForOpponent)
                    player.PlayerState = PlayerState.Idle;
                context.SaveChanges();
                return Ok();
            }
        }

        /// <summary>
        /// Cancels the match in which the current player currently is. Does nothing if the player
        /// is not currently in any match.
        /// </summary>
        /// <returns>Ok http response.</returns>
        [HttpPut]
        public IHttpActionResult CancelMatch()
        {
            using (var context = new ApplicationDbContext())
            {
                var player = GetCurrentPlayer(context);

                // TODO check that game server looks on state
                try
                {
                    using (var gameServer = new GameServerService.GameServerServiceClient())
                    {
                        gameServer.CancelMatch(player.Name);
                    }
                }
                catch (CommunicationObjectFaultedException)
                {
                    player.PlayerState = PlayerState.Idle;
                }

                context.SaveChanges();
                return Ok();
            }
        }
    }
}