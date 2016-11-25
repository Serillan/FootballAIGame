using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FootballAIGameWeb.Models;

namespace FootballAIGameWeb.Helpers
{
    public class JoinedTournamentComparer : IComparer<Tournament>
    {
        public int Compare(Tournament x, Tournament y)
        {
            if (x.TournamentState == TournamentState.Unstarted &&
                y.TournamentState != TournamentState.Unstarted)
                return -1;
            if (y.TournamentState == TournamentState.Unstarted &&
                x.TournamentState != TournamentState.Unstarted)
                return 1;
            if (x.TournamentState == TournamentState.Unstarted &&
                y.TournamentState == TournamentState.Unstarted)
                return x.StartTime.CompareTo(y.StartTime); // oldest first (the first that will start)

            // both are finished
            return -x.StartTime.CompareTo(y.StartTime); // newest first (the last that was finished)
        }
    }
}