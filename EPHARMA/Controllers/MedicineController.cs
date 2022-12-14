using EPHARMA.Data;
using EPHARMA.Models;
using EPHARMA.Services.Interface;
using EPHARMA.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class MedicineController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageInterface _imageService;
        private Medicine _medicine;
        private readonly UserManager<IdentityUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        public MedicineController(ApplicationDbContext Context, IImageInterface ImageService, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = Context;
            _imageService = ImageService;
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
                var AllMedicines = _context.Medicines.Include(x => x.Pharmacies).Where(x => x.Status);
                return View(AllMedicines);
            }
            else
            {
                var vendor = _context.Vendors.Where(v => v.VendorEmail == user.Email).FirstOrDefault();
                var pharma = _context.Pharmacies.Where(v => v.VendorId == vendor.VendorId).FirstOrDefault();
                var AllMedicines = _context.Medicines.Include(x => x.Pharmacies).Where(x => x.Status && x.PharmacyId == pharma.PharmacyId);
                return View(AllMedicines);
            }
           
        }
        [Authorize(Roles = "Admin,Vendor")]
        public async Task<IActionResult> Create(int id)
        {
            MedicineViewModel medicine_ViewModel = new MedicineViewModel();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            var role = await _userManager.GetRolesAsync(user);
            if (role.ElementAt(0) == "Admin")
            {
                ViewBag.PharmacyId = new SelectList(_context.Pharmacies.Where(x => x.Status), "PharmacyId", "PharmacyName");
            }
            else
            {
                var vendor = _context.Vendors.Where(v => v.VendorEmail == user.Email).FirstOrDefault();
                var pharma = _context.Pharmacies.Where(v => v.VendorId == vendor.VendorId).FirstOrDefault();
                ViewBag.PharmacyId = new SelectList(_context.Pharmacies.Where(x => x.Status && x.PharmacyId == pharma.PharmacyId), "PharmacyId", "PharmacyName");
            }
            ViewBag.AllCategories = new SelectList(_context.Categories.Where(x => x.Status && x.CategoryType=="Medicine"), "CategoryId", "CategoryTitle");
            if (id == 0)
            {
                medicine_ViewModel.Medicines = new Medicine();
                return View(medicine_ViewModel);
            }
            else
            {
                medicine_ViewModel.Medicines = _context.Medicines.Find(id);
                medicine_ViewModel.CategoryIds = _context.MedicineCategories.Where(x => x.MedicineId == id).Select(x => x.CategoryId).ToList();
                return View(medicine_ViewModel);
            }
        }
        [HttpPost]
        public IActionResult Create(MedicineViewModel NewMedicine, IFormFile MedicineImage)
        {
            string uniqueFileName;
            _medicine = NewMedicine.Medicines;

            if (_medicine.MedicineId == 0)
            {
                uniqueFileName = _imageService.AddImage(MedicineImage);
                NewMedicine.Medicines.MedicineImage = uniqueFileName;
                _context.Medicines.Add(_medicine);
                _context.SaveChanges();
                //Add All Medicine Category
                MedicineCategory obj;
                foreach (var category in NewMedicine.CategoryIds)
                {
                    obj = new MedicineCategory();
                    obj.CategoryId = category;
                    obj.MedicineId = _medicine.MedicineId;
                    _context.MedicineCategories.Add(obj);
                }
                _context.SaveChanges();
            }
            else //In Case of Edit 
            {
                _context.MedicineCategories.RemoveRange(_context.MedicineCategories.Where(x => x.MedicineId == NewMedicine.Medicines.MedicineId));
                _context.SaveChanges();

                MedicineCategory obj;
                foreach (var category in NewMedicine.CategoryIds)
                {
                    obj = new MedicineCategory();
                    obj.CategoryId = category;
                    obj.MedicineId = NewMedicine.Medicines.MedicineId;
                    _context.MedicineCategories.Add(obj);
                }
                _context.SaveChanges();

                if (MedicineImage != null) //Logo is edited(changed)
                {
                    _imageService.RemoveImage(NewMedicine.Medicines.MedicineImage);
                    uniqueFileName = _imageService.AddImage(MedicineImage);
                    _medicine.MedicineImage = uniqueFileName;
                }
                _context.Entry(_medicine).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var pharmacy = _context.Pharmacies.Where(x => x.PharmacyId == id);
            foreach (var item in pharmacy)
            {
                item.Status = false;
                _context.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            }
            var medicine = _context.Medicines.Find(id);
            if (medicine == null)
            {
                return NotFound();
            }
            medicine.Status = false;
            _context.Entry(medicine).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return RedirectToAction("Index");

        }
        public async Task<JsonResult> ChangeMedicineStatus(int id)
        {
            var doc = _context.Medicines.Find(id);
            doc.AvailabilityStatus = !doc.AvailabilityStatus;
            _context.SaveChanges();
            return Json("Success");
        }

    }
}
