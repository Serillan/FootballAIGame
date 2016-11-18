using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FootballAIGameWeb.Dtos
{
    public class MatchDto
    {
        public int Id { get; set; }

        public string Player1Name { get; set; }

        public string Player2Name { get; set; }

        public string Score { get; set; }

        public int Winner { get; set; }
    }
}