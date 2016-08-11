using System.Collections.Generic;
using FootballAIGameWeb.Models;

namespace FootballAIGameWeb.ViewModels.Home
{
    public class IndexViewModel
    {
        public List<Match> LastMatches { get; set; }

        public List<Tournament> NextTournaments { get; set; }

    }
}