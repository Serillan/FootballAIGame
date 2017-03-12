using System;

namespace FootballAIGame.MatchSimulation.Models
{
    public class SimulationError
    {
        public string Time { get; set; }

        public ErrorType Type { get; set; }

        public Team Team { get; set; }

        public int? AffectedPlayerNumber { get; set; }

        public enum ErrorType
        {
            TooHighSpeed, TooHighAcceleration, TooStrongKick,
            InvalidMovementVector, InvalidKickVector, Disconnection, Cancel
        }
    }
}
