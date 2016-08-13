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
                    .Where(t => t.StartTime.CompareTo(DateTime.Now) > 0).ToList(),

                LastMatches = _context.Matches
                    .Include(m => m.Player1)
                    .Include(m => m.Player2)
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
                .OrderBy(m => m.Time)
                .Take(5) // only last 5 matches
                .ToList();

            var viewModel = new ViewModels.Home.PlayerHomeViewModel()
            {
                ActiveAIs = new List<string>() { "MyBestAI1", "MyStupidAI" },
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

        [HttpPost]
        public ActionResult StartRandomMatch(PlayerHomeViewModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index");
            // game server TODO
            CurrentPlayer.PlayerState = PlayerState.LookingForOpponent;
            _context.SaveChanges();
            return View("LookingForOpponent"); // will do ajax calls whether game has started
        }

        [HttpPost]
        public ActionResult ChallengePlayer(PlayerHomeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                RedirectToAction("Index");
            }

            var player = CurrentPlayer;

            var challengeInDb = _context.Challenges
                .Include(c => c.ChallengingPlayer)
                .Include(c => c.ChallengedPlayer)
                .SingleOrDefault(c =>
                    (c.ChallengingPlayer.Name == model.OpponentPlayerName
                    && c.ChallengedPlayer.UserId == player.UserId)
                    || c.ChallengingPlayer.UserId == player.UserId);

            // new challenge
            if (challengeInDb == null)
            {
                var opponent = _context.Players.SingleOrDefault(p => p.Name == model.OpponentPlayerName);
                if (opponent == null || opponent == player)
                {
                    model = GetNewPlayerHomeViewModel();
                    model.ErrorFromTheServer = "Opponent with the given name was not found.";
                    return View("PlayerHome", model);
                }

                var challenge = new Challenge()
                {
                    ChallengingPlayer = player,
                    ChallengedPlayer = opponent
                };
                _context.Challenges.Add(challenge);
                player.PlayerState = PlayerState.WaitingForOpponentToAcceptChallenge;
                _context.SaveChanges();
            }

            // only one concurrent challenge allowed
            else if (challengeInDb.ChallengingPlayer == player)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Only one concurrent challenge is allowed.");
            }

            // accept challenge
            else if (challengeInDb.ChallengingPlayer.Name == model.OpponentPlayerName &&
                     challengeInDb.ChallengedPlayer == player)
            {
                // Game Server TODO Challenge(player1Name, ai1Name , player2Name, ai2Name)
                challengeInDb.ChallengingPlayer.PlayerState = PlayerState.PlayingMatch;
                challengeInDb.ChallengedPlayer.PlayerState = PlayerState.PlayingMatch;
                _context.Challenges.Remove(challengeInDb);
                _context.SaveChanges();
                return View("PlayingMatch");
            }

            return View("WaitingForOpponentToAcceptChallenge"); // will do ajax calls to check
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

    }
}