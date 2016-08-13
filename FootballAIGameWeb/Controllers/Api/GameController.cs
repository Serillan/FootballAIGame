using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FootballAIGameWeb.Models;
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
    }
}
