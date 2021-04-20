using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShopingAPI.DataLayer.Data;
using ShopingAPI.DataLayer;
using ShopingAPI.BusinessLayer.ViewModel;
using ShopingAPI.DataLayer.Models;

namespace ShopingAPI.WebAPI.WebAPI
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : CommanCtorController
    {

        public CategoriesController(DataContext context, IMapper mapper, ILogger<CommanCtorController> logger) : base(context, mapper, logger)
        {
        }
        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryViewModel>>> GetCategories()
        {
            IEnumerable<Category> categories  = await  _context.Categories.Where(f => f.IsActive == true).ToListAsync();
            IEnumerable<CategoryViewModel> categoryViewModels = _mapper.Map<IEnumerable<CategoryViewModel>>(categories);
            return  categoryViewModels.ToList();
        }

    }
}
