using System.Collections.Generic;

namespace FootballAIGame.MatchSimulator
{
    public class MatchInfo
    {
        public List<float> MatchData { get; set; }

        /// <summary>
        /// Gets or sets the goals that were scored in the match. <para/>
        /// Format: "goalTime;userName;playerK|...", where K is number of player from userName team who scored
        /// and '|' is the separator between goals.
        /// </summary>
        /// <value>
        /// The goals.
        /// </value>
        public string Goals { get; set; }

        public string Player1ErrorLog { get; set; }

        public string Player2ErrorLog { get; set; }

        public string Winner { get; set; }

        public int Goals1 { get; set; }

        public int Goals2 { get; set; }

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

        public MatchInfo()
        {
            MatchData = new List<float>();
            Player1ErrorLog = "";
            Player2ErrorLog = "";
            Goals = "";
        }

    }

    
}
