using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FootballAIGame.DbModel.Models;

namespace FootballAIGame.Web.ViewModels.Players
{
    public class PlayerDetailsViewModel
    {
        public List<Match> LastMatches { get; set; }

        public List<Tournament> LastTournaments { get; set; }

        public List<string> ActiveAIs { get; set; }

        public string SelectedAi { get; set; }

        public Player Player { get; set; }

        public Player CurrentPlayer { get; set; }

        public int Rank { get; set; }
    }
}