using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel;
using System.Web.Http;
using FootballAIGame.DbModel.Models;
using FootballAIGame.Web.Utilities;

namespace FootballAIGame.Web.Controllers.Api
{
    [Authorize]
    public class MatchesController : BaseApiController
    {
        /// <summary>
        /// Starts looking for a random match.
        /// </summary>
        /// <returns>Ok http response if looking for random match has succesfully started;
        /// otherwise returns BadRequest response with a corresponding error message.</returns>
        [HttpPost]
        public IHttpActionResult StartRandomMatch()
        {
            Player player = null;

            try
            {
                using (var gameServer = new GameServerService.GameServerServiceClient())
                {
                    player = CurrentPlayer;

                    if (player.PlayerState != PlayerState.Idle)
                        return BadRequest("Invalid player state.");

                    player.PlayerState = PlayerState.LookingForOpponent;
                    Context.SaveChanges(); // must be done before calling service!

                    string msg;
                    if ((msg = gameServer.WantsToPlay(player.Name, player.SelectedAI)) != "ok")
                    {
                        player.PlayerState = PlayerState.Idle;
                        Context.SaveChanges();
                        return BadRequest(msg);
                    }
                }
            }
            catch (Exception ex) when (ex is CommunicationObjectFaultedException || ex is EndpointNotFoundException)
            {
                player.PlayerState = PlayerState.Idle;
                Context.SaveChanges();
                return BadRequest("Game Server is offline.");
            }

            return Ok();
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
            var player = CurrentPlayer;

            var match = Context.Matches
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
            var match = Context.Matches.SingleOrDefault(m => m.Id == id);
            if (match == null) return NotFound();
            var data = match.MatchData;

            var result = new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(data) };

            result.Content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/octet-stream"); // binary data

            return ResponseMessage(result);
        }

        /// <summary>
        /// Cancels looking for opponent. Does nothing it the player is not currently looking for opponent.
        /// </summary>
        /// <returns>Ok http response.</returns>
        [HttpPut]
        public IHttpActionResult CancelLooking()
        {
            var player = CurrentPlayer;

            try
            {
                using (var gameServer = new GameServerService.GameServerServiceClient())
                {
                    gameServer.CancelLooking(player.Name);
                }
            }
            catch (CommunicationObjectFaultedException) { }

            if (player.PlayerState == PlayerState.LookingForOpponent)
                player.PlayerState = PlayerState.Idle;
            Context.SaveChanges();
            return Ok();
        }

        /// <summary>
        /// Cancels the match in which the current player currently is. Does nothing if the player
        /// is not currently in any match.
        /// </summary>
        /// <returns>Ok http response.</returns>
        [HttpPut]
        public IHttpActionResult CancelMatch()
        {
            var player = CurrentPlayer;

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

            Context.SaveChanges();
            return Ok();
        }

        /// <summary>
        /// Gets the current simulation step of the match in which the player currently is.
        /// If he is not in any match then it returns the maximum step number.
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetMatchStep()
        {
            try
            {
                using (var gameServer = new GameServerService.GameServerServiceClient())
                {
                    return Ok(gameServer.GetCurrentMatchStep(CurrentPlayer.Name));
                }
            }
            catch (CommunicationObjectFaultedException)
            {
                return BadRequest("Game Server is offline.");
            }
        }
    }
}
