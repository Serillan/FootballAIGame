using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using FootballAIGame.DbModel.Models;
using Microsoft.AspNet.Identity;

namespace FootballAIGame.Web.Controllers.Api
{
    public abstract class BaseApiController : ApiController
    {
        /// <summary>
        /// The application database context used for accessing the database using entity framework.
        /// </summary>
        private ApplicationDbContext _context;

        /// <summary>
        /// Gets the application database context used for accessing the database using entity framework.
        /// </summary>
        /// <value>
        /// The <see cref="ApplicationDbContext"/> used for accessing the database using entity framework.
        /// </value>
        protected ApplicationDbContext Context => _context ?? (_context = new ApplicationDbContext());

        protected Player CurrentPlayer
        {
            get
            {
                var userId = User.Identity.GetUserId();
                var user = Context.Users
                        .Include(u => u.Player)
                        .SingleOrDefault(u => u.Id == userId);
                return user?.Player;
            }
        }

        /// <summary>
        /// Releases unmanaged resources and optionally releases managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            _context?.Dispose();
        }
    }
}