using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FootballAIGame.Web.Models;

namespace FootballAIGame.Web.ViewModels.Tournaments
{
    public class TournamentDetailsViewModel
    {
        public Tournament Tournament { get; set; }

        public List<string> ActiveAIs { get; set; }

        public Player CurrentPlayer { get; set; }

        public TournamentPlayer CurrentTournamentPlayer { get; set; }
    }
}