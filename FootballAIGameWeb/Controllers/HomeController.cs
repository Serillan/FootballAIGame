using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FootballAIGameWeb.Models;
using FootballAIGameWeb.ViewModels.Home;
using FootballAIGameWeb.ViewModels.Manage;
using Microsoft.AspNet.Identity;

namespace FootballAIGameWeb.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext _context;

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
                    .Where(t => t.StartTime.CompareTo(DateTime.Now) > 0).ToList(),

                LastMatches = _context.Matches
                    .Include(m => m.Player1)
                    .Include(m => m.Player2)
                    .ToList()
            };
            return View(viewModel);
        }

        private ActionResult PlayerHome()
        {
            var lastMatches = _context.Matches
                .Include(m => m.Player1)
                .Include(m => m.Player2)
                .OrderBy(m => m.Time)
                .Take(5) // only last 5 matches
                .ToList();

            var userId = User.Identity.GetUserId();

            var user = _context.Users
                .Include(u => u.Player)
                .Single(u => u.Id == userId);

            var player = user.Player;

            var viewModel = new ViewModels.Home.PlayerHomeViewModel()
            {
                ActiveAIs = new List<string>() { "MyBestAI1", "MyStupidAI"},
                LastMatches = lastMatches,
                Challenges = new List<Player>()
                {
                    _context.Players.Single(p => p.Name == "Portain")
                },
                Player = player,
                SelectedAi = player.SelectedAi
            };

            return View("PlayerHome", viewModel);
        }

        [HttpPost]
        public ActionResult StartRandomMatch(PlayerHomeViewModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index");
            // game server TODO
            return View("SimulatingMatch");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
    }
}