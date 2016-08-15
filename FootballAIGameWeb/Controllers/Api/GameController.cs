using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using FootballAIGameWeb.Models;
using Microsoft.AspNet.Identity;

namespace FootballAIGameWeb.Controllers.Api
{
    public class GameController : ApiController
    {

        private Player GetCurrentPlayer(ApplicationDbContext context)
        {
            var userId = User.Identity.GetUserId();
            var user = context.Users
                .Include(u => u.Player)
                .Single(u => u.Id == userId);
            var player = user.Player;

            return player;
        }

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
                    using (var gameServer = new GameService.GameServerServiceClient())
                    {
                        gameServer.StartGame(player.UserId, challengeInDb.ChallengingPlayer.UserId);
                    }

                    challengeInDb.ChallengingPlayer.PlayerState = PlayerState.PlayingMatch;
                    challengeInDb.ChallengedPlayer.PlayerState = PlayerState.PlayingMatch;
                    context.Challenges.Remove(challengeInDb);
                    context.SaveChanges();
                    return Ok();
                }

                return BadRequest();
            }
        }

        [HttpPost]
        public IHttpActionResult StartRandomMatch()
        {
            using (var context = new ApplicationDbContext())
            {
                var player = GetCurrentPlayer(context);

                using (var gameServer = new GameService.GameServerServiceClient())
                {
                    gameServer.WantsToPlay(player.UserId, player.SelectedAi);
                }

                    if (player.PlayerState != PlayerState.Idle)
                        return BadRequest("Invalid player state.");
                
                player.PlayerState = PlayerState.LookingForOpponent;
                context.SaveChanges();
                return Ok();
            }
        }

        [HttpGet]
        public IHttpActionResult GetPlayerState()
        {
            using (var context = new ApplicationDbContext())
            {
                var player = GetCurrentPlayer(context);
                return Ok(player.PlayerState);
            }
        }

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
                    .OrderBy(m => m.Time)
                    .FirstOrDefault();

                if (match == null)
                    return NotFound();

                var isWinner = match.Winner == 1 && match.Player1.UserId == player.UserId ||
                               match.Winner == 2 && match.Player2.UserId == player.UserId;
                var result = match.Winner == 0 ? "draw" : isWinner ? "win" : "loose";

                return Ok(new {result, matchId = match.Id});
            }
        }

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
                    .Select(c => c.ChallengingPlayer.Name) // only oppont names
                    .ToList();


                return Ok(challengersNames);
            }
        }

        [HttpPut]
        public IHttpActionResult CancelLooking()
        {
            using (var context = new ApplicationDbContext())
            {
                var player = GetCurrentPlayer(context);

                // TODO check that game server is checking state!
                player.PlayerState = PlayerState.Idle;
                context.SaveChanges();
                return Ok();
            }
        }

        [HttpPut]
        public IHttpActionResult CancelMatch()
        {
            using (var context = new ApplicationDbContext())
            {
                var player = GetCurrentPlayer(context);

                // TODO check that game server looks on state

                player.PlayerState = PlayerState.Idle; 
                context.SaveChanges();

                return Ok();
            }
        }
    }
}