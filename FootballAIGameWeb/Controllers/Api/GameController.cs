using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FootballAIGameWeb.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;

namespace FootballAIGameWeb.Controllers.Api
{
    public class GameController : ApiController
    {

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
                var userId = User.Identity.GetUserId();
                var user = context.Users
                    .Include(u => u.Player)
                    .Single(u => u.Id == userId);
                var player = user.Player;

                // TODO tell game server that player wants to cancel challenge
                // and wait for server response
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

                var userId = User.Identity.GetUserId();
                var user = context.Users
                    .Include(u => u.Player)
                    .Single(u => u.Id == userId);
                var player = user.Player;

                player.SelectedAi = id;

                context.SaveChanges();
            }

            return Ok();
        }

        [HttpGet]
        public IHttpActionResult GetPlayerState()
        {
            using (var context = new ApplicationDbContext())
            {
                var userId = User.Identity.GetUserId();
                var user = context.Users
                    .Include(u => u.Player)
                    .Single(u => u.Id == userId);
                var player = user.Player;

                return Ok(player.PlayerState);
            }
        }

        [HttpGet]
        public IHttpActionResult GetLastMatchResult()
        {
            using (var context = new ApplicationDbContext())
            {
                var userId = User.Identity.GetUserId();
                var user = context.Users
                    .Include(u => u.Player)
                    .Single(u => u.Id == userId);
                var player = user.Player;

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

                return Ok(new { result = result, matchId = match.Id });
            }
        }

        [HttpGet]
        public IHttpActionResult GetChallengersNames()
        {
            using (var context = new ApplicationDbContext())
            {
                var userId = User.Identity.GetUserId();
                var user = context.Users
                    .Include(u => u.Player)
                    .Single(u => u.Id == userId);
                var player = user.Player;

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
                var userId = User.Identity.GetUserId();
                var user = context.Users
                    .Include(u => u.Player)
                    .Single(u => u.Id == userId);
                var player = user.Player;

                // TODO tell game server that player doesn't want to fight
                // and wait for server response
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
                var userId = User.Identity.GetUserId();
                var user = context.Users
                    .Include(u => u.Player)
                    .Single(u => u.Id == userId);
                var player = user.Player;

                // TODO tell game server that player wants to cancel match
                // server will change the player states! 

                player.PlayerState = PlayerState.Idle; // for now, -> TODO server will change it
                context.SaveChanges(); // TODO will be removed ^^

                return Ok();
            }
        }

    }
}
