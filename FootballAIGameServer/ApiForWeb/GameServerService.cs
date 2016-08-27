using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using FootballAIGameServer.Models;

namespace FootballAIGameServer.ApiForWeb
{
    public class GameServerService : IGameServerService
    {
        public string WantsToPlay(string userName, string ai)
        {
            try
            {
                var manager = ConnectionManager.Instance;
                lock (manager.ActiveConnections)
                {
                    var connection = manager.ActiveConnections
                        .FirstOrDefault(c => c.PlayerName == userName && c.AiName == ai);
                    if (connection == null)
                        return "Ai is no longer active.";

                    if (manager.WantsToPlayConnections.Count == 0)
                    {
                        manager.WantsToPlayConnections.Add(connection);
                    }
                    else // start match
                    {
                        var otherPlayerConnection = manager.WantsToPlayConnections[0];
                        manager.WantsToPlayConnections.Remove(otherPlayerConnection);

                        using (var context = new ApplicationDbContext())
                        {
                            var player1 = context.Players.FirstOrDefault(p => p.Name == connection.PlayerName);
                            var player2 = context.Players.FirstOrDefault(p => p.Name == otherPlayerConnection.PlayerName);
                            player1.PlayerState = PlayerState.PlayingMatch;
                            player2.PlayerState = PlayerState.PlayingMatch;

                            context.SaveChanges();
                        }

                        StartGame(connection.PlayerName, connection.AiName,
                            otherPlayerConnection.PlayerName, otherPlayerConnection.AiName);
                    }

                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("wants to play exc");
                Console.WriteLine(ex.Message);
            }
            return "ok";
        }

        public string StartGame(string userName1, string ai1, string userName2, string ai2)
        {
            var manager = ConnectionManager.Instance;
            lock (manager.ActiveConnections)
            {
                var connection1 = manager.ActiveConnections
                    .FirstOrDefault(c => c.PlayerName == userName1 && c.AiName == ai1);
                var connection2 = manager.ActiveConnections
                    .FirstOrDefault(c => c.PlayerName == userName2 && c.AiName == ai2);

                if (connection1 == null || connection2 == null)
                    return "Ai is no longer active.";

                MatchSimulator matchSimulator = new MatchSimulator(connection1, connection2);
                matchSimulator.SimulateMatch();

                return "ok";
            }
        }

        public void CancelMatch(string playerName)
        {
            foreach (var runningSimulation in MatchSimulator.RunningSimulations)
            {
                if (runningSimulation.Player1AiConnection.PlayerName == playerName)
                    runningSimulation.Player1CancelRequested = true;
                if (runningSimulation.Player2AiConnection.PlayerName == playerName)
                    runningSimulation.Player2CancelRequested = true;
            }
        }

        public void CancelLooking(string playerName)
        {
            ConnectionManager.Instance.WantsToPlayConnections.RemoveAll(p => p.PlayerName == playerName);
        }
    }
}
