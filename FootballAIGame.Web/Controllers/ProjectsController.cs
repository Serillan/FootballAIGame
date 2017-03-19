using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FootballAIGame.Web.Controllers
{
    public class ProjectsController : Controller
    {
        /// <summary>
        /// Returns C# AI Project view.
        /// </summary>
        /// <returns>The C# AI Project view.</returns>
        public ActionResult CSharp()
        {
            return View();
        }

        /// <summary>
        /// Returns Java AI Project view.
        /// </summary>
        /// <returns>The Java AI Project view.</returns>
        public ActionResult Java()
        {
            return View();
        }

        /// <summary>
        /// Returns C# FSM AI Project view.
        /// </summary>
        /// <returns>The C# FSM AI Project view.</returns>
        public ActionResult CSharpFsm()
        {
            return View();
        }

        /// <summary>
        /// Returns Java FSM AI Project view.
        /// </summary>
        /// <returns>The Java FSM AI Project view.</returns>
        public ActionResult JavaFsm()
        {
            return View();
        }
    }
}