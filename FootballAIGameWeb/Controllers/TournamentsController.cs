using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FootballAIGameWeb.Models;

namespace FootballAIGameWeb.Controllers
{
    public class TournamentsController : Controller
    {
        private ApplicationDbContext _context;

        public TournamentsController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        // GET: tournaments
        public ActionResult Index()
        {
            return View();
        }

        // GET: tournaments/details/id
        public ActionResult Details(int id)
        {
            var tournament = _context.Tournaments.SingleOrDefault(t => t.Id == id);

            if (tournament == null)
                return HttpNotFound();

            return View("Details", tournament);
        }
    }
}