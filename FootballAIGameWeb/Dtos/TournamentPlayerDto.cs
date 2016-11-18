using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FootballAIGameWeb.Dtos
{
    public class TournamentPlayerDto
    {
        public int? Position { get; set; }

        public string Name { get; set; }

        public int Score { get; set; }
    }
}