using EPHARMA.Data;
using EPHARMA.Models;
using EPHARMA.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EPHARMA.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private Order _order;
        private readonly UserManager<IdentityUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        public OrderController(ApplicationDbContext Context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = Context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [Authorize(Roles = "Admin,Vendor")]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            var role = await _userManager.GetRolesAsync(user);
            if (role.ElementAt(0) == "Admin")
            {
                List<Order> AllOrders = _context.Orders.Include(x => x.Pharmacies).Include(x => x.Customers).Where(x => x.Status).ToList();
                return View(AllOrders);
            }
            else
            {
                var vendor = _context.Vendors.Where(v => v.VendorEmail == user.Email).FirstOrDefault();
                var pharma = _context.Pharmacies.Where(v => v.VendorId == vendor.VendorId).FirstOrDefault();
                List<Order> AllOrders = _context.Orders.Include(x => x.Pharmacies).Include(x => x.Customers).Where(x => x.Status && x.PharmacyId==pharma.PharmacyId).ToList();
                return View(AllOrders);
            }

           
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
            OrderViewModel orderViewModel = new OrderViewModel();
            orderViewModel.Order = _context.Orders.Include(x => x.Customers).Include(x => x.Pharmacies).Include(x => x.City).FirstOrDefault(x => x.OrderId == id);
            orderViewModel.Prescription = _context.OrderPrescriptions.FirstOrDefault(x => x.OrderId == id);
            orderViewModel.OrderMedicine = _context.OrderMedicines.Include(x => x.Medicines).Where(x => x.OrderId == id).ToList();
            //ViewBag.Customer = new SelectList(_context.Customers.Where(x => x.Status), "CustomerId", "CustomerName");
            if (id == 0)
            {
                orderViewModel.Order = new Order();
                return View(orderViewModel);
            }
            else
            {
                orderViewModel.Order = _context.Orders.Find(id);
                return View(orderViewModel);
            }

        }
        [HttpPost]
        public IActionResult Create(OrderViewModel orderViewModel)
        {
            if (orderViewModel.Order.OrderId == 0)
            {
                _context.Orders.Add(orderViewModel.Order);
                _context.SaveChanges();
            }
            else
            {
                var order = _context.Orders.FirstOrDefault(x => x.OrderId == orderViewModel.Order.OrderId);
                if (orderViewModel.Order.OrderStatus == "Pending")
                {
                    order.OrderStatus = "Pending";
                }
                else if (orderViewModel.Order.OrderStatus == "Active")
                {
                    order.OrderStatus = "Active";
                }
                else if (orderViewModel.Order.OrderStatus == "Rejected")
                {
                    order.Status = false;
                    order.OrderStatus = "Rejected";
                    var orderMedicineStatus = _context.OrderMedicines.Where(x => x.OrderId == order.OrderId).ToList();
                    foreach (var item in orderMedicineStatus)
                    {
                        item.Status = false;
                    }
                }
                else if (orderViewModel.Order.OrderStatus == "Completed")
                {
                    order.IsFinished = true;
                    order.OrderStatus = "Completed";
                    var orderMedicineStatus = _context.OrderMedicines.Where(x => x.OrderId == order.OrderId).ToList();
                    foreach (var item in orderMedicineStatus)
                    {
                        item.Status = false;
                    }
                }
                _context.Entry(order).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
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
