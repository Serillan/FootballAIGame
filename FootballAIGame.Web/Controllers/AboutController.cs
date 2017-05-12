using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FootballAIGame.Web.Controllers
{
    /// <summary>
    /// The about section controller.
    /// </summary>
    /// <seealso cref="FootballAIGame.Web.Controllers.BaseController" />
    public class AboutController : BaseController
    {
        /// <summary>
        /// Returns the index view.
        /// </summary>
        /// <returns>The index view.</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Returns how to play view.
        /// </summary>
        /// <returns>The how to play view.</returns>
        [Route("about/how-to-play")]
        public ActionResult HowToPlay()
        {
            return View();
        }

        /// <summary>
        /// Returns the simulators view.
        /// </summary>
        /// <returns>The simulators view.</returns>
        public ActionResult Simulators()
        {
            return View();
        }

        /// <summary>
        /// Returns protocol view.
        /// </summary>
        /// <returns>The protocol view.</returns>
        public ActionResult Protocol()
        {
            return View();
        }

        /// <summary>
        /// Returns restriction view.
        /// </summary>
        /// <returns>The rules view.</returns>
        public ActionResult Restrictions()
        {
            return View();
        }

        /// <summary>
        /// Returns rules view.
        /// </summary>
        /// <returns>The rules view.</returns>
        public ActionResult Rules()
        {
            return View();
        }

        /// <summary>
        /// Returns the simulator save structure view.
        /// </summary>
        /// <returns>The simulator save structure view.</returns>
        [Route("about/simulator-save-structure")]
        public ActionResult SimulatorSaveStructure()
        {
            return View();
        }
    }
}