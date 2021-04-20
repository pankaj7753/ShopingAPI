using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShopingAPI.DataLayer.Data;
using ShopingAPI.DataLayer;
using ShopingAPI.BusinessLayer.Repository;
using ShopingAPI.BusinessLayer.ViewModel;
using ShopingAPI.DataLayer.Models;

namespace ShopingAPI.WebAPI.WebAPI
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductController : CommanCtorController
    {
        public ProductController(ISingletonRepository _singleton, ILogger<CommanCtorController> logger) : base(_singleton,logger)
        {

        }
        [HttpPost]
        
        public async Task<ActionResult<IEnumerable<ProductViewModel>>> GetProducts(SearchProductViewModel searchProduct)
        {
            var result= await _singleton.productRepository.GetProducts(searchProduct);
            return result;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Unit>>> GetProductUnits()
        {
            var result = await _singleton.productRepository.GetProductUnits();
            return result;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryViewModel>>> GetProductCategory()
        {
            var result = await _singleton.productRepository.GetProductCategory();
            return result;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubCategoryViewModel>>> GetProductSubCategory(string categoryId)
        {
            var result = await _singleton.productRepository.GetProductSubCategory(categoryId);
            return result;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductAutoCompleteViewModel>>> GetSearchProductByName(string productName)
        {
            var result = await _singleton.productRepository.SearchProductByName(productName);
            return result;
        }


    }
}
