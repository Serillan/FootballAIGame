using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FootballAIGame.DbModel.Models;

namespace FootballAIGame.Web.ViewModels.Matches
{
    public class MatchDetailsViewModel
    {
        public Match Match { get; set; }

        public int? LoggedPlayerActionLatency { get; set; }
    }
}