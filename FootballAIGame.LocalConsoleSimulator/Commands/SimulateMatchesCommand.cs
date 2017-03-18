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
    /// The simulate matches command.
    /// </summary>
    public class SimulateMatchesCommand : ICommand
    {
        public bool ExtendedResultOn { get; set; }

        public bool SavingOn { get; set; }

        public DirectoryInfo SavingDirectory { get; set; }

        public FileInfo[] SavingFiles { get; set; }

        public List<Tuple<string, string>> Opponents { get; set; }

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
