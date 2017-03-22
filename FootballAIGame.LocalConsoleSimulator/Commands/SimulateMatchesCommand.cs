using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FootballAIGame.LocalSimulationBase;
using FootballAIGame.LocalSimulationBase.Models;
using FootballAIGame.MatchSimulation.Models;

namespace FootballAIGame.LocalConsoleSimulator.Commands
{
    /// <summary>
    /// Represents the simulation command.
    /// </summary>
    public class SimulateMatchesCommand : ICommand
    {
        /// <summary>
        /// Gets or sets a value indicating whether the extended match result should be shown.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the extended match result should be shown; otherwise, <c>false</c>.
        /// </value>
        public bool ExtendedResultOn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the saving to the specified <see cref="SavingDirectory"/> and
        /// <see cref="SavingFiles"/> should be done.
        /// </summary>
        /// <value>
        /// <c>true</c> if the saving to the specified <see cref="SavingDirectory"/> and
        /// <see cref="SavingFiles"/> should be done; otherwise, <c>false</c>.
        /// </value>
        public bool SavingOn { get; set; }

        /// <summary>
        /// Gets or sets the saving directory to which the match results will be saved during command execution
        /// if the <see cref="SavingOn"/> is set to <c>true</c>.
        /// </summary>
        /// <value>
        /// The <see cref="DirectoryInfo"/> instance containing the directory.
        /// </value>
        public DirectoryInfo SavingDirectory { get; set; }

        /// <summary>
        /// Gets or sets the saving files to which the match results will be saved during command execution
        /// if the <see cref="SavingOn"/> is set to <c>true</c>.
        /// </summary>
        /// <value>
        /// The array of <see cref="FileInfo"/> instances containing the files to which the matches are saved.
        /// </value>
        public FileInfo[] SavingFiles { get; set; }

        /// <summary>
        /// Gets or sets the pairs of opponents (their names) that will be in match against each other during the command execution.
        /// </summary>
        /// <value>
        /// The <see cref="IList{T}"/> of <see cref="Tuple{T1, T2}"/> that represent the pairs of opponents.
        /// </value>
        public IList<Tuple<string, string>> Opponents { get; set; }

        /// <summary>
        /// Executes the command asynchronously.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous execute operation.
        /// </returns>
        public async Task ExecuteAsync()
        {
            var manager = SimulationManager.Instance;

            if (!CheckForAiDuplicates())
                return;

            if (Program.IsVerbose)
                Console.WriteLine("Starting simulations.");

            var simulations = Opponents.Select(pair => manager.SimulateAsync(pair.Item1, pair.Item2)).ToList();

            if (Program.IsVerbose)
                Console.WriteLine("Simulating...");

            var errorMessages = new List<string>();

            foreach (var simulation in simulations)
            {
                try
                {
                    await simulation;
                }
                catch (Exception ex)
                {
                    errorMessages.Add($"Error: {ex.Message}");
                }
            }

            if (errorMessages.Count > 0)
            {
                errorMessages.ForEach(msg => Console.Error.WriteLine(msg));
                return;
            }

            var matches = simulations.Select(s => s.Result).ToArray();

            Console.WriteLine(GetResultMessage(matches));

            if (SavingOn)
                SaveMatches(matches);
        }

        /// <summary>
        /// Saves the specified matches to the specified <see cref="SavingDirectory"/> and <see cref="SavingFiles"/> if they
        /// are different from null respectively.
        /// </summary>
        /// <param name="matches">The matches to be saved.</param>
        private void SaveMatches(IReadOnlyList<Match> matches)
        {
            if (SavingDirectory != null)
            {
                if (Program.IsVerbose)
                    Console.WriteLine($"Saving matches to directory: {SavingDirectory}");

                foreach (var match in matches)
                {
                    SaveMatch(Path.Combine(SavingDirectory.FullName, $"{match.Ai1Name}_{match.Ai2Name}.json"), match);
                }
            }

            if (SavingFiles != null)
            {
                if (Program.IsVerbose)
                    Console.WriteLine("Saving matches to the specified files.");

                for (int i = 0; i < SavingFiles.Length && i < matches.Count; i++)
                {
                    var file = SavingFiles[i];
                    var match = matches[i];

                    SaveMatch(file.FullName, match);
                }
            }
        }

