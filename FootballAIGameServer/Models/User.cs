using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FootballAIGameServer.Models
{
    /// <summary>
    /// Asp.NET identity user class.
    /// Also stores reference to the corresponding <see cref="Models.Player"/> class.
    /// </summary>
    /// <seealso cref="Microsoft.AspNet.Identity.EntityFramework.IdentityUser" />
    public class User : IdentityUser
    {
        /// <summary>
        /// Gets or sets the <see cref="Models.Player"/> class corresponding to this user.
        /// </summary>
        /// <value>
        /// The player.
        /// </value>
        public Player Player { get; set; }

    }
}