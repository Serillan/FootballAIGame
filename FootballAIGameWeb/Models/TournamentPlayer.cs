using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FootballAIGameWeb.Models
{
    public class TournamentPlayer
    {
        [Key, ForeignKey("Player"), Column(Order = 0)]
        public string UserId { get; set; }

        [Key, ForeignKey("Tournament"), Column(Order = 1)]
        public int TournamentId { get; set; }

        public Player Player { get; set; }

        public Tournament Tournament { get; set; }

        public int? PlayerPosition { get; set; }

    }
}