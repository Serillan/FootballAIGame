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
        private ApplicationDbContext _context;

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

        public HomeController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

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

        private PlayerHomeViewModel GetNewPlayerHomeViewModel()
        {
            var player = CurrentPlayer;
            var lastMatches = _context.Matches
                .Include(m => m.Player1)
                .Include(m => m.Player2)
                .OrderByDescending(m => m.Time)
                .Take(5) // only last 5 matches
                .ToList();

            var activeAIs = player.ActiveAis.Split(';').ToList();

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

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult HowToPlay()
        {
            return View();
        }

        public ActionResult Statistics()
        {
            return View();
        }
    }
}