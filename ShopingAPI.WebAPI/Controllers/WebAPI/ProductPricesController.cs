using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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

namespace ShopingAPI.WebAPI.WebAPI
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductPricesController : CommanCtorController
    {
       

        public ProductPricesController(DataContext context, IMapper mapper, ILogger<CommanCtorController> logger) :base(context,mapper,logger)
        {

        }

        // GET: api/ProductPrices
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductViewModel>>> GetProductPrices()
        {
            var result = await _context.ProductPrices.Include(p => p.Product).Include(p => p.Unit).ToListAsync();
            IEnumerable<ProductViewModel> categoryViewModels =  _mapper.Map<IEnumerable<ProductViewModel>>(result);

            var result2 = _context.ProductPrices.ToListAsync();
            return  categoryViewModels.ToList();
        }
        // GET: api/ProductPrices
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductViewModel>>> GetProductPricesByCategoryId(string CategoryId)
        {
            var result = await _context.ProductPrices.Where(x=>x.Product.CategoryId== CategoryId && x.IsActive==true).Include(p => p.Product).Include(p => p.Unit).ToListAsync();
            IEnumerable<ProductViewModel> categoryViewModels = _mapper.Map<IEnumerable<ProductViewModel>>(result);

            var result2 = _context.ProductPrices.ToListAsync();
            return categoryViewModels.ToList();
        }


        // GET: api/ProductPrices/5
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<IEnumerable<CartViewModel>>> GetProductCarts(CartIds[] cartIds)
        {
            List<string> ppid = new List<string>();
            ppid = cartIds.Select(x => x.id).ToList();
                        
            var result = await _context.ProductPrices.Where(x => ppid.Contains(x.Id) && x.IsActive==true).Include(p => p.Product).Include(p => p.Unit).ToListAsync();
            List<CartViewModel> cartViewModellist = new List<CartViewModel>();
            foreach (var item in result)
            {
                var quantity = cartIds.Where(x => x.id == item.Id).FirstOrDefault();
                CartViewModel cartViewModel1 = new CartViewModel
                {
                    Description=item.Product.Description,
                    DiscountPercentage=item.DiscountPercentage,
                    ImageUrl=item.Product.ImageUrl,
                    MRPPrice=item.MRPPrice,
                    ProductId=item.ProductId,
                    ProductName=item.Product.ProductName,
                    ProductPriceId=item.Id,
                    Quantity=quantity.count,
                    Total=item.SellingPrice* Convert.ToInt32(quantity.count),
                    SellingPrice=item.SellingPrice,
                    UnitType=item.Unit.UnitType,
                    UserId= User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            };

                cartViewModellist.Add(cartViewModel1);
            }


            if (cartViewModellist == null)
            {
                return NotFound();
            }

            return cartViewModellist.ToList();
        }
    }
}
