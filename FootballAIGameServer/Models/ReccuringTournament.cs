using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballAIGameServer.Models
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
