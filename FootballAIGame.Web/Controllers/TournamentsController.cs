using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using FootballAIGame.Web.GameServerService;
using FootballAIGame.Web.Models;
using FootballAIGame.Web.ViewModels.Tournaments;
using Microsoft.AspNet.Identity;

namespace FootballAIGame.Web.Controllers
{
    public class TournamentsController : Controller
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
                .Where(t => t.TournamentState != TournamentState.Unstarted &&
                            t.TournamentState != TournamentState.Running);

            return View(viewModel);
        }

        /// <summary>
        /// Returns the current tournaments view.
        /// </summary>
        public ActionResult Current()
        {
            var viewModel = _context.Tournaments
                .Include(t => t.Players)
                .Where(t => t.TournamentState == TournamentState.Unstarted || 
                            t.TournamentState == TournamentState.Running);

            return User.IsInRole(RolesNames.TournamentAdmin) ? View("CurrentForAdmin", viewModel) : View(viewModel);
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
                .Include(t => t.Matches.Select(m => m.Player1))
                .Include(t => t.Matches.Select(m => m.Player2))
                .SingleOrDefault(t => t.Id == id);

            if (tournament == null)
                return HttpNotFound();

            tournament.Players = 
                tournament.Players.OrderBy(p => p.PlayerPosition)
                .ThenByDescending(p => p.Player.Score).ToList();

            var activeAIs = CurrentPlayer?.ActiveAis?.Split(';').ToList() ?? new List<string>();
            var currentTournamentPlayer = tournament.Players.SingleOrDefault(tp => tp.Player == CurrentPlayer);
            
            var viewModel = new TournamentDetailsViewModel()
            {
                Tournament = tournament,
                ActiveAIs = activeAIs,
                CurrentPlayer = CurrentPlayer,
                CurrentTournamentPlayer = currentTournamentPlayer
            };

            return View("Details", viewModel);
        }

        /// <summary>
        /// Returns the tournament matches partial view.
        /// </summary>
        public ActionResult TournamentMatches()
        {
            return PartialView("_TournanamentMatches");
        }

        /// <summary>
        /// Returns the manage recurring tournaments view.
        /// </summary>
        [Authorize(Roles = RolesNames.TournamentAdmin)]
        public ActionResult ManageRecurring()
        {
            var viewModel = _context.RecurringTournaments;
            return View(viewModel);
        }

        /// <summary>
        /// Returns the create tournament view.
        /// </summary>
        [Authorize(Roles = RolesNames.TournamentAdmin)]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates the specified tournament if it's valid; otherwise returns
        /// the create tournament view with the specified tournament as it's model.
        /// </summary>
        /// <param name="tournament">The tournament.</param>
        [HttpPost]
        [Authorize(Roles = RolesNames.TournamentAdmin)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tournament tournament)
        {
            if (ModelState.IsValid)
            {
                _context.Tournaments.Add(tournament);
                _context.SaveChanges();
                // let server know
                try
                {
                    using (var gameServer = new GameServerServiceClient())
                    {
                        gameServer.PlanTournament(tournament.Id);
                    }
                }
                catch
                {
                    // ignored
                }
                return RedirectToAction("Current");
            }

            // If we got this far, something failed, redisplay form
            return View(tournament);
        }

        /// <summary>
        /// Returns the create recurring tournament view.
        /// </summary>
        [Authorize(Roles = RolesNames.TournamentAdmin)]
        public ActionResult CreateRecurring()
        {
            return View();
        }

        /// <summary>
        /// Creates the specified tournament if it's valid; otherwise returns 
        /// the create recurring tournament view with the specified tournament as it's model.
        /// </summary>
        /// <param name="tournament">The recurring tournament.</param>
        [HttpPost]
        [Authorize(Roles = RolesNames.TournamentAdmin)]
        [ValidateAntiForgeryToken]
        public ActionResult CreateRecurring(RecurringTournament recurringTournament)
        {
            if (!ModelState.IsValid) return View(recurringTournament);

            _context.RecurringTournaments.Add(recurringTournament);

            // plan recurringTournament.NumberOfPresentTournaments tournaments
            var time = recurringTournament.StartTime;
            for (int i = 0; i < recurringTournament.NumberOfPresentTournaments; i++)
            {
                var tournament = new Tournament(recurringTournament, time);
                _context.Tournaments.Add(tournament);
                _context.SaveChanges();

                // let server know
                try
                {
                    using (var gameServer = new GameServerServiceClient())
                    {
                        gameServer.PlanTournament(tournament.Id);
                    }
                }
                catch
                {
                    // ignored
                }

                time += TimeSpan.FromMinutes(recurringTournament.RecurrenceInterval);
            }

            return RedirectToAction("ManageRecurring");
        }

    }
}