using EPHARMA.Data;
using EPHARMA.Models;
using EPHARMA.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPHARMA.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private Order _order;
        public OrderController(ApplicationDbContext Context)
        {
            _context = Context;
        }
        public IActionResult Index()
        {
            List<Order> AllOrders = _context.Orders.Include(x => x.Pharmacies).Include(x => x.Customers).Where(x => x.Status).ToList();      
            
            return View(AllOrders);
        }

        public IActionResult Details(int id)
        {
            OrderViewModel order = new OrderViewModel();
             order.Order = _context.Orders.Include(c => c.Customers).Where(a => a.OrderId == id).FirstOrDefault();
            order.OrderMedicine = _context.OrderMedicines.Include(m => m.Medicines).Where(a => a.OrderId == id).ToList();
            order.Prescription = _context.OrderPrescriptions.Where(a => a.OrderId == id).FirstOrDefault();
            return View(order);
        }
        public IActionResult Create(int id)
        {
            ViewBag.Pharmacy = new SelectList(_context.Pharmacies.Where(x => x.Status), "PharmacyId", "PharmacyName");
            ViewBag.Customer = new SelectList(_context.Customers.Where(x => x.Status), "CustomerId", "CustomerName");
            if (id == 0)
            {
                _order = new Order();
            }
            else
            {
                _order = _context.Orders.Find(id);
            }
            return View(_order);
        }
        [HttpPost]
        public IActionResult Create(Order NewOrder)
        {
            if (NewOrder.OrderId == 0)
            {
                _context.Orders.Add(NewOrder);
            }
            else
            {
                _context.Entry(NewOrder).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            }
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var order = _context.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }
            order.Status = false;
            _context.Entry(order).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return RedirectToAction("Index");

        }
    }
}
