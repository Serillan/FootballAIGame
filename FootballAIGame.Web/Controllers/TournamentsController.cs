using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using FootballAIGame.Web.GameServerService;
using FootballAIGame.DbModel.Models;
using FootballAIGame.Web.Utilities;
using FootballAIGame.Web.ViewModels.Tournaments;
using Microsoft.AspNet.Identity;

namespace FootballAIGame.Web.Controllers
{
    public class TournamentsController : BaseController
    {
        /// <summary>
        /// Returns the current tournaments view.
        /// </summary>
        public ActionResult Current()
        {
            var viewModel = Context.Tournaments
                .Include(t => t.Players)
                .Where(t => t.TournamentState == TournamentState.Unstarted || 
                            t.TournamentState == TournamentState.Running);

            return User.IsInRole(RolesNames.TournamentAdmin) ? View("CurrentForAdmin", viewModel) : View(viewModel);
        }

        /// <summary>
        /// Returns the finished tournaments view.
        /// </summary>
        /// <returns></returns>
        public ActionResult Finished()
        {
            var viewModel = Context.Tournaments
                .Include(t => t.Players)
                .Where(t => t.TournamentState != TournamentState.Unstarted &&
                            t.TournamentState != TournamentState.Running);

            return View(viewModel);
        }

        /// <summary>
        /// Returns the details of the specified tournament.
        /// </summary>
        /// <param name="id">The tournament identifier.</param>
        /// <returns>The details of the specified tournament.</returns>
        public ActionResult Details(int id)
        {
            var tournament = Context.Tournaments
                .Include(t => t.Players.Select(tp => tp.Player))  // nested include
                .Include(t => t.Matches.Select(m => m.Player1))
                .Include(t => t.Matches.Select(m => m.Player2))
                .SingleOrDefault(t => t.Id == id);

            if (tournament == null)
                return HttpNotFound();

            tournament.Players = 
                tournament.Players.OrderBy(p => p.PlayerPosition)
                .ThenByDescending(p => p.Player.Score).ToList();

            var currentPlayer = CurrentPlayer;

            var activeAIs = currentPlayer?.ActiveAIs?.Split(';').ToList() ?? new List<string>();
            var currentTournamentPlayer = tournament.Players.SingleOrDefault(tp => tp.Player == currentPlayer);
            
            var viewModel = new TournamentDetailsViewModel()
            {
                Tournament = tournament,
                ActiveAIs = activeAIs,
                CurrentPlayer = currentPlayer,
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
        [Route("tournaments/manage-recurring")]
        public ActionResult ManageRecurring()
        {
            var viewModel = Context.RecurringTournaments;
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
                Context.Tournaments.Add(tournament);
                Context.SaveChanges();
                // let server know
                try
                {
                    using (var gameServer = new GameServerServiceClient())
                    {
                        gameServer.PlanTournament(tournament.Id);
                    }
                }
                catch (Exception ex) when (ex is CommunicationObjectFaultedException || ex is EndpointNotFoundException)
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
        [Route("tournaments/create-recurring")]
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
        [Route("tournaments/create-recurring")]
        public ActionResult CreateRecurring(RecurringTournament recurringTournament)
        {
            if (!ModelState.IsValid) return View(recurringTournament);

            Context.RecurringTournaments.Add(recurringTournament);

            // plan recurringTournament.NumberOfPresentTournaments tournaments
            var time = recurringTournament.StartTime;

            for (int i = 0; i < recurringTournament.NumberOfPresentTournaments; i++)
            {
                var tournament = new Tournament()
                {
                    StartTime = time,
                    TournamentState = TournamentState.Unstarted,
                    Name = recurringTournament.Name,
                    MinimumNumberOfPlayers = recurringTournament.MinimumNumberOfPlayers,
                    MaximumNumberOfPlayers = recurringTournament.MaximumNumberOfPlayers,
                    RecurringTournament = recurringTournament
                };

                Context.Tournaments.Add(tournament);
                Context.SaveChanges();

                // let server know
                try
                {
                    using (var gameServer = new GameServerServiceClient())
                    {
                        gameServer.PlanTournament(tournament.Id);
                    }
                }
                catch (Exception ex) when (ex is CommunicationObjectFaultedException || ex is EndpointNotFoundException)
                {
                    // ignored
                }

                time += TimeSpan.FromMinutes(recurringTournament.RecurrenceInterval);
            }

            return RedirectToAction("ManageRecurring");
        }

    }
}