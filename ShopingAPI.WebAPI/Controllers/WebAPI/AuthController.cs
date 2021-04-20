using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ShopingAPI.BusinessLayer.ViewModel;
using ShopingAPI.DataLayer.Data;
using Microsoft.EntityFrameworkCore;
using ShopingAPI.DataLayer;
using ShopingAPI.BusinessLayer.Helpers;
using Microsoft.Extensions.DependencyInjection;
using ShopingAPI.BusinessLayer.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using ShopingAPI.DataLayer.Models;

namespace ShopingAPI.WebAPI.WebAPI
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : CommanCtorController
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private new readonly IConfiguration _configuration;

        public AuthController(ISingletonRepository singleton, ILogger<CommanCtorController> logger,UserManager<User> userManager,SignInManager<User> signInManager,IMapper mapper, IConfiguration config) :base(singleton,logger,mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = config;

        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(UserForLoginViewModel userForLogig)
        {
            userForLogig.UserName = userForLogig.UserName.ToLower();
            var user = await _userManager.FindByNameAsync(userForLogig.UserName.ToLower());
            if (user == null)
            {
                return Unauthorized("User dose not exist.");
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, userForLogig.Password,false);

            if (result.Succeeded)
            {
                var appUser = _mapper.Map<UserForListDto>(user);
                return Ok(new
                {
                    token = GenerateJwtToken(user),
                    user = appUser
                });
            }
             return Unauthorized("Invalid username or password");
            
            //HttpContext.Session.SetString("UserId", sessionUser.UserId);
            //HttpContext.Session.SetComplexData("SessionUser", sessionUser);
            //var sessionUserGet =HttpContext.Session.GetComplexData<SessionUser>("SessionUser");


            //   var appSettingsSection = _configuration.GetSection("AppSettings:Secret");
            // _services.Configure<AppSettings>(appSettingsSection);
            //var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            // var appSettings = appSettingsSection.Get<AppSettings>();
            
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Name,user.FullName),
                // new Claim(ClaimTypes.Email,user.UserName),
                //new Claim(ClaimTypes.UserData,user.TableName),
            };

            var roles = _userManager.GetRolesAsync(user).Result;

            #region  deletion or updation of new role for selected user
            //var result = _userManager.RemoveFromRolesAsync(user, roles).Result;
            //var resultRole =   _userManager.AddToRolesAsync(user, new[] { "User", "Vendor", "Admin", "VendroEmployee" }).Result;//nokout
            #endregion

            foreach (var item in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, item));
            }
            string userTable = user.Id.Replace("-", "");
            var appSecret = _configuration.GetSection("AppSettings").GetSection("Secret").Value;

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(appSecret));
            var cards = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = cards
            };
            var tokenHeader = new JwtSecurityTokenHandler();
            var token = tokenHeader.CreateToken(tokenDescriptor);

            return tokenHeader.WriteToken(token);
        }
        

        
        [AllowAnonymous]

        [HttpPost]
        public async Task<IActionResult> Register(UserForRegisterViewModel userForRegister)
        {
            userForRegister.UserName = userForRegister.UserName.ToLower();
            var userToCreate = _mapper.Map<User>(userForRegister);
            userToCreate.Id = Guid.NewGuid().ToString();
            userToCreate.TableName = await _singleton.userRepository.GetUserOrdersTableOnRegister(userToCreate.Id);
            var result = await _userManager.CreateAsync(userToCreate, userForRegister.Password);
            var userToReurn = _mapper.Map<UserForListDto>(userToCreate);
            string tblName = string.Empty;
            if(result.Succeeded)
            {
                ///await _singleton.userRepository.CreateUserOrdersTable(userToCreate.Id);
                var resultRole = _userManager.AddToRoleAsync(userToCreate, "User").Result;

                //await signInManager.SignInAsync(user, isPersistent: false);

                return StatusCode(201);

            }
            return BadRequest("User already exists");
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Find the user by email
                var user = await _userManager.FindByNameAsync(model.Email);
                // If the user is found AND Email is confirmed
                //if (user != null && await _userManager.IsEmailConfirmedAsync(user))
                if (user != null)
                {
                    // Generate the reset password token
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    // Build the password reset link
                    var passwordResetLink = Url.Action("ResetPassword", "Account",
                            new { email = model.Email, token = token }, Request.Scheme);

                    return Ok(new ResetPasswordViewModel
                    {
                        Email=model.Email,
                        Token=token
                    });


                    // Log the password reset link
                    // logger.Log(LogLevel.Warning, passwordResetLink);

                    // Send the user to Forgot Password Confirmation view
                    //return View("ForgotPasswordConfirmation");
                }
                return BadRequest(ModelState);
            }
            return BadRequest(ModelState);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Find the user by email
                var user = await _userManager.FindByNameAsync(model.Email);

                if (user != null)
                {
                    // reset the user password
                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        return Ok();
                    }
                    // Display validation errors. For example, password reset token already
                    // used to change the password or password complexity rules not met
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return BadRequest(ModelState);
                }

                // To avoid account enumeration and brute force attacks, don't
                // reveal that the user does not exist
                return BadRequest(ModelState);
            }
            // Display validation errors if model state is not valid
            return BadRequest(ModelState);
        }
    }
}