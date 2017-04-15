using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FootballAIGame.Web.Models;
using FootballAIGame.Web.ViewModels.Matches;
using Microsoft.AspNet.Identity;

namespace FootballAIGame.Web.Controllers
{
    public class MatchesController : Controller
    {
        /// <summary>
        /// The application database context used for accessing database using entity framework.
        /// </summary>
        private ApplicationDbContext _context;

        /// <summary>
        /// Gets the current connected player.
        /// </summary>
        /// <value>
        /// The current player.
        /// </value>
        private Player CurrentPlayer
        {
            get
            {
                var userId = User.Identity.GetUserId();
                var user = _context.Users
                    .Include(u => u.Player)
                    .SingleOrDefault(u => u.Id == userId);
                return user?.Player;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MatchesController"/> class.
        /// </summary>
        public MatchesController()
        {
            _context = new ApplicationDbContext();
        }

        /// <summary>
        /// Releases unmanaged resources and optionally releases managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        /// <summary>
        /// Returns the matches index view.
        /// </summary>
        /// <returns>The index matches view.</returns>
        public ActionResult Index()
        {
            var viewModel = _context.Matches
                .Include(m => m.Player1)
                .Include(m => m.Player2)
                .ToList();

            return View(viewModel);
        }

        /// <summary>
        /// Returns the match details view.
        /// </summary>
        /// <param name="id">The match identifier.</param>
        /// <returns>The details view if the match with the specified identifier exists;
        /// otherwise returns HttpNotFound response.</returns>
        public ActionResult Details(int id)
        {
            var match = _context.Matches
                .Include(m => m.Player1)
                .Include(m => m.Player2)
                .SingleOrDefault(m => m.Id == id);

            if (match == null)
                return HttpNotFound();


            var model = new MatchDetailsViewModel() { Match = match };

            var currentPlayer = CurrentPlayer;

            if (currentPlayer != null)
            {
                if (currentPlayer.Name == match.Player1.Name)
                    model.LoggedPlayerActionLatency = match.Player1AverageActionLatency;
                if (currentPlayer.Name == match.Player2.Name)
                    model.LoggedPlayerActionLatency = match.Player2AverageActionLatency;
            }

            return View("Details", model);
        }

        /// <summary>
        /// Returns the match errors view.
        /// </summary>
        /// <param name="id">The match identifier.</param>
        /// <returns>The errors view if the match with the specified identifier exists;
        /// otherwise returns HttpNotFound response.</returns>
        public ActionResult Errors(int id)
        {
            var match = _context.Matches
                .Include(m => m.Player1)
                .Include(m => m.Player2)
                .SingleOrDefault(m => m.Id == id);

            if (match == null)
                return HttpNotFound();

            return View("Errors", match);
        }

        /// <summary>
        /// Returns the match watch view.
        /// </summary>
        /// <param name="id">The match identifier.</param>
        /// <returns>The watch view if the match with the specified identifier exists;
        /// otherwise returns HttpNotFound response.</returns>
        public ActionResult Watch(int id)
        {
            var match = _context.Matches
                .Include(m => m.Player1)
                .Include(m => m.Player2)
                .SingleOrDefault(m => m.Id == id);

            if (match == null)
                return HttpNotFound();

            return View("Watch", match);
        }

    }
}