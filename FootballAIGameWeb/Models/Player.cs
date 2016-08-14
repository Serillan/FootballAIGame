using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballAIGameWeb.Models
{
    public class Player
    {
        [Key, ForeignKey("User")]
        public string UserId { get; set; }

        public User User { get; set; }

        public string Name { get; set; }

        public ICollection<Tournament> Tournaments { get; set; }

        public string SelectedAi { get; set; }

        public PlayerState PlayerState { get; set; }

        public int Score { get; set; }

        public int WonGames { get; set; }

        public int WonTournaments { get; set; }

    }

    public enum PlayerState
    {
        PlayingMatch,
        WaitingForOpponentToAcceptChallenge,
        LookingForOpponent,
        Idle
    }
}