using EPHARMA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EPHARMA.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
           /* DateTime currentDate = DateTime.Now;
            var DateList = new List<KeyValuePair<string, DateTime>>();
            for(int i=1; i < 8; i++)
            {
                var newdate = currentDate.AddDays(i).Date;
                var newentry = new KeyValuePair<string, DateTime>(newdate.DayOfWeek.ToString(), newdate);
                DateList.Add(newentry);
            }*/
            /*     List<string> intervals = new List<string>();
                 var v = ((DateTime.Now.AddHours(2) - DateTime.Now).TotalMinutes) / 15;
                 var start = DateTime.Now;
                 var startTemp = DateTime.Now;
                 for (var i = 1; i <= 8; i++)
                 {
                     var endTime = DateTime.Now.AddMinutes(15 * i);
                     var a = startTemp.ToShortTimeString() + " - " + endTime.ToShortTimeString();
                     startTemp = endTime;
                 }*/
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
