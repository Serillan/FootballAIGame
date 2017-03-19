using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using FootballAIGame.Web.Models;

namespace FootballAIGame.Web.ViewModels.Home
{
    public class PlayerHomeViewModel
    {
        public Player Player { get; set; }

        public List<string> ActiveAIs { get; set; }

        public List<Player> Challenges { get; set; }

        public List<Match> LastMatches { get; set; }

        public List<Tournament> JoinedTournaments { get; set; }

        [Display(Name = "Name")]
        public string OpponentPlayerName { get; set; }

        public string SelectedAi { get; set; }
    }
}