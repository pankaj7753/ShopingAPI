using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShopingAPI.DataLayer.Data;
using ShopingAPI.BusinessLayer.Repository;
using ShopingAPI.BusinessLayer.ViewModel;

namespace ShopingAPI.WebAPI.WebAPI
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class VendorController : CommanCtorController
    {
        public VendorController(ISingletonRepository _singleton, ILogger<CommanCtorController> logger) : base(_singleton, logger)
        {
        }
        [HttpPost]
        [Authorize(Roles =CustomRoles.Vendor)]
        public async Task<IActionResult> AddProducts(List<ProductViewModel> products)
        {
            string UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _singleton.vendorRepository.AddVendorProducts(products, UserId);
            return Ok();
        }

        [HttpPost]
        [Authorize(Roles = CustomRoles.Vendor)]

        public async Task<ActionResult<IEnumerable<VendorProductViewModel>>> GetProducts(SearchProductViewModel searchProduct)
        {
            string UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _singleton.vendorRepository.GetVendorProducts(UserId,searchProduct);
            return result;
        }

        [HttpPost]
        public async Task<ActionResult<bool>> UpdateProducts(List<VendorProductViewModel> vendorProducts)
        {
            string UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _singleton.vendorRepository.UpdateVendorProducts(vendorProducts, UserId);
            return result;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductAutoCompleteViewModel>>> GetSearchProductByName(string productName)
        {
            string UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _singleton.vendorRepository.SearchProductByName(productName,UserId);
            return result;
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<ProductAutoCompleteViewModel>>> GetSearchProductByName(SearchProductByNameCategoryIdSubCategoryId productName)
        {
            var result = await _singleton.vendorRepository.SearchProductByName(productName);
            return result;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ShopViewModel>>> GetVendorShops(SearchShops searchShops)
        {
            var result = await _singleton.vendorRepository.GetVendorShops(searchShops);
            return result;
        }

        [HttpPost]
        [Authorize(Roles =CustomRoles.AdminOrVendor)]
        public async Task<ActionResult<bool>> UpdateVendorShops(ShopViewModel shopViewModel)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _singleton.vendorRepository.UpdateVendorShops(shopViewModel, userId);
            return result;
        }



        [HttpGet]
        [Authorize(Roles = CustomRoles.AdminOrVendor)]
        public async Task<ActionResult<ShopViewModel>> GetVendorShopProfile()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _singleton.vendorRepository.GetVendorShopProfile(userId);
            return result;
        }

        [HttpGet]
        [Authorize(Roles = CustomRoles.Admin)]
        public async Task<ActionResult<ShopViewModel>> GetVendorShopByShopId(string shopId)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _singleton.vendorRepository.GetVendorShopByShopId(shopId);
            return result;
        }

        [HttpPost,DisableRequestSizeLimit]
        [Authorize(Roles =CustomRoles.AdminOrVendor)]
        public async Task<ActionResult<bool>> UpdateVendorShopImage()
        {
            try
            {
                IFormFile file = Request.Form.Files[0];
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var result = await _singleton.vendorRepository.UpdateVendorShopImage(file, userId);
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
           
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<VendorProductViewModel>>> GetProductByShopId(SearchShopProductViewModel searchProduct)
        {
            var result = await _singleton.vendorRepository.GetVendorProductByShopId(searchProduct);
            return result;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<VendorProductViewModel>>> GetProductById(List<ProductAddToCart> productAddToCarts)
        {

            var result = await _singleton.vendorRepository.GetVendorProductById(productAddToCarts);
            return result;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShopViewModel>>> GetFilterShopByNo(string shopNo)
        {
            var result = await _singleton.vendorRepository.FilterShopByNo(shopNo);
            return result;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CategoryViewModel>>> GetShopCategory(string shopId)
        {
            var result = await _singleton.vendorRepository.GetShopCategory(shopId);
            return result;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<BrandViewModel>>> GetShopBrands(string shopId)
        {
            var result = await _singleton.vendorRepository.GetShopBrands(shopId);
            return result;
        }

        [HttpGet]
        [Authorize(Roles = CustomRoles.AdminOrVendor)]
        public async Task<ActionResult<IEnumerable<BrandViewModel>>> GetProductBrands()
        {
            var result = await _singleton.vendorRepository.GetProductBrands();
            return result;
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShopViewModel>>> GetFilterShopByAddress(string city)
        {
            var result = await _singleton.vendorRepository.FilterShopByAddres(city);
            return result;
        }
    }
}
