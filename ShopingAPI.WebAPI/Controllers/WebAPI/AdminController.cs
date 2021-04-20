using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ShopingAPI.DataLayer.Models;
using ShopingAPI.BusinessLayer.Repository;
using ShopingAPI.BusinessLayer.ViewModel;

namespace ShopingAPI.WebAPI.WebAPI
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdminController : CommanCtorController
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private new readonly IConfiguration _configuration;
        public AdminController(ISingletonRepository singleton, ILogger<CommanCtorController> logger, UserManager<User> userManager, SignInManager<User> signInManager, IMapper mapper, IConfiguration config) : base(singleton, logger, mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = config;

        }
        [HttpGet]
        public async Task<IActionResult> GetUsersWithRoles()
        {

            var result = await _singleton.adminRepository.GetUsersWithRoles();
            return Ok(result);
        }


        [HttpPost("{userName}")]
        public async Task<IActionResult>  EditRoles(string userName, RoleEditViewModel  roleEdit)
        {
            string modifyBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userManager.FindByNameAsync(userName);
            var userRoles = await _userManager.GetRolesAsync(user);
            var selectedRoles = roleEdit.RoleNames;
            selectedRoles = selectedRoles ?? new string[] { };
            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded)
                return BadRequest("Faild to add to roles");
            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
            if (!result.Succeeded)
                return BadRequest("Faild to remove to roles");
            if (selectedRoles.Contains("Vendor"))
            {
              var shops= await _singleton.vendorRepository.AddVendorShopTableName(user, modifyBy);
            }
            return Ok(await _userManager.GetRolesAsync(user));
       }

    }
}
