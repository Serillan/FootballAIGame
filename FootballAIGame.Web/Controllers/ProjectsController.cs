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

    }
}