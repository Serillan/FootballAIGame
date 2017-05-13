using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FootballAIGame.DbModel.Models;
using FootballAIGame.Web.Utilities;
using FootballAIGame.Web.ViewModels.Matches;
using Microsoft.AspNet.Identity;

namespace FootballAIGame.Web.Controllers
{
    /// <summary>
    /// The matches section controller.
    /// </summary>
    public class MatchesController : BaseController
    {
        /// <summary>
        /// Returns the matches index view.
        /// </summary>
        /// <returns>The index matches view.</returns>
        public ActionResult Index()
        {
            var viewModel = Context.Matches
                .Include(m => m.Player1)
                .Include(m => m.Player2)
                .ToList();

            return View(viewModel);
        }

        /// <summary>
        /// Returns the match details view.
        /// </summary>
        /// <param name="id">The match identifier.</param>
        /// <returns>The details view if the match with the specified identifier exists;
        /// otherwise returns HttpNotFound response.</returns>
        public ActionResult Details(int id)
        {
            var match = Context.Matches
                .Include(m => m.Player1)
                .Include(m => m.Player2)
                .SingleOrDefault(m => m.Id == id);

            if (match == null)
                return HttpNotFound();


            var model = new MatchDetailsViewModel() { Match = match };

            var currentPlayer = CurrentPlayer;

            if (currentPlayer != null)
            {
                if (currentPlayer.Name == match.Player1.Name)
                    model.LoggedPlayerActionLatency = match.Player1AverageActionLatency;
                if (currentPlayer.Name == match.Player2.Name)
                    model.LoggedPlayerActionLatency = match.Player2AverageActionLatency;
            }

            return View("Details", model);
        }

        /// <summary>
        /// Returns the match errors view.
        /// </summary>
        /// <param name="id">The match identifier.</param>
        /// <returns>The errors view if the match with the specified identifier exists;
        /// otherwise returns HttpNotFound response.</returns>
        public ActionResult Errors(int id)
        {
            var match = Context.Matches
                .Include(m => m.Player1)
                .Include(m => m.Player2)
                .SingleOrDefault(m => m.Id == id);

            if (match == null)
                return HttpNotFound();

            return View("Errors", match);
        }

        /// <summary>
        /// Returns the match watch view.
        /// </summary>
        /// <param name="id">The match identifier.</param>
        /// <returns>The watch view if the match with the specified identifier exists;
        /// otherwise returns HttpNotFound response.</returns>
        public ActionResult Watch(int id)
        {
            var match = Context.Matches
                .Include(m => m.Player1)
                .Include(m => m.Player2)
                .SingleOrDefault(m => m.Id == id);

            if (match == null)
                return HttpNotFound();

            return View("Watch", match);
        }
    }
}