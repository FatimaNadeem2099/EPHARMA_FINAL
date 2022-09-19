using EPHARMA.Data;
using EPHARMA.Models;
using EPHARMA.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPHARMA.Controllers
{

    public class UserWebAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IImageInterface _imageService;
        public UserWebAPIController(ApplicationDbContext db, UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IImageInterface ImageService)
        {
            _db = db;
            _userManager = userManager;
            this._roleManager = roleManager;
            _imageService = ImageService;
        }


        [HttpGet]

        public async Task<object> GetCities()
        {
            var data = _db.Cities.Where(c => c.Status).Select(
                r => new {
                    CityId = r.CityId,
                    CityName = r.CityName,
                    Status = r.Status,
                    Date = r.Date,
                  
                }).ToList();
            return Ok(data);
        }


        public async Task<IActionResult> GetBestRatedPharmacies()
        {
            try
            {
                var bestRatedpharmacies = _db.Pharmacies.Include(c => c.Cities).Where(a => a.Status== true && a.OnlineStatus == true && a.IsBestRated== true).Select(brr => new
                {
                    brr.PharmacyId,
                    pharmacyName = brr.PharmacyName,
                    image = brr.CoverImage,
                    pharmacyCity = brr.Cities.CityName
                }).ToList();
                return Ok(bestRatedpharmacies);

            }
            catch (Exception e)
            {
                ;
                return BadRequest("Error : " + e);
            }
        }


        public async Task<IActionResult> GetPharmaciesData()
        {
            try
            {
                var pharmacies = _db.Pharmacies.Include(c => c.Cities).Where(a => a.Status == true && a.OnlineStatus == true).Select(p => new
                {
                    p.PharmacyId,
                    p.PharmacyName,
                    p.CoverImage,
                    p.Cities.CityName,
                    p.PharmacyAddress,
                    MedicineCount=_db.Medicines.Where(a => a.AvailabilityStatus && a.PharmacyId == p.PharmacyId).Count()
                }).Take(5).ToList(); 
               
            
                return Ok(pharmacies);
            }
            catch (Exception e)
            {
                return BadRequest("Message" + e);
            }
        }


        public async Task<IActionResult> GetAllPharmacies()
        {
            try
            {
                var pharmacies = _db.Pharmacies.Include(c => c.Cities).Where(a => a.Status == true && a.OnlineStatus == true).Select(p => new
                {
                    p.PharmacyId,
                    p.PharmacyName,
                    p.CoverImage,
                    p.Cities.CityName,
                    p.PharmacyAddress,
                    MedicineCount = _db.Medicines.Where(a => a.AvailabilityStatus && a.PharmacyId == p.PharmacyId).Count()
                }).Take(5).ToList();


                return Ok(pharmacies);
            }
            catch (Exception e)
            {
                return BadRequest("Message" + e);
            }
        }

        [NonAction]
        public static List<object> getMedicineCategories(List<MedicineCategory> MedicineCategories)
        {

                List<object> cat = new List<object>();
            foreach(var med in MedicineCategories)
            {
                var obj = new {
                    CategoryId= med.CategoryId,
            MedicineCategoryId = med.MedicineCategoryId,
            MedicineName = med.Categories.CategoryTitle
            };
                cat.Add(obj);
            }
            return (cat);
        }
         
        public async Task<IActionResult> GetMedicinesOfPharmacy(int id)
        {
            try
            {
                var categories = _db.MedicineCategories.Include(c=> c.Categories).Where(a => a.Medicines.PharmacyId == id).Select(med => new{

                    CategoryId = med.CategoryId,
                    MedicineCategoryId = med.MedicineCategoryId,
                    CategoryTitle = med.Categories.CategoryTitle
                }).ToList();

                var  pharmacy = _db.Pharmacies.Include(v => v.Vendors).Include(c => c.Cities).Where(a => a.PharmacyId == id).Select(p=>new
                {
                    PharmacyId= p.PharmacyId,
                    CityId= p.CityId,      
                    CityName= p.Cities.CityName,
                    PharmacyName= p.PharmacyName,
                    DeliveryCharges = p.DeliveryCharges,
                    OnlineStatus = p.OnlineStatus,
        PharmacyAddress= p.PharmacyAddress,
        CoverImage= p.CoverImage,
        PharmacyLogo= p.PharmacyLogo,
        Overview= p.Overview,
                }).FirstOrDefault();
                var medicines = _db.Medicines.Include(c => c.Pharmacies).Where(a => a.Status == true && a.PharmacyId == id).Select(p => new
                {
                    MedicineId = p.MedicineId,
                    PharmacyId = p.PharmacyId,
                    MedicineName = p.MedicineName,
                    Storage = p.Storage,
                    PackageSize = p.PackageSize,
                    KeyBenefits = p.KeyBenefits,
                    Precautions = p.Precautions,
                    IsPrescribed= p.MedicineType=="otcdrug"?false:true,
                    AvailabilityStatus = p.AvailabilityStatus,
                    MedicineType = p.MedicineType == "otcdrug" ? "Over the Counter Drug" : "Prescription Drug ",
                    MedicineBrand = p.MedicineBrand,
                    MedicinePrice = p.MedicinePrice,
                    MedicineImage = p.MedicineImage,
                    MedicineDescription = p.MedicineDescription,
                    MedicineIngredients = p.MedicineIngredients,
                    Quantity =p.Quantity,
                    MedicineCategories = getMedicineCategories(_db.MedicineCategories.Include(c => c.Categories).Where(a => a.MedicineId == p.MedicineId).ToList())
                }).ToList();

                var obj = new
                {
                    PharmacyInfo= pharmacy,
                    PharmacyCategories=  categories.GroupBy(x => x.CategoryTitle).Select(y => y.FirstOrDefault()),
                    TrendingMedicines = medicines,
                    Medicines= medicines,
                };
                return Ok(obj);
            }
            catch (Exception e)
            {
                return BadRequest("Message" + e);
            }
        }

        public async Task<bool> MakeIdentity(string Email, string Password, string Role)
        {
            try
            {
                if (!await _roleManager.RoleExistsAsync(Role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(Role));
                }
                var user = new IdentityUser { UserName = Email, Email = Email, EmailConfirmed = true };
                var result = await _userManager.CreateAsync(user, Password);
                if (result.Succeeded)
                {
                    //_logger.LogInformation("User created a new account with password.");
                    try
                    {
                        await _userManager.AddToRoleAsync(user, Role);

                    }
                    catch (Exception e)
                    {
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }

            return false;
        }

        [HttpPost]
        public async Task<Object> CreateCustomer(IFormCollection form)
        {
            try
            {
                var obj = JsonConvert.DeserializeObject<Customer>(form["Customer"]);
                if (await MakeIdentity(obj.CustomerEmail, obj.Password, "Customer"))
                {
                  
                    await _db.Customers.AddAsync(obj);                   
                    await _db.SaveChangesAsync();
                    return ("Success");
                }
                return ("Error");
            }
            catch (Exception e)
            {
                return ("Error, Something went wrong \n" + e);
            }
        }
        [HttpGet]
        public async Task<object> GetCustomerById(int id)
        {
            var data = _db.Customers.Include(c => c.Cities).Where(i => i.CustomerId==id).FirstOrDefault();
            return Ok(data);
        }


        [HttpPost]
        public async Task<Object> CreateOrder(IFormCollection form)
        {
            try
            {
                var order = JsonConvert.DeserializeObject<Order>(form["Order"]);
                var orderMedicines = JsonConvert.DeserializeObject<OrderMedicine[]>(form["OrderMedicines"]);
                if (order != null) { 
                    order.OrderStatus = "Pending";
                order.RecievingTime = DateTime.Now.TimeOfDay.ToString();
                order.OrderCode = "ORDR" + order.Date.ToString();
                _db.Orders.Add(order);
                _db.SaveChanges();
                foreach (var med in orderMedicines)
                {
                    med.OrderId = order.OrderId;
                        _db.OrderMedicines.Add(med);

                        
                }
                    _db.SaveChanges();
                    List<OrderPrescription> orderPrescriptions = new List<OrderPrescription>();
                if (Request.Form.Files.Count() > 0)
                {
                    var prescriptionImageFile = Request.Form.Files[0];

                    if (prescriptionImageFile != null)
                    {
                            var medicines = _db.Medicines.Where(a => a.PharmacyId== order.PharmacyId).ToList();
                        foreach (var med in orderMedicines)
                            {
                                var medicine = medicines.Where(a => a.MedicineId==med.MedicineId).FirstOrDefault();

                                if(medicine.MedicineType== "prescriptiondrug")
                                {
                                    OrderPrescription orderPrescription = new OrderPrescription();
                                    orderPrescription.MedicineId = medicine.MedicineId;
                                    orderPrescription.OrderId = order.OrderId;
                                    orderPrescription.Image = _imageService.AddImage(prescriptionImageFile);
                                    orderPrescriptions.Add(orderPrescription);
                                }
                            }
                            _db.OrderPrescriptions.AddRange(orderPrescriptions);
                            _db.SaveChanges();
                        }
                }
            }
                        return ("Error");
            }
            catch (Exception e)
            {
                return ("Error, Something went wrong \n" + e);
            }
        }


        [HttpGet]
        public async Task<object> GetDoctorsForMainScreen()
        {
            var data = _db.Doctors.Include(c => c.Categories).Select(e => new {
            e.DoctorId,
            e.DoctorName,
            e.DoctorEmail,
            e.YearsOfExperience,
            e.OnlineStatus,
            e.ProfilePhoto,
            e.PhoneNumber,
            e.Description,
            e.Services,
            e.CNIC,
            e.Education,
            e.ConsultancyCharges,
                e.Categories.CategoryId,
            e.Categories.CategoryTitle,
            }).ToList();
            return Ok(data);
        }

        [HttpGet]
        public async Task<object> GetDoctorDetails(int id)
        {
            var data = _db.Doctors.Include(c => c.Categories).Where(d => d.DoctorId==id).Select(e => new {
                e.DoctorId,
                e.DoctorName,
                e.DoctorEmail,
                e.YearsOfExperience,
                e.OnlineStatus,
                e.ProfilePhoto,
                e.PhoneNumber,
                e.Description,
                e.Services,
                e.CNIC,
                e.Education,
                e.ConsultancyCharges,
                e.Categories.CategoryId,
                e.Categories.CategoryTitle,
            }).FirstOrDefault();
            var timesheet = _db.DoctorWeeklyTimeSheets.Where(a => a.DoctorId == id).ToList();
            var obj = new {
                Doctor = data,
            Timesheet = timesheet
           };
            return Ok(obj);
        }
        [HttpGet]
        public async Task<object> GetAllDocterCategories()
        {
            var data = _db.Categories.Where(c => c.CategoryType== "Doctor").Select(e => new {
                e.CategoryId,
                e.CategoryTitle,
            }).ToList();
            return Ok(data);
        }

        [HttpPost]
        public async Task<Object> CreateAppointment(IFormCollection form)
        {
            try
            {
                var Appointment = JsonConvert.DeserializeObject<Appointment>(form["Appointment"]);
                var AppointmentBilling = JsonConvert.DeserializeObject<AppointmentBilling>(form["AppointmentBilling"]);
                if (Appointment != null)
                {
                    Appointment.AppointmentCode = "APPOINTMENT" + Appointment.AppointmentDate.ToString();
                    _db.Appointments.Add(Appointment);
                    _db.SaveChanges();
                    AppointmentBilling.AppointmentId = Appointment.AppointmentId;
                    _db.AppointmentBillings.Add(AppointmentBilling);
                    _db.SaveChanges();
                    return ("success");
                }
                return ("Error");
            }
            catch (Exception e)
            {
                return ("Error, Something went wrong \n" + e);
            }
        }


        public async Task<IActionResult> GetCustomerAppointments(int id)
        {
            try
            {
                var appointments = _db.Appointments.Include(c => c.Doctors).ThenInclude(d => d.Categories).Where(a => a.CustomerId == id).Select(a => new
                {
                    a.AppointmentId,
                    a.CustomerId,
                    a.DoctorId,
                    a.AppointmentCode,
                    a.AppointmentDay,
                    a.AppointmentStatus,
                    a.IsFinished,
                    a.AppointmentDate,
                    a.StartTime,
                    a.EndTime,
                    a.NumberOfSlots,
                    a.HasPrescription,
                    a.Prescription,
                    DoctorName = a.Doctors.DoctorName,
                    DoctorCategory = a.Doctors.Categories.CategoryTitle,
                    AppointmentBilling = _db.AppointmentBillings.Where(z => z.AppointmentId == a.AppointmentId).FirstOrDefault()
                }).ToList();


                return Ok(appointments);
            }
            catch (Exception e)
            {
                return BadRequest("Message" + e);
            }
        }


    }
}
