using System;
using Microsoft.AspNetCore.Mvc;
using cvicenie_mvc.Models;

namespace cvicenie_mvc.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly Repository repository = new Repository();

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Read()
        {
            return View(repository.GetAllAttendance());
        }

        public IActionResult Process()
        {
            repository.ProcessAndCalculateStats();
            return RedirectToAction("Stats");
        }

        public IActionResult Stats()
        {
            return View(repository.GetAttendanceStats());
        }

        public IActionResult Visualization()
        {
            return View(repository.GetAttendanceStats());
        }
    }
}