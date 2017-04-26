using System;
using System.Data.Entity;
using System.Linq;
using System.ServiceModel;
using System.Web.Http;
using FootballAIGame.DbModel.Models;
using FootballAIGame.Web.Utilities;

namespace FootballAIGame.Web.Controllers.Api
{
    [Authorize]
    public class ChallengesController : BaseApiController
    {
        /// <summary>
        /// Declines the specified challenge. If the specified challenge doesn't
        /// exist, it does nothing.
        /// </summary>
        /// <param name="id">The challenge identifier.</param>
        /// <returns>Ok http response.</returns>
        [HttpDelete]
        public IHttpActionResult Decline(string id)
        {
            var challenge = Context.Challenges
                .Include(c => c.ChallengingPlayer)
                .SingleOrDefault(c => c.ChallengingPlayer.Name == id);

            if (challenge != null)
            {
                challenge.ChallengingPlayer.PlayerState = PlayerState.Idle;
                Context.Challenges.Remove(challenge);
                Context.SaveChanges();
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
            var player = CurrentPlayer;

            var challengeInDb = Context.Challenges
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
                var opponent = Context.Players.SingleOrDefault(p => p.Name == id);
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
                Context.Challenges.Add(challenge);
                player.PlayerState = PlayerState.WaitingForOpponentToAcceptChallenge;
                Context.SaveChanges();
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
                        var msg = gameServer.StartGame(player.Name, player.SelectedAI,
                            challengeInDb.ChallengingPlayer.Name, challengeInDb.ChallengingPlayer.SelectedAI);

                        if (msg != "ok")
                        {
                            Context.SaveChanges();
                            return BadRequest(msg);
                        }
                    }

                    challengeInDb.ChallengingPlayer.PlayerState = PlayerState.PlayingMatch;
                    challengeInDb.ChallengedPlayer.PlayerState = PlayerState.PlayingMatch;
                    Context.Challenges.Remove(challengeInDb);
                    Context.SaveChanges();
                    return Ok();
                }
                catch (Exception ex) when (ex is CommunicationObjectFaultedException || ex is EndpointNotFoundException)
                {
                    return BadRequest("Game Server is offline.");
                }
            }

            return BadRequest();
        }

        /// <summary>
        /// Gets the names of challengers that are challenging current player.
        /// </summary>
        /// <returns>Ok Http response with opponent names in it's body.</returns>
        [HttpGet]
        public IHttpActionResult GetChallengersNames()
        {
            var challengersNames = Context.Challenges
                .Include(c => c.ChallengingPlayer)
                .Include(c => c.ChallengedPlayer)
                .Where(c => c.ChallengedPlayer.UserId == CurrentPlayer.UserId)
                .Select(c => c.ChallengingPlayer.Name) // only names
                .ToList();


            return Ok(challengersNames);
        }

        /// <summary>
        /// Cancels the current player challenge.
        /// </summary>
        /// <returns>Ok http response.</returns>
        [HttpDelete]
        public IHttpActionResult CancelChallenge()
        {
            var player = CurrentPlayer;

            var challenge = Context.Challenges
                .Include(c => c.ChallengingPlayer)
                .SingleOrDefault(c => c.ChallengingPlayer.UserId == player.UserId);

            if (challenge != null)
                Context.Challenges.Remove(challenge);

            player.PlayerState = PlayerState.Idle;
            Context.SaveChanges();
            return Ok();
        }
    }
}
