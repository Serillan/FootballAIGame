using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FootballAIGame.Web.Models;

namespace FootballAIGame.Web.Helpers
{
    /// <summary>
    /// The comparer used for sorting <see cref="Tournament"/>s by their start time and state.
    /// The unstarted/running tournaments have higher priority than finished (or closed) ones.
    /// Secondary newest finished/closed (first that ended) tournaments and oldest (first that will start)
    /// unstarted/running tournaments go first.
    /// </summary>
    /// <seealso cref="System.Collections.Generic.IComparer{FootballAIGame.Web.Models.Tournament}" />
    public class JoinedTournamentComparer : IComparer<Tournament>
    {
        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in the following table.Value Meaning Less than zero<paramref name="x" /> is less than <paramref name="y" />.Zero<paramref name="x" /> equals <paramref name="y" />.Greater than zero<paramref name="x" /> is greater than <paramref name="y" />.
        /// </returns>
        public int Compare(Tournament x, Tournament y)
        {
            if ((x.TournamentState == TournamentState.Unstarted || x.TournamentState == TournamentState.Running) &&
                (y.TournamentState != TournamentState.Unstarted && y.TournamentState != TournamentState.Running))
                return -1;
            if ((y.TournamentState == TournamentState.Unstarted || y.TournamentState == TournamentState.Running) &&
                (x.TournamentState != TournamentState.Unstarted && x.TournamentState != TournamentState.Running))
                return 1;
            if ((x.TournamentState == TournamentState.Unstarted || x.TournamentState == TournamentState.Running) &&
                (y.TournamentState == TournamentState.Unstarted || y.TournamentState == TournamentState.Running))
                return x.StartTime.CompareTo(y.StartTime); // oldest first (the first that will start / is running)

            // both are finished
            return -x.StartTime.CompareTo(y.StartTime); // newest first (the last that was finished)
        }
    }
}