using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FootballAIGameWeb.Models;

namespace FootballAIGameWeb.Controllers
{
    public class MatchesController : Controller
    {
        private ApplicationDbContext _context;

        public MatchesController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        // GET: players
        public ActionResult Index()
        {
            var viewModel = _context.Matches
                .Include(m => m.Player1)
                .Include(m => m.Player2)
                .ToList();

            return View(viewModel);
        }

        // GET: players/details/id
        public ActionResult Details(int id)
        {
            var matches = _context.Matches.SingleOrDefault(m => m.Id == id);

            if (matches == null)
                return HttpNotFound();

            return View("Details", matches);
        }

        public ActionResult Watch(int id)
        {
            var matches = _context.Matches.SingleOrDefault(m => m.Id == id);

            if (matches == null)
                return HttpNotFound();

            return View("Watch", matches);
        }

    }
}