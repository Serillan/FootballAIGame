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

        // GET: matches
        public ActionResult Index()
        {
            var viewModel = _context.Matches
                .Include(m => m.Player1)
                .Include(m => m.Player2)
                .ToList();

            return View(viewModel);
        }

        // GET: matches/details/id
        public ActionResult Details(int id)
        {
            var match = _context.Matches
                .Include(m => m.Player1)
                .Include(m => m.Player2)
                .SingleOrDefault(m => m.Id == id);

            if (match == null)
                return HttpNotFound();

            return View("Details", match);
        }

        public ActionResult Errors(int id)
        {
            var match = _context.Matches
                .Include(m => m.Player1)
                .Include(m => m.Player2)
                .SingleOrDefault(m => m.Id == id);

            if (match == null)
                return HttpNotFound();

            return View("Errors", match);
        }

        public ActionResult Watch(int id)
        {
            var match = _context.Matches
                .Include(m => m.Player1)
                .Include(m => m.Player2)
                .SingleOrDefault(m => m.Id == id);

            if (match == null)
                return HttpNotFound();

            return View("Watch", match);
        }

    }
}