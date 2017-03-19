using System.Collections.Generic;
using FootballAIGame.Web.Models;

namespace FootballAIGame.Web.ViewModels.Home
{
    public class IndexViewModel
    {
        public List<Match> LastMatches { get; set; }

        public List<Tournament> NextTournaments { get; set; }

    }
}