using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FootballAIGameWeb.Models;

namespace FootballAIGameWeb.Dtos
{
    public class TournamentInfoDto
    {
        public TournamentState TournamentState { get; set; }

        public ICollection<MatchDto> Matches { get; set; }

        public ICollection<TournamentPlayerDto> Players { get; set; }
    }
}