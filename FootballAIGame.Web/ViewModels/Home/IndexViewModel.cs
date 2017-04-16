using System.Collections.Generic;
using FootballAIGame.DbModel.Models;

namespace FootballAIGame.Web.ViewModels.Home
{
    public class IndexViewModel
    {
        public List<Match> LastMatches { get; set; }

        public List<Tournament> NextTournaments { get; set; }

    }
}