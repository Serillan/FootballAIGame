using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FootballAIGame.DbModel.Models;
using FootballAIGame.Web.Utilities;
using FootballAIGame.Web.ViewModels.Home;
using FootballAIGame.Web.ViewModels.Manage;
using Microsoft.AspNet.Identity;

namespace FootballAIGame.Web.Controllers
{
    public class HomeController : BaseController
    {
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
                NextTournaments = Context.Tournaments
                    .Include(t => t.Players)
                    .Where(t => t.TournamentState == TournamentState.Unstarted ||
                                t.TournamentState == TournamentState.Running)
                    .OrderBy(t => t.StartTime)
                    .Take(5)
                    .ToList(),

                LastMatches = Context.Matches
                    .Include(m => m.Player1)
                    .Include(m => m.Player2)
                    .OrderByDescending(m => m.Time)
                    .Take(5)
                    .ToList()
            };
            return View(viewModel);
        }

        /// <summary>
        /// Returns the view corresponding to the current player state.
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
                case PlayerState.PlayingTournamentPlaying:
                case PlayerState.PlayingTournamentWaiting:
                    var viewModel = GetPlayingTournamentViewModel();
                    if (viewModel != null)
                        return View("PlayingTournament", viewModel);
                    player.PlayerState = PlayerState.Idle;  // tournament wasn't found -> back to idle
                    Context.SaveChanges();
                    break;
            }

            return View("PlayerHome", GetNewPlayerHomeViewModel());
        }

        /// <summary>
        /// Gets the new player home view model.
        /// </summary>
        /// <returns>The new player home view model</returns>
        private PlayerHomeViewModel GetNewPlayerHomeViewModel()
        {
            var player = CurrentPlayer;

            var lastMatches = Context.Matches
                .Include(m => m.Player1)
                .Include(m => m.Player2)
                .Where(m => m.Player1.UserId == player.UserId || m.Player2.UserId == player.UserId)
                .OrderByDescending(m => m.Time)
                .Take(5) // only last 5 matches
                .ToList();

            var joinedTournaments = Context.Tournaments
                .Include(t => t.Players.Select(tp => tp.Player))
                .AsEnumerable() // to use comparison with player (only allowed in memory!)
                .Where(t => t.Players.Any(tp => tp.Player == player))
                .ToList();
            joinedTournaments.Sort(new JoinedTournamentComparer());

            // unstarted + 5 newest finished
            var i = 0;
            joinedTournaments = joinedTournaments
                .TakeWhile(t => t.TournamentState == TournamentState.Unstarted ||
                                i++ < 5).ToList();

            var activeAIs = player.ActiveAIs?.Split(';').ToList() ?? new List<string>();

            var viewModel = new ViewModels.Home.PlayerHomeViewModel()
            {
                ActiveAIs = activeAIs,
                LastMatches = lastMatches,
                Challenges = Context.Challenges
                    .Where(c => c.ChallengedPlayer.UserId == player.UserId)
                    .Select(c => c.ChallengingPlayer)
                    .ToList(),
                Player = player,
                SelectedAi = player.SelectedAI,
                JoinedTournaments = joinedTournaments
            };

            return viewModel;
        }

        /// <summary>
        /// Gets the playing tournament view model.
        /// </summary>
        private PlayingTournamentViewModel GetPlayingTournamentViewModel()
        {
            var tournament = Context.Tournaments
                .Include(t => t.Matches.Select(m => m.Player1))
                .Include(t => t.Matches.Select(m => m.Player2))
                .Include(t => t.Players.Select(tp => tp.Player))
                .FirstOrDefault(t => t.TournamentState == TournamentState.Running);

            if (tournament?.Players != null)
                tournament.Players = tournament.Players
                    .OrderBy(p => p.PlayerPosition)
                    .ThenByDescending(p => p.Player.Score)
                    .ToList();

            var viewModel = new PlayingTournamentViewModel()
            {
                Tournament = tournament,
                CurrentPlayer = CurrentPlayer
            };
            

            return viewModel;
        }

    }
}