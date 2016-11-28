using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FootballAIGameWeb.Models
{
    public class ReccuringTournament : TournamentBase
    {
        [DisplayName("Reccurence Interval (minutes)")]
        public int RecurrenceInterval { get; set; }

        [DisplayName("Number of Present Tournaments")]
        public int NumberOfPresentTournaments { get; set; }

        public ICollection<Tournament> Tournaments { get; set; }
    }
}