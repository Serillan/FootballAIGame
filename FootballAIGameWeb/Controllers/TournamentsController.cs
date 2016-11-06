using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FootballAIGameWeb.Models;

namespace FootballAIGameWeb.Controllers
{
    public class TournamentsController : Controller
    {
        /// <summary>
        /// The application database context used for accessing database using entity framework.
        /// </summary>
        private ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="TournamentsController"/> class.
        /// </summary>
        public TournamentsController()
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
        /// Returns the tournaments index view.
        /// </summary>
        /// <returns>The tournaments index view.</returns>
        public ActionResult Index()
        {
            var viewModel = _context.Tournaments
                .Include(t => t.Players)
                .Where(t => t.TournamentState != TournamentState.Unstarted);

            return View(viewModel);
        }

        public ActionResult Next()
        {
            var viewModel = _context.Tournaments
                .Include(t => t.Players)
                .Where(t => t.TournamentState == TournamentState.Unstarted);

            return View(viewModel);
        }

        /// <summary>
        /// Returns the details of the specified tournament.
        /// </summary>
        /// <param name="id">The tournament identifier.</param>
        /// <returns>The details of the specified tournament.</returns>
        public ActionResult Details(int id)
        {
            var tournament = _context.Tournaments
                .Include(t => t.Players.Select(tp => tp.Player))  // nested include
                .SingleOrDefault(t => t.Id == id);

            if (tournament == null)
                return HttpNotFound();

            tournament.Players = 
                tournament.Players.OrderBy(p => p.PlayerPosition).ToList();

            return View("Details", tournament);
        }
    }
}