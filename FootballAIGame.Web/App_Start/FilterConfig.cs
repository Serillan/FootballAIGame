using System.Web;
using System.Web.Mvc;

namespace FootballAIGame.Web
{
    /// <summary>
    /// Provides the method to register global filters.
    /// </summary>
    public class FilterConfig
    {
        /// <summary>
        /// Registers the global filters. With global filters we can add attributes globally to all controllers actions.
        /// </summary>
        /// <param name="filters">The filters.</param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
