using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using FootballAIGameWeb.Models;

namespace FootballAIGameWeb.ViewModels.Home
{
    public class PlayerHomeViewModel
    {
        public Player Player { get; set; }

        public List<string> ActiveAIs { get; set; }

        public List<Player> Challenges { get; set; }

        public List<Match> LastMatches { get; set; }

        [Display(Name = "Opponent Name")]
        public string OpponentPlayerName { get; set; }

        [Required(ErrorMessage = "AI must be selected.")]
        public string SelectedAi { get; set; }
    }
}