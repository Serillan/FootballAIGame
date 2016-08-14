using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FootballAIGameWeb.Models;
using FootballAIGameWeb.ViewModels.Players;

namespace FootballAIGameWeb.Controllers
{
    public class PlayersController : Controller
    {
        private ApplicationDbContext _context;

        public PlayersController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        // GET: players
        public ActionResult Index()
        {
            var viewModel = _context.Players.ToList();

            return View(viewModel);
        }

        // GET: players/details/id
        public ActionResult Details(string id)
        {
            var player = _context.Players.SingleOrDefault(p => p.UserId == id);

            if (player == null)
                return HttpNotFound();

            var lastMatches = _context.Matches
                .Include(m => m.Player1)
                .Include(m => m.Player2)
                .Where(m => m.Player1.UserId == player.UserId || m.Player2.UserId == player.UserId)
                .OrderByDescending(m => m.Time)
                .Take(5).ToList();

            var lastTournaments = new List<Tournament>(); // TODO

            var viewModel = new PlayerDetailsViewModel()
            {
                Player = player,
                LastMatches = lastMatches,
                LastTournaments = lastTournaments,
                ActiveAIs = new List<string>() { "MyBestAI1", "MyStupidAI" } // from server TODO
            };

            return View("Details", viewModel);
        }
    }
}