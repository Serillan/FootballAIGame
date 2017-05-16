using System;

namespace FootballAIGame.DbModel.Models
{
    /// <summary>
    /// Represents the football match.
    /// </summary>
    public class Match
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the time when the match simulation started.
        /// </summary>
        /// <value>
        /// The time when the match simulation started..
        /// </value>
        public DateTime Time { get; set; }

        /// <summary>
        /// Gets or sets the player1.
        /// </summary>
        /// <value>
        /// The player1.
        /// </value>
        public Player Player1 { get; set; }

        /// <summary>
        /// Gets or sets the player2.
        /// </summary>
        /// <value>
        /// The player2.
        /// </value>
        public Player Player2 { get; set; }

        /// <summary>
        /// Gets or sets the player1 AI name.
        /// </summary>
        /// <value>
        /// The player1 AI name.
        /// </value>
        public string Player1Ai { get; set; }

        /// <summary>
        /// Gets or sets the player2 AI name.
        /// </summary>
        /// <value>
        /// The player2 AI name.
        /// </value>
        public string Player2Ai { get; set; }

        /// <summary>
        /// Gets or sets the final match score. <para />
        /// Format: "NumberOfUser1Goals:NumberOfUser2Goals".
        /// </summary>
        /// <value>
        /// The score.
        /// </value>
        public string Score { get; set; }

        /// <summary>
        /// Gets or sets the goals that were scored in the match. <para/>
        /// Format: "goalTime;userName;playerK|...", where K is number of player from userName team who scored
        /// and '|' is the separator between goals.
        /// </summary>
        /// <value>
        /// The goals.
        /// </value>
        public string Goals { get; set; }

        /// <summary>
        /// Gets or sets the number of player1 team shots.
        /// </summary>
        /// <value>
        /// The number of player1 team shots.
        /// </value>
        public int Shots1 { get; set; }

        /// <summary>
        /// Gets or sets the number of player2 team shots.
        /// </summary>
        /// <value>
        /// The number of player2 team shots.
        /// </value>
        public int Shots2 { get; set; }

        /// <summary>
        /// Gets or sets the number of player1 team shots on target.
        /// </summary>
        /// <value>
        /// The number of player1 team shots on target.
        /// </value>
        public int ShotsOnTarget1 { get; set; }

        /// <summary>
        /// Gets or sets the number of player2 team shots on target.
        /// </summary>
        /// <value>
        /// The number of player2 team shots on target.
        /// </value>
        public int ShotsOnTarget2 { get; set; }

        /// <summary>
        /// Gets or sets the player1's error log. <para />
        /// Format: "Time - ErrorMessage;...", ';' is errors separator.
        /// </summary>
        /// <value>
        /// The error log of the player1.
        /// </value>
        public string Player1ErrorLog { get; set; }

        /// <summary>
        /// Gets or sets the player2's error log. <para />
        /// Format: "Time - ErrorMessage;...", ';' is errors separator.
        /// </summary>
        /// <value>
        /// The error log of the player2.
        /// </value>
        public string Player2ErrorLog { get; set; }

        /// <summary>
        /// Gets or sets the player1's average time that the simulator waited for the team's action to be received.
        /// </summary>
        /// <value>
        /// The player1's average time that the simulator waited for the team's action to be received.
        /// </value>
        public int Player1AverageActionLatency { get; set; }

        /// <summary>
        /// Gets or sets the player2's average time that the simulator waited for the team's action to be received.
        /// </summary>
        /// <value>
        /// The player2's average time that the simulator waited for the team's action to be received.
        /// </value>
        public int Player2AverageActionLatency { get; set; }

        /// <summary>
        /// Gets or sets the match data. 
        /// Match data is binary representation of game states from all simulation steps. <para />
        /// For each step there is a ball position with a movement vector and 22 players positions and movement vectors. <para />
        /// Everything is encoded as float. So each step takes 23*4 = 92 floats = 368 Bytes
        /// </summary>
        /// <value>
        /// The match data.
        /// </value>
        public byte[] MatchData { get; set; }

        /// <summary>
        /// Returns 1 if Player1 won, 2 if Player2 won, otherwise (draw) returns 0.
        /// </summary>
        /// <value>1 if Player1 won, 2 if Player2 won, otherwise (draw) returns 0.</value>
        public int Winner { get; set; }

        /// <summary>
        /// Gets or sets the tournament identifier to which this match belongs.
        /// If the match is not part of a tournament it is equal to null.
        /// </summary>
        public int? TournamentId { get; set; }

        /// <summary>
        /// Gets or sets the tournament to which this match belongs.
        /// If the match is not part of a tournament it is equal to null.
        /// </summary>
        public Tournament Tournament { get; set; }
    }
}