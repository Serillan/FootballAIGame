namespace FootballAIGame.DbModel.Models
{
    /// <summary>
    /// Identifies player's states.
    /// </summary>
    public enum PlayerState
    {
        /// <summary>
        /// Player is currently in a match with his selected AI.
        /// </summary>
        PlayingMatch,
        /// <summary>
        /// Player is waiting for opponent to accept the challenge.
        /// </summary>
        WaitingForOpponentToAcceptChallenge,
        /// <summary>
        /// Player is looking for random opponent for the match.
        /// </summary>
        LookingForOpponent,
        /// <summary>
        /// Player is idle.
        /// </summary>
        Idle,
        /// <summary>
        /// Player is currently in a running tournament and waiting for next match.
        /// </summary>
        PlayingTournamentWaiting,
        /// <summary>
        /// Player is currently in a running tournament and playing a match.
        /// </summary>
        PlayingTournamentPlaying
    }
}