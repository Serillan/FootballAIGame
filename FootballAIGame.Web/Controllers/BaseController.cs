﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using FootballAIGame.DbModel.Models;
using Microsoft.AspNet.Identity;

namespace FootballAIGame.Web.Controllers
{
    /// <summary>
    /// Provides the base class from which other controllers
    /// are derived. Extends <see cref="Controller"/>. 
    /// We use it to define methods and properties that are shared between controllers.
    /// </summary>
    /// <seealso cref="System.Web.Mvc.Controller" />
    public abstract class BaseController : Controller
    {
        /// <summary>
        /// The application database context used for accessing the database using Entity Framework.
        /// </summary>
        private ApplicationDbContext _context;

        /// <summary>
        /// Gets the application database context used for accessing the database using Entity Framework.
        /// </summary>
        /// <value>
        /// The <see cref="ApplicationDbContext"/> used for accessing the database using Entity Framework.
        /// </value>
        protected ApplicationDbContext Context => _context ?? (_context = new ApplicationDbContext());

        /// <summary>
        /// Gets the current logged player.
        /// </summary>
        /// <value>
        /// The current logged <see cref="Player"/> if there is one; otherwise returns null.
        /// </value>
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