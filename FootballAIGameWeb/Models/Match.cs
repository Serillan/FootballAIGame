using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FootballAIGameWeb.Models
{
    public class Match
    {
        public int Id { get; set; }

        public DateTime Time { get; set; }

        public Player Player1 { get; set; }

        public string Player1Ai { get; set; }

        public Player Player2 { get; set; }

        public string Player2Ai { get; set; }

        public string Score { get; set; }

        public string Goals { get; set; }

        public int Shots1 { get; set; }

        public int Shots2 { get; set; }

        public int ShotsOnTarget1 { get; set; }

        public int ShotsOnTarget2 { get; set; }

        public string Player1ErrorLog { get; set; }

        public string Player2ErrorLog { get; set; }

        public byte[] MatchData { get; set; }


        /// <summary>
        /// Returns 1 if Player1 won, otherwise returns false.
        /// </summary>
        /// <value>1 if Player1 won, otherwise returns false.</value>
        public int Winner { get; set; }
    }
}