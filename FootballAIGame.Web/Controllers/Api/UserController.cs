using System.Linq;
using System.Web;
using System.Web.Http;
using FootballAIGame.DbModel.Models;
using FootballAIGame.DbModel.Utilities;
using FootballAIGame.Web.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;

namespace FootballAIGame.Web.Controllers.Api
{
    [Authorize]
    public class UserController : BaseApiController
    {
        /// <summary>
        /// Gets the player's state of the current connected user.
        /// </summary>
        /// <returns>OK <see cref="IHttpActionResult"/> with the player's state in it's body.</returns>
        [HttpGet]
        public IHttpActionResult GetPlayerState()
        {
            var player = CurrentPlayer;
            return Ok(player.PlayerState);
        }

        /// <summary>
        /// If the specified user doesn't have the specified role then adds the role to the user; otherwise
        /// removes it.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="roleName">Name of the role.</param>
        [Route("api/user/togglerole/{userId}/{roleName}")]
        [HttpPut]
        [Authorize(Roles = RolesNames.MainAdmin)]
        public IHttpActionResult ToggleRole(string userId, string roleName)
        {
            var user = Context.Users.SingleOrDefault(u => u.Id == userId);
            var roleExists = Context.Roles.Any(r => r.Name == roleName);
            if (user == null || !roleExists)
                return NotFound();

            var userStore = new UserStore<User>(Context);
            var userManager = new UserManager<User>(userStore);
            if (!userManager.IsInRole(userId, roleName))
                userManager.AddToRole(userId, roleName);
            else
                userManager.RemoveFromRole(userId, roleName);

            Context.SaveChanges();

            // if it's current user (that has called this service), relog him (for changes to take effect)
            if (user == CurrentPlayer.User)
            {
                var authenticationManager = HttpContext.Current.GetOwinContext().Authentication;

                //Log the user out
                authenticationManager.SignOut();

                //Log the user back in
                var identity = userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = true }, identity);
            }

            return Ok();
        }

        /// <summary>
        /// Generates the new access key for the connected player.
        /// </summary>
        [HttpPost]
        public IHttpActionResult GenerateNewAccessKey()
        {
            var newKey = AccessKeyGenerator.Generate();

            var player = CurrentPlayer;
            player.AccessKey = newKey;
            Context.SaveChanges();

            return Ok(newKey);
        }
    }
}