        /// <summary>
        /// Saves the specified match to the file specified by the path to that file.
        /// </summary>
        /// <param name="filePath">The save file path.</param>
        /// <param name="match">The match to be saved.</param>
        private static void SaveMatch(string filePath, Match match)
        {
            try
            {
                var stream = File.Create(filePath);
                match.Save(stream);
                stream.Close();
            }
            catch (Exception)
            {
                Console.Error.WriteLine($"Error: Couldn't save the match ({match.Ai1Name} vs {match.Ai2Name}) to " +
                              $"file {filePath}.");
            }
        }

        /// <summary>
        /// Checks whether there are duplicate names in the <see cref="Opponents"/>.
        /// If there are duplicates it also writes the error message to the standard error output.
        /// </summary>
        /// <returns><c>true</c> if there are duplicate names in the <see cref="Opponents"/>; 
        /// otherwise, <c>false</c>.</returns>
        private bool CheckForAiDuplicates()
        {
            var duplicates = Opponents
                .SelectMany(pair => new List<string>() { pair.Item1, pair.Item2 })
                .GroupBy(aiName => aiName)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key).ToArray();

            if (duplicates.Length <= 0) return true;

            Console.Error.Write("Error: The following AIs are used more than once - ");
            Console.Error.WriteLine(string.Join(", ", duplicates));
            return false;
        }

        /// <summary>
        /// Gets the result message describing the specified matches.
        /// </summary>
        /// <param name="matches">The matches.</param>
        /// <returns>The result message.</returns>
        private string GetResultMessage(IEnumerable<Match> matches)
        {
            string[] messages;

            if (ExtendedResultOn)
            {
                messages = (from match in matches
                            let matchInfo = match.MatchInfo
                            let team1 = matchInfo.Team1Statistics
                            let team2 = matchInfo.Team2Statistics
                            let goals = string.Join(";", matchInfo.Goals.Select(g => $"{g.ScoreTime}," +
                                                                    $"{(g.TeamThatScored == Team.FirstPlayer ? match.Ai1Name : match.Ai2Name)}," +
                                                                    $"Player{g.ScorerNumber}"))
                            let errors = string.Join(";", matchInfo.Errors.Select(e => GetErrorMessage(e, match.Ai1Name, match.Ai2Name)))
                            select $"({team1.Goals}:{team2.Goals}, {team1.Shots}:{team2.Shots}, {team1.ShotsOnTarget}:{team2.ShotsOnTarget}, " +
                                   $"[{goals}], [{errors}])").ToArray();

            }
            else
            {
                messages = matches
                    .Select(m => m.MatchInfo)
                    .Select(mi => $"{mi.Team1Statistics.Goals}:{mi.Team2Statistics.Goals}").ToArray();
            }


            return string.Join(", ", messages);
        }

        /// <summary>
        /// Gets the error message corresponding to the specified error.
        /// </summary>
        /// <param name="error">The simulation error.</param>
        /// <param name="ai1">The first AI from the simulation.</param>
        /// <param name="ai2">The second AI from the simulation.</param>
        /// <returns>The error message corresponding to the specified error.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The specified <paramref name="error"/> doesn't have
        /// a corresponding error message.</exception>
        private static string GetErrorMessage(SimulationError error, string ai1, string ai2)
        {
            var ai = error.Team == Team.FirstPlayer ? ai1 : ai2;

            switch (error.Reason)
            {
                case SimulationErrorReason.TooHighSpeed:
                    return $"{error.Time} : {ai} - Player{error.AffectedPlayerNumber} has too high speed.";
                case SimulationErrorReason.TooHighAcceleration:
                    return $"{error.Time} : {ai} - Player{error.AffectedPlayerNumber} has too high acceleration.";
                case SimulationErrorReason.TooStrongKick:
                    return $"{error.Time} : {ai} - Player{error.AffectedPlayerNumber} has made too strong kick.";
                case SimulationErrorReason.InvalidMovementVector:
                    return $"{error.Time} : {ai} - Player{error.AffectedPlayerNumber} has invalid movement vector set.";
                case SimulationErrorReason.InvalidKickVector:
                    return $"{error.Time} : {ai} - Player{error.AffectedPlayerNumber} has invalid kick vector set.";
                case SimulationErrorReason.Disconnection:
                    return $"{error.Time} : {ai} - Player has disconnected.";
                case SimulationErrorReason.Cancellation:
                    return $"{error.Time} : {ai} - Player has left the match.";
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }
    }
}
