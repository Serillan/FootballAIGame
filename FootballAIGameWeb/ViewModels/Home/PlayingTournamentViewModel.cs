using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FootballAIGameWeb.Models;

namespace FootballAIGameWeb.ViewModels.Home
{
    public class PlayingTournamentViewModel
    {
        public Tournament Tournament { get; set; }

        public Player CurrentPlayer { get; set; }
    }
}