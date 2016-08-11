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

        /// <summary>
        /// Returns 1 if Player1 won, otherwise returns false.
        /// </summary>
        /// <value>1 if Player1 won, otherwise returns false.</value>
        public int Winner { get; set; }
    }
}