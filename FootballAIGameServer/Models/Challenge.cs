using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FootballAIGameServer.Models
{
    public class Challenge
    {
        public int Id { get; set; }

        public Player ChallengingPlayer { get; set; }

        public Player ChallengedPlayer { get; set; }
    }
}