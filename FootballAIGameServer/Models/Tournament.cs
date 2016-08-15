using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FootballAIGameServer.Models
{
    public class Tournament
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public DateTime StartTime { get; set; }

        public int MaximumNumberOfPlayers { get; set; }

        public ICollection<Player> Players { get; set; }

    }
}