using EPHARMA.Data;
using EPHARMA.Models;
using EPHARMA.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPHARMA.Controllers
{
    public class DoctorTimeTableController : Controller
    {
        private readonly ApplicationDbContext _context;
      //  private DoctorTimeTable _doctorTimeTable;
        public DoctorTimeTableController(ApplicationDbContext Context)
        {
            _context = Context;
        }
        public IActionResult Index()
        {
            
            var doctorTimetable = _context.DoctorTimeTables.Where(x => x.Status).GroupBy(x => x.DoctorId).ToList();

            return View(doctorTimetable);
        }
        public IActionResult Create(int id)
        {
            var docTT = _context.DoctorTimeTables.Where(a => a.DoctorId == id).FirstOrDefault();
            if (docTT != null)
            {
                string link = Request.Scheme + "://" + Request.Host + "/DoctorTimeTable/Edit/" + id;

                return Redirect(link);
            }
            ViewBag.DoctorId = new SelectList(_context.Doctors.Where(x => x.Status), "DoctorId", "DoctorName");
          
            return View();
        }
        [HttpPost]
        public IActionResult Create( DoctorTimeTableVm doctorTimeTable)
        {
            try
            {

                foreach (var timetable in doctorTimeTable.DoctorTimeTables)
                {
                    timetable.DoctorId = doctorTimeTable.DoctorID;
                    var timebtetween = (timetable.EndTime - timetable.StartTime).TotalMinutes;
                    timetable.NumberOfSlots = Convert.ToInt32(timebtetween) / 15;

                    List<string> intervals = new List<string>();

                    var startTemp = timetable.StartTime;
                    for (var i = 1; i <= timetable.NumberOfSlots; i++)
                    {
                        var endTime = timetable.StartTime.AddMinutes(15 * i);
                       
                        var TimeRange = startTemp.ToShortTimeString() + " - " + endTime.ToShortTimeString();
                        startTemp = endTime;
                        var doctorWeeklyTimeSheet = new DoctorWeeklyTimeSheet();
                        doctorWeeklyTimeSheet.Available = true;
                        doctorWeeklyTimeSheet.TimeRange = TimeRange;
                        doctorWeeklyTimeSheet.WeekDay = timetable.Day;
                        doctorWeeklyTimeSheet.DoctorId = timetable.DoctorId;
                        _context.DoctorWeeklyTimeSheets.Add(doctorWeeklyTimeSheet);
                    }
                    _context.SaveChanges();
                    _context.DoctorTimeTables.Add(timetable);
                    _context.SaveChanges();
                }
                string link = Request.Scheme + "://" + Request.Host + "/Doctor/Index";
                return Redirect(link);
            }
            catch(Exception e)
            {
                string link = Request.Scheme + "://" + Request.Host + "/Doctor/Index";
                return Redirect(link);
            }
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var doctor = _context.Doctors.Where(x => x.DoctorId == id);
            foreach (var item in doctor)
            {
                item.Status = false;
                _context.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            }
            var doctorTimetable = _context.DoctorTimeTables.Find(id);
            if (doctorTimetable == null)
            {
                return NotFound();
            }
            doctorTimetable.Status = false;
            _context.Entry(doctorTimetable).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
