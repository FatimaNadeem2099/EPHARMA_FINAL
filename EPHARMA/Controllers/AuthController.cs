using EPHARMA.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EPHARMA.Controllers
{
   
    
    public class AuthController : ControllerBase
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        //private readonly ILogger<LoginModel> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;
        //private readonly ApplicationSettings _appSettings;
        public AuthController(ApplicationDbContext db, SignInManager<IdentityUser> signInManager,
            //ILogger<LoginModel> logger,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _signInManager = signInManager;
            //_logger = logger;
            _userManager = userManager;
            this._roleManager = roleManager;
            //_appSettings = appSettings.Value;
        }
        //public IActionResult Index()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public async Task<IActionResult> Login(LoginDto loginDto)
        //{
        //    var user = await _userManager.FindByEmailAsync(loginDto.UserName);
        //    if (user != null)
        //    {
        //        var result = await _signInManager.PasswordSignInAsync(user.Email, loginDto.Password, true, false);
        //        //var result = await _signInManager.PasswordSignInAsync(loginDto.username, loginDto.password, true, lockoutOnFailure: false);
        //        if (result.Succeeded)
        //        {

        //            _logger.LogInformation("User logged in.");

        //            var current_User = _userManager.GetUserAsync(HttpContext.User);
        //            var roles = await _userManager.GetRolesAsync(user);
        //            var userId = "" + current_User.Id;
        //            HttpContext.Session.SetString("id", userId);
        //            HttpContext.Session.SetString("username", loginDto.UserName);

        //            //await _roleManager.
        //            //Role is not added in session yet
        //            HttpContext.Session.SetString("roles", roles.FirstOrDefault());
        //            //return Ok(new {username=loginDto.username,role=roles.FirstOrDefault() });
        //            //if (roles[0] == "Restaurant")
        //            return Redirect(("http://foodybirdsangular3.s3-website-ap-southeast-1.amazonaws.com/login/" + loginDto.UserName + "/" + roles.FirstOrDefault()));
        //            return Redirect(("http://localhost:4200/login/" + loginDto.UserName + "/" + roles.FirstOrDefault()));
        //            //else if (roles[0] == "RestaurantKitchen")
        //            //    return Redirect("http://localhost:4200/Restaurant/Kitchen");
        //            //else
        //            //    return Redirect("http://localhost:4200/CRM");
        //        }
        //        else
        //        {
        //            ViewBag.Result = result.ToString();
        //        }
        //    }
        //    return Redirect("~/getapi/login1");
        //}

        //[HttpPost]
        //public async Task<IActionResult> Login1(LoginDto loginDto)
        //{
        //    var user = await _userManager.FindByEmailAsync(loginDto.UserName);

        //    var result = await _signInManager.PasswordSignInAsync(user.Email, loginDto.Password, true, false);
        //    //var result = await _signInManager.PasswordSignInAsync(loginDto.username, loginDto.password, true, lockoutOnFailure: false);
        //    if (result.Succeeded)
        //    {

        //        _logger.LogInformation("User logged in.");

        //        var current_User = _userManager.GetUserAsync(HttpContext.User);
        //        var roles = await _userManager.GetRolesAsync(user);
        //        var userId = "" + current_User.Id;
        //        HttpContext.Session.SetString("id", userId);
        //        HttpContext.Session.SetString("username", loginDto.UserName);

        //        //await _roleManager.
        //        //Role is not added in session yet
        //        HttpContext.Session.SetString("roles", roles.FirstOrDefault());
        //        IdentityOptions _options = new IdentityOptions();

        //        var token = getToken(userId.ToString(), (loginDto.UserName).ToString(), roles.FirstOrDefault().ToString());


        //        // OLD TOKEN CODE
        //        //var tokenDescriptor = new SecurityTokenDescriptor
        //        //{
        //        //    Subject = new ClaimsIdentity(new Claim[]
        //        //    {
        //        //        new Claim("UserID",user.Id.ToString()),
        //        //        new Claim(_options.ClaimsIdentity.RoleClaimType,roles.FirstOrDefault())
        //        //    }),
        //        //    Expires = DateTime.UtcNow.AddDays(1),
        //        //    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWT_Secret)), SecurityAlgorithms.HmacSha256Signature)
        //        //};
        //        //var tokenHandler = new JwtSecurityTokenHandler();
        //        //var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        //        //var token = tokenHandler.WriteToken(securityToken);
        //        return Redirect(("http://localhost:4200/login/" + loginDto.UserName + "/" + roles.FirstOrDefault()));
        //        //return Ok(new { token });
        //    }
        //    else
        //    {
        //        ViewBag.Result = result.ToString();
        //    }
        //    return Redirect("~/getapi/login1");
        //}

        [HttpPost]
        public async Task<Object> LoginCustomer(IFormCollection formData)
        {
            try
            {
                var loginDto = JsonConvert.DeserializeObject<LoginDto>(formData["Credentials"]);
                var user = await _userManager.FindByEmailAsync(loginDto.userName);

                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user.Email, loginDto.password, true, false);
                    if (result.Succeeded)
                    {

                        //_logger.LogInformation("User logged in.");

                        var current_User = _userManager.GetUserAsync(HttpContext.User);
                        var roles = await _userManager.GetRolesAsync(user);
                        var userId = user.Id;
                        //HttpContext.Session.SetString("id", userId);
                        //HttpContext.Session.SetString("username", loginDto.UserName);
                        //HttpContext.Session.SetString("roles", roles.FirstOrDefault());
                        if (roles.FirstOrDefault() != "Customer")
                        {
                            return BadRequest();
                        }
                        var customer = _db.Customers.Where(a => a.CustomerEmail == user.Email).FirstOrDefault();
                        var token = getToken(userId.ToString(),customer.CustomerId.ToString(),customer.UserName, (loginDto.userName).ToString(), roles.FirstOrDefault().ToString());

                        return token;
                    }
                    return BadRequest();
                }
                return BadRequest();
            }
            catch(Exception e)
            {
                return BadRequest();
            }
        }

     
      


        //[NonAction]
        public Object getToken(string userId, string cusid, string username, string email, string role)
        {
            string key = "foody_bird_restaurants";
            var issuer = "http://foody_bird.com";
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var permClaims = new List<Claim>();
            permClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

            permClaims.Add(new Claim("userid", userId));
            permClaims.Add(new Claim("name", username));
            permClaims.Add(new Claim("username", email));
            permClaims.Add(new Claim("role", role));
            permClaims.Add(new Claim("customerid", cusid));

            var token = new JwtSecurityToken(issuer,
                            issuer,
                            permClaims,
                            expires: DateTime.Now.AddDays(1),
                            signingCredentials: credentials);
            var jwt_token = new JwtSecurityTokenHandler().WriteToken(token);

            return new { token = jwt_token };
        }







    }

}
