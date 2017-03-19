namespace FootballAIGameServer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Challenge
    {
        [StringLength(128)]
        public string ChallengedPlayer_UserId { get; set; }

        [StringLength(128)]
        public string ChallengingPlayer_UserId { get; set; }

        public int Id { get; set; }

        public virtual Player Player { get; set; }

        public virtual Player Player1 { get; set; }
    }
}
