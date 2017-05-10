using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using FootballAIGame.DbModel.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FootballAIGame.DbModel.Models
{
    /// <summary>
    /// Represents the Asp.NET Identity user.
    /// Also stores reference to the corresponding <see cref="DbModel.Models.Player"/> class.
    /// </summary>
    /// <seealso cref="Microsoft.AspNet.Identity.EntityFramework.IdentityUser" />
    public class User : IdentityUser
    {
        /// <summary>
        /// Gets or sets user name.
        /// </summary>
        /// <returns>The user name.</returns>
        [Required]
        [RegularExpression(@"^[^\s]+$", ErrorMessage = "Space is not allowed in User Name.")]
        public override string UserName { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DbModel.Models.Player"/> class corresponding to the user.
        /// </summary>
        /// <value>
        /// The <see cref="DbModel.Models.Player"/> class corresponding to the user.
        /// </value>
        public Player Player { get; set; }

        /// <summary>
        /// Generates the user identity asynchronously.
        /// </summary>
        /// <param name="manager">The user manager.</param>
        /// <returns>The identity.</returns>
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}