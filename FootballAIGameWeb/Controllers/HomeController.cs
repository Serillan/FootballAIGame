using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FootballAIGameWeb.Models;
using FootballAIGameWeb.ViewModels.Home;
using FootballAIGameWeb.ViewModels.Manage;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;

namespace FootballAIGameWeb.Controllers
{
    public class HomeController : Controller
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
                    .Single(u => u.Id == userId);
                return user.Player;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        public HomeController()
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
        /// Returns default index view with last matches and next tournaments if the user is
        /// not log on. Otherwise returns player home view.
        /// </summary>
        /// <returns>Default index view with last matches and next tournaments if the user is
        /// not log on; otherwise returns player home view.</returns>
        public ActionResult Index()
        {
            if (User?.Identity != null && User.Identity.IsAuthenticated)
            {
                return PlayerHome();
            }

            var viewModel = new ViewModels.Home.IndexViewModel()
            {
                NextTournaments = _context.Tournaments
                    .Include(t => t.Players)
                    .Where(t => t.StartTime.CompareTo(DateTime.Now) > 0)
                    .OrderBy(t => t.StartTime)
                    .ToList(),

                LastMatches = _context.Matches
                    .Include(m => m.Player1)
                    .Include(m => m.Player2)
                    .OrderByDescending(m => m.Time)
                    .Take(5)
                    .ToList()
            };
            return View(viewModel);
        }

        /// <summary>
        /// Returns view corresponding to the current player state.
        /// </summary>
        /// <returns>The view corresponding to the current player state.</returns>
        private ActionResult PlayerHome()
        {
            var player = CurrentPlayer;

            switch (player.PlayerState)
            {
                case PlayerState.WaitingForOpponentToAcceptChallenge:
                    return View("WaitingForOpponentToAcceptChallenge");
                case PlayerState.LookingForOpponent:
                    return View("LookingForOpponent");
                case PlayerState.PlayingMatch:
                    return View("PlayingMatch");
            }

            return View("PlayerHome", GetNewPlayerHomeViewModel());
        }

        /// <summary>
        /// Returns the about view.
        /// </summary>
        /// <returns>The about view.</returns>
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        /// <summary>
        /// Returns the how to play view.
        /// </summary>
        /// <returns>The how to play view.</returns>
        public ActionResult HowToPlay()
        {
            return View();
        }

        /// <summary>
        /// Returns the protocol view.
        /// </summary>
        /// <returns>The protocol view.</returns>
        public ActionResult Protocol()
        {
            return View();
        }

        /// <summary>
        /// Returns the rules view.
        /// </summary>
        /// <returns>The rules view.</returns>
        public ActionResult Restrictions()
        {
            return View();
        }

        /// <summary>
        /// Returns the statistics view.
        /// </summary>
        /// <returns>The statistics view.</returns>
        public ActionResult Statistics()
        {
            return View();
        }

        /// <summary>
        /// Gets the new player home view model.
        /// </summary>
        /// <returns>The new player home view model</returns>
        private PlayerHomeViewModel GetNewPlayerHomeViewModel()
        {
            var player = CurrentPlayer;
            var lastMatches = _context.Matches
                .Include(m => m.Player1)
                .Include(m => m.Player2)
                .Where(m => m.Player1.UserId == player.UserId || m.Player2.UserId == player.UserId)
                .OrderByDescending(m => m.Time)
                .Take(5) // only last 5 matches
                .ToList();

            var activeAIs = player.ActiveAis?.Split(';').ToList() ?? new List<string>();

            var viewModel = new ViewModels.Home.PlayerHomeViewModel()
            {
                ActiveAIs = activeAIs,
                LastMatches = lastMatches,
                Challenges = _context.Challenges
                    .Where(c => c.ChallengedPlayer.UserId == player.UserId)
                    .Select(c => c.ChallengingPlayer)
                    .ToList(),
                Player = player,
                SelectedAi = player.SelectedAi
            };

            return viewModel;
        }

    }
}