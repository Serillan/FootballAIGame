using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FootballAIGameWeb.Models;

namespace FootballAIGameWeb.ViewModels.Players
{
    public class PlayerDetailsViewModel
    {
        public List<Match> LastMatches { get; set; }

        public List<Tournament> LastTournaments { get; set; }

        public List<string> ActiveAIs { get; set; }

        public Player Player { get; set; }

        public int Rank { get; set; }
    }
}