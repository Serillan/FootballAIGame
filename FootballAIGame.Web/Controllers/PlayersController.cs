using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FootballAIGame.DbModel.Models;
using FootballAIGame.Web.Utilities;
using FootballAIGame.Web.ViewModels.Players;
using Microsoft.AspNet.Identity;

namespace FootballAIGame.Web.Controllers
{
    public class PlayersController : BaseController
    {
        /// <summary>
        /// Returns the players index view.
        /// </summary>
        /// <returns>The index matches view.</returns>
        public ActionResult Index()
        {
            if (User.IsInRole(RolesNames.MainAdmin))
            {
                var viewModel = Context.Players
                    .Include(p => p.User.Roles)
                    .ToList();

                return View("IndexForAdmin", viewModel);
            }
            else
            {
                var viewModel = Context.Players.ToList();
                return View(viewModel);
            }
        }

        /// <summary>
        /// Returns the player details view.
        /// </summary>
        /// <param name="id">The player name.</param>
        /// <returns>The details view if the player with the specified name exists;
        /// otherwise returns HttpNotFound response.</returns>
        public ActionResult Details(string id)
        {
            var player = Context.Players.SingleOrDefault(p => p.Name == id);
            var currentPlayer = CurrentPlayer;

            if (player == null)
                return HttpNotFound();

            var activeAIs = currentPlayer?.ActiveAIs?.Split(';').ToList() ?? new List<string>();

            var orderedPlayers = Context.Players.OrderByDescending(p => p.Score).ToList();
            var rank = 1 + orderedPlayers.TakeWhile(orderedPlayer => orderedPlayer.UserId != player.UserId).Count();

            var lastMatches = Context.Matches
                .Include(m => m.Player1)
                .Include(m => m.Player2)
                .Where(m => m.Player1.UserId == player.UserId || m.Player2.UserId == player.UserId)
                .OrderByDescending(m => m.Time)
                .Take(5).ToList();

            var lastTournaments = Context.Tournaments
                .Include(t => t.Players.Select(tp => tp.Player))
                .AsEnumerable() // to use comparison with player (only allowed in memory!)
                .Where(t => t.TournamentState == TournamentState.Finished && 
                            t.Players.Any(tp => tp.Player == player))
                .OrderByDescending(t => t.StartTime)
                .Take(5).ToList();

            lastTournaments.Sort(new JoinedTournamentComparer());

            var viewModel = new PlayerDetailsViewModel()
            {
                Player = player,
                CurrentPlayer = currentPlayer,
                LastMatches = lastMatches,
                LastTournaments = lastTournaments,
                ActiveAIs = activeAIs,
                SelectedAi = currentPlayer?.SelectedAI,
                Rank = rank
            };

            return View("Details", viewModel);
        }
    }
}