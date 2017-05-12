using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.ServiceModel;
using System.Web.Http;
using FootballAIGame.DbModel.Models;
using FootballAIGame.Web.Dtos;
using FootballAIGame.Web.GameServerService;
using FootballAIGame.Web.Utilities;

namespace FootballAIGame.Web.Controllers.Api
{
    /// <summary>
    /// Provides services for managing the user's tournaments.
    /// </summary>
    [Authorize]
    public class TournamentsController : BaseApiController
    {
        /// <summary>
        /// Join with the specified tournament with the specified AI.
        /// </summary>
        /// <param name="tournamentId">The tournament identifier.</param>
        /// <param name="aiName">Name of the AI.</param>
        /// <returns>OK <see cref="IHttpActionResult"/> if the action was successful; otherwise
        /// returns a bad request with an error message.</returns>
        [HttpPost]
        [Route("api/tournaments/JoinTournament/{tournamentId}/{aiName}")]
        public IHttpActionResult JoinTournament(int tournamentId, string aiName)
        {
            var tournament = Context.Tournaments
                .Include(t => t.Players)
                .SingleOrDefault(t => t.Id == tournamentId);

            if (tournament == null)
                return BadRequest("Invalid tournament ID.");

            if (tournament.TournamentState != TournamentState.Unstarted)
                return BadRequest("The tournament has already started.");

            var player = CurrentPlayer;

            if (tournament.Players == null)
                tournament.Players = new List<TournamentPlayer>();

            if (tournament.Players.Any(tp => tp.Player == player))
                return BadRequest("Player is already joined.");

            if (string.IsNullOrEmpty(aiName))
                return BadRequest("Invalid AI.");

            if (tournament.Players.Count >= tournament.MaximumNumberOfPlayers)
                return BadRequest("Tournament is currently full.");

            tournament.Players.Add(new TournamentPlayer()
            {
                Player = player,
                PlayerAi = aiName,
                PlayerPosition = null
            });

            Context.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Leaves the tournament.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>OK <see cref="IHttpActionResult"/> if the action was successful; otherwise
        /// returns a bad request with an error message.</returns>
        [HttpPut]
        public IHttpActionResult LeaveTournament(int id)
        {
            var tournament = Context.Tournaments
                .Include(t => t.Players.Select(tp => tp.Player))
                .SingleOrDefault(t => t.Id == id);

            if (tournament == null)
                return BadRequest("Invalid tournament ID");

            if (tournament.Players == null)
                return Ok();

            var tournamentPlayer = tournament.Players.SingleOrDefault(tp => tp.Player == CurrentPlayer);
            if (tournamentPlayer == null)
                return BadRequest("Player is not in the tournament.");

            if (tournament.TournamentState == TournamentState.Unstarted)
                tournament.Players.Remove(tournamentPlayer);

            if (tournament.TournamentState == TournamentState.Running)
            {
                if (tournamentPlayer.Player.PlayerState == PlayerState.PlayingTournamentPlaying)
                    try
                    {
                        using (var gameServer = new GameServerServiceClient())
                        {
                            gameServer.RemoveFromRunningTournament(tournamentPlayer.Player.Name);
                        }
                    }
                    catch (Exception ex) when (ex is CommunicationObjectFaultedException || ex is EndpointNotFoundException)
                    {
                        // ignored
                    }

                tournamentPlayer.Player.PlayerState = PlayerState.Idle;
            }

            Context.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Gets the tournament position.
        /// </summary>
        /// <param name="id">The tournament identifier.</param>
        /// <returns>OK <see cref="IHttpActionResult"/> with the tournament position 
        /// if the action was successful; otherwise returns a bad request with an error message.</returns>
        [HttpGet]
        public IHttpActionResult GetTournamentPosition(int id)
        {
            var tournament = Context.Tournaments
                .Include(t => t.Players.Select(tp => tp.Player))
                .SingleOrDefault(t => t.Id == id);

            if (tournament == null)
                return NotFound();

            var tournamentPlayer = tournament.Players
                .SingleOrDefault(tp => tp.Player == CurrentPlayer);

            if (tournamentPlayer == null)
                return NotFound();

            if (tournamentPlayer.PlayerPosition == null)
                return BadRequest();

            return Ok(tournamentPlayer.PlayerPosition);
        }

        /// <summary>
        /// Gets the joined tournaments.
        /// </summary>
        /// <returns>OK <see cref="IHttpActionResult"/> with the list of
        /// <see cref="TournamentTableEntryDto"/> instances.</returns>
        [HttpGet]
        public IHttpActionResult GetJoinedTournaments()
        {
            var player = CurrentPlayer;

            var joinedTournaments = Context.Tournaments
                .Include(t => t.Players.Select(tp => tp.Player))
                .AsEnumerable() // to use comparison with player (only allowed in memory!)
                .Where(t => t.Players.Any(tp => tp.Player == player))
                .ToList();

            joinedTournaments.Sort(new JoinedTournamentComparer());

            // unstarted + 5 newest finished
            var i = 0;

            joinedTournaments = joinedTournaments
                .TakeWhile(t => t.TournamentState == TournamentState.Unstarted || i++ < 5)
                .ToList();

            var joinedTournamentsDtos = joinedTournaments
                .Select(t => new TournamentTableEntryDto()
                {
                    Id = t.Id,
                    StartTime = t.StartTime,
                    TournamentState = t.TournamentState,
                    Name = t.Name,
                    MaximumNumberOfPlayers = t.MaximumNumberOfPlayers,
                    CurrentNumberOfPlayers = t.Players.Count,
                    CurrentPlayerJoinedAi = t.Players.Single(tp => tp.Player.Name == player.Name).PlayerAi
                }
                );

            return Ok(joinedTournamentsDtos);
        }

        /// <summary>
        /// Gets the tournament information contained in <see cref="TournamentInfoDto"/>.
        /// </summary>
        /// <param name="id">The tournament identifier.</param>
        /// <returns><see cref="TournamentInfoDto"/> contained in OK <see cref="IHttpActionResult"/> if the specified
        /// tournament exists; otherwise returns NotFound response.</returns>
        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult GetTournamentInfo(int id)
        {
            var tournament = Context.Tournaments
                .Include(t => t.Players.Select(tp => tp.Player))
                .Include(t => t.Matches.Select(m => m.Player1))
                .Include(t => t.Matches.Select(m => m.Player2))
                .SingleOrDefault(t => t.Id == id);

            if (tournament == null)
                return NotFound();

            var playersDto = tournament.Players.Select(tp => new TournamentPlayerDto()
            {
                Name = tp.Player.Name,
                Position = tp.PlayerPosition,
                Score = tp.Player.Score
            }).OrderBy(p => p.Position).ThenByDescending(p => p.Score).ToList();


            var matchesDto = tournament.Matches.Select(m => new MatchDto()
            {
                Id = m.Id,
                Winner = m.Winner,
                Score = m.Score,
                Player1Name = m.Player1.Name,
                Player2Name = m.Player2.Name,
            }).ToList();

            var dto = new TournamentInfoDto()
            {
                Players = playersDto,
                Matches = matchesDto,
                TournamentState = tournament.TournamentState
            };

            return Ok(dto);
        }

        /// <summary>
        /// Deletes the specified tournament.
        /// </summary>
        /// <param name="id">The tournament identifier.</param>
        [HttpDelete]
        [Authorize(Roles = RolesNames.TournamentAdmin)]
        public IHttpActionResult DeleteTournament(int id)
        {
            var tournament = Context.Tournaments
                .Include(t => t.RecurringTournament)
                .SingleOrDefault(t => t.Id == id);

            if (tournament == null)
                return BadRequest("A tournament with the specified ID doesn't exist.");

            var isReccuring = tournament.RecurringTournament != null; // because of the second use after delete op.

            if (isReccuring)
                PlanNextRecurring(tournament.RecurringTournament.Id);

            var res = DeleteTournament(tournament);
            if (res != "")
                return BadRequest(res);

            Context.SaveChanges();

            if (isReccuring)
                return Ok("The tournament was deleted, but because it is a part of a recurring tournament, a next" +
                          " tournament was planned. (reload page to see the new one)");

            return Ok("The tournament was deleted.");
        }

        /// <summary>
        /// Deletes the tournament.
        /// </summary>
        /// <param name="tournament">The tournament.</param>
        /// <returns>OK <see cref="IHttpActionResult"/> if the action was successful; otherwise
        /// returns a bad request with an error message.</returns>
        private string DeleteTournament(Tournament tournament)
        {
            if (tournament == null)
                return "A tournament with the specified ID doesn't exist.";

            if (tournament.TournamentState == TournamentState.Running) // if it was already running
                return "The tournament has already started and cannot be deleted anymore.";

            // remove tournament matches (though there shouldn't be any!)
            Context.Matches.RemoveRange(
                Context.Matches.Where(m => m.TournamentId == tournament.Id));

            Context.Tournaments.Remove(tournament);

            return "";
        }

        /// <summary>
        /// Deletes the specified recurring tournament.
        /// </summary>
        /// <param name="id">The recurring tournament identifier.</param>
        /// <param name="deleteUnstarted">if set to <c>true</c> then it also deletes all created unstarted tournaments
        /// belonging to the specified recurring tournament.</param>
        /// <returns>OK <see cref="IHttpActionResult"/> if the action was successful; otherwise
        /// returns a bad request with an error message.</returns>
        [Route("api/tournaments/DeleteRecurringTournament/{id}/{deleteUnstarted}")]
        [HttpDelete]
        [Authorize(Roles = RolesNames.TournamentAdmin)]
        public IHttpActionResult DeleteRecurringTournament(int id, bool deleteUnstarted)
        {
            var reccuringTournament = Context.RecurringTournaments
                .Include(rt => rt.Tournaments)
                .SingleOrDefault(t => t.Id == id);

            if (reccuringTournament == null)
                return NotFound();

            foreach (var tournament in reccuringTournament.Tournaments)
            {
                tournament.RecurringTournament = null;
            }

            if (deleteUnstarted)
            {
                var unstarted = reccuringTournament.Tournaments
                    .Where(t => t.TournamentState == TournamentState.Unstarted).ToList();

                foreach (var tournament in unstarted)
                {
                    DeleteTournament(tournament);
                }
            }

            Context.RecurringTournaments.Remove(reccuringTournament);
            Context.SaveChanges();
            return Ok();
        }

        /// <summary>
        /// Creates and plans the next tournament from the specified <see cref="RecurringTournament"/>.
        /// </summary>
        /// <param name="id">The recurring tournament's ID.</param>
        private void PlanNextRecurring(int id)
        {
            var recurringTournament =
                Context.RecurringTournaments.Include(rt => rt.Tournaments).SingleOrDefault(t => t.Id == id);

            if (recurringTournament == null)
                return;

            // the current time if there are no tournaments
            var lastTournamentTime = DateTime.Now;
            if (recurringTournament.Tournaments.Any(t => true))
                lastTournamentTime = recurringTournament.Tournaments.Max(t => t.StartTime);

            var nextTournament = new Tournament()
            {
                StartTime = lastTournamentTime + TimeSpan.FromMinutes(recurringTournament.RecurrenceInterval),
                TournamentState = TournamentState.Unstarted,
                Name = recurringTournament.Name,
                MinimumNumberOfPlayers = recurringTournament.MinimumNumberOfPlayers,
                MaximumNumberOfPlayers = recurringTournament.MaximumNumberOfPlayers,
                RecurringTournament = recurringTournament
            };

            Context.Tournaments.Add(nextTournament);
            Context.SaveChanges();

            // try plan
            try
            {
                using (var gameServer = new GameServerServiceClient())
                {
                    gameServer.PlanTournament(nextTournament.Id);
                }
            }
            catch (Exception ex) when (ex is CommunicationObjectFaultedException || ex is EndpointNotFoundException)
            {
                // ignore
            }

        }
    }
}
