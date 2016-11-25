using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FootballAIGameWeb.Models;

namespace FootballAIGameWeb.Dtos
{
    public class TournamentTableEntryDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime StartTime { get; set; }

        public TournamentState TournamentState { get; set; }

        public int MaximumNumberOfPlayers { get; set; }

        public int CurrentNumberOfPlayers { get; set; }

        public string CurrentPlayerJoinedAi { get; set; }
    }
}