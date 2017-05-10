namespace FootballAIGame.DbModel.Models
{
    /// <summary>
    /// Identifies tournament's states.
    /// </summary>
    public enum TournamentState
    {
        /// <summary>
        /// Tournament has not started yet.
        /// </summary>
        Unstarted,
        /// <summary>
        /// Tournament is currently being simulated.
        /// </summary>
        Running,
        /// <summary>
        /// Tournament has already finished.
        /// </summary>
        Finished,
        /// <summary>
        /// Tournament was closed because there were not enough players signed at start time.
        /// </summary>
        NotEnoughtPlayersClosed,
        /// <summary>
        /// Tournament was closed because there was an error during it's simulation.
        /// </summary>
        ErrorClosed
    }
}