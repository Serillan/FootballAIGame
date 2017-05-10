using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using FootballAIGame.DbModel.Models;
using FootballAIGame.Web.Utilities;

namespace FootballAIGame.Web.Controllers.Api
{
    [Authorize]
    public class AIsController : BaseApiController
    {
        /// <summary>
        /// Selects the specified AI.
        /// </summary>
        /// <param name="id">The AI name.</param>
        /// <returns>OK <see cref="IHttpActionResult"/>.</returns>
        [HttpPut]
        public IHttpActionResult SelectAI(string id)
        {
            CurrentPlayer.SelectedAI = id;

            Context.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// If the specified AI is not currently selected, select it. Otherwise
        /// deselects it.
        /// </summary>
        /// <param name="id">The AI name.</param>
        /// <returns>OK <see cref="IHttpActionResult"/>.</returns>
        [HttpPut]
        public IHttpActionResult ToggleAI(string id)
        {
            var player = CurrentPlayer;

            player.SelectedAI = player.SelectedAI == id ? null : id;

            Context.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Gets the active AIs names that are connected to the game server
        /// with the current player name.
        /// </summary>
        /// <returns>OK <see cref="IHttpActionResult"/> with the player's active AIs' names.</returns>
        [HttpGet]
        public IHttpActionResult GetActiveAIs()
        {
            var activeAIs = CurrentPlayer.ActiveAIs?.Split(';').ToList() ?? new List<string>();
            return Ok(activeAIs);
        }
    }
}
