﻿using EPHARMA.Data;
using EPHARMA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPHARMA.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private Appointment _appointment;
        public AppointmentController(ApplicationDbContext Context)
        {
            _context = Context;
        }
        public IActionResult Index()
        {
            var AllAppointments = _context.Appointments.Include(d => d.Doctors).Where(x => x.Status);
            return View(AllAppointments);
        }

        public IActionResult Create(int id)
        {
            ViewBag.CustomerId = new SelectList(_context.Customers.Where(x => x.Status), "CustomerId", "CustomerName");
            ViewBag.DoctorId = new SelectList(_context.Doctors.Where(x => x.Status), "DoctorId", "DoctorName");
            if (id == 0)
            {
                _appointment = new Appointment();
            }
            else
            {
                _appointment = _context.Appointments.Find(id);
            }
            return View(_appointment);
        }
        [HttpPost]
        public IActionResult Create(Appointment NewAppointment)
        {
            if (NewAppointment.AppointmentId == 0)
            {
                _context.Appointments.Add(NewAppointment);
            }
            else
            {
                _context.Entry(NewAppointment).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            }
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        public IActionResult CreatePrescription(int id)
        {
            ViewBag.CustomerId = new SelectList(_context.Customers.Where(x => x.Status), "CustomerId", "CustomerName");
            ViewBag.DoctorId = new SelectList(_context.Doctors.Where(x => x.Status), "DoctorId", "DoctorName");
            if (id == 0)
            {
                _appointment = new Appointment();
            }
            else
            {
                _appointment = _context.Appointments.Include(c => c.Customers).Include(d => d.Doctors).Where(d => d.AppointmentId == id).FirstOrDefault();
            }
         //   ViewBag.Editortext = "<p><strong>Patient Name:&nbsp;</strong>"+_appointment.Customers.CustomerName+"</p><p><strong> Patient Age: &nbsp;</strong>"+ _appointment.Customers.Age+"</p><p><strong> Date:&nbsp;</strong>"+DateTime.Now.Date.ToShortDateString()+"</p><p><strong> Doctor:&nbsp;</strong>"+_appointment.Doctors.DoctorName+"</p><p> &nbsp;</p> ";
            return View(_appointment);
        }
        [HttpPost]
        public IActionResult CreatePrescription(int id, string editortext)
        {
            _appointment = _context.Appointments.Include(c => c.Customers).Include(d => d.Doctors).Where(d => d.AppointmentId == id).FirstOrDefault();
           var PreText = "<p><strong>Patient Name:&nbsp;</strong>" + _appointment.Customers.CustomerName + "</p><p><strong> Patient Age: &nbsp;</strong>" + _appointment.Customers.Age + "</p><p><strong> Date:&nbsp;</strong>" + DateTime.Now.Date.ToShortDateString() + "</p><p><strong> Doctor:&nbsp;</strong>" + _appointment.Doctors.DoctorName + "</p><p> &nbsp;</p> ";
            _appointment.HasPrescription = true;
            _appointment.Prescription = PreText + editortext;
            _context.Appointments.Update(_appointment);
            _context.SaveChanges();
            return RedirectToAction("Index");
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
            var appointment = _context.Appointments.Find(id);
            if (appointment == null)
            {
                return NotFound();
            }
            appointment.Status = false;
            _context.Entry(appointment).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return RedirectToAction("Index");

        }
    }
}
