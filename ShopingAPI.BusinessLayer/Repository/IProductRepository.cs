using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ShopingAPI.DataLayer.Data;
using ShopingAPI.DataLayer.Models;
using ShopingAPI.BusinessLayer.ViewModel;

namespace ShopingAPI.BusinessLayer.Repository
{
    public interface IProductRepository
    {
        Task<List<ProductViewModel>> GetProducts(SearchProductViewModel searchProduct);
        Task<List<ProductAutoCompleteViewModel>> SearchProductByName(string productName);
        Task<List<Unit>> GetProductUnits();
        Task<List<CategoryViewModel>> GetProductCategory();
        Task<List<SubCategoryViewModel>> GetProductSubCategory(string CategoryId);
    }

    public class ProductRepository : Repository, IProductRepository
    {
        public ProductRepository(DataContext _context, IMapper _mapper) :base(_context,_mapper)
        {
        }

       

        public async Task<List<ProductViewModel>> GetProducts(SearchProductViewModel searchProduct)
        {
            List<Product> product1 = new List<Product>();
            List<ProductViewModel> products;
            if (!string.IsNullOrEmpty(searchProduct.ProductId))
            {
                product1 = await _context.Products.Where(x => x.Id == searchProduct.ProductId).Skip(searchProduct.PageSize * (searchProduct.PageNumber - 1)).Take(searchProduct.PageSize).ToListAsync();
            }
            else if (!string.IsNullOrEmpty(searchProduct.CategoryId) || !string.IsNullOrEmpty(searchProduct.BrandId))
            {
                if (!string.IsNullOrEmpty(searchProduct.SubCategoryId))
                {
                    if(!string.IsNullOrEmpty(searchProduct.BrandId))
                        product1 = await _context.Products.Where(x => x.SubCategoryId == searchProduct.SubCategoryId && x.BrandId==searchProduct.BrandId).Skip(searchProduct.PageSize * (searchProduct.PageNumber - 1)).Take(searchProduct.PageSize).ToListAsync();
                    else
                        product1 = await _context.Products.Where(x => x.SubCategoryId == searchProduct.SubCategoryId).Skip(searchProduct.PageSize * (searchProduct.PageNumber - 1)).Take(searchProduct.PageSize).ToListAsync();
                }
                else
                {
                    product1 = await _context.Products
                        .Where(x => x.CategoryId == (searchProduct.CategoryId!=null?searchProduct.CategoryId:x.CategoryId) && x.BrandId == (searchProduct.BrandId!=null?searchProduct.BrandId:x.BrandId)).Skip(searchProduct.PageSize * (searchProduct.PageNumber - 1)).Take(searchProduct.PageSize).ToListAsync();

                    //if(!string.IsNullOrEmpty(searchProduct.CategoryId) && !string.IsNullOrEmpty(searchProduct.BrandId))
                    //else if(!string.IsNullOrEmpty(searchProduct.CategoryId) && !string.IsNullOrEmpty(searchProduct.BrandId))
                    //    product1 = await _context.Products.Where(x => x.CategoryId == searchProduct.CategoryId && x.BrandId == searchProduct.BrandId).Skip(searchProduct.PageSize * (searchProduct.PageNumber - 1)).Take(searchProduct.PageSize).ToListAsync();
                    //else
                    //product1 = await _context.Products.Where(x => x.CategoryId == searchProduct.CategoryId).Skip(searchProduct.PageSize * (searchProduct.PageNumber - 1)).Take(searchProduct.PageSize).ToListAsync();
                }

            }
            else
            {
                product1 = await _context.Products.Skip(searchProduct.PageSize* (searchProduct.PageNumber-1)).Take(searchProduct.PageSize).ToListAsync();
            }
            products = _mapper.Map<List<ProductViewModel>>(product1);
            return products;
        }
        public async Task<List<CategoryViewModel>> GetProductCategory()
        {
            List<Category> categories1 = await _context.Categories.Where(x => x.IsActive == true).ToListAsync();
            List<CategoryViewModel> categories=_mapper.Map<List<CategoryViewModel>>(categories1);
            return categories;
        }
        public async Task<List<SubCategoryViewModel>> GetProductSubCategory(string categoryId)
        {
            List<SubCategory> subCategories1 = await _context.SubCategories.Where(x => x.IsActive == true && x.CategoryId==(categoryId==null?x.CategoryId:categoryId)).ToListAsync();
            List<SubCategoryViewModel> subCategories = _mapper.Map<List<SubCategoryViewModel>>(subCategories1);
            return subCategories;
        }

        public Task<List<Unit>> GetProductUnits()
        {
            return _context.Unit.ToListAsync();
        }

        public Task<List<ProductAutoCompleteViewModel>> SearchProductByName(string productName)
        {
            if (string.IsNullOrEmpty(productName))
            {
                var result= (from p in _context.Products
                        select new ProductAutoCompleteViewModel
                        {
                            ProductId = p.Id,
                            ProductName = p.ProductName
                        }).Take(5).ToList();
                return Task.FromResult(result);
            }
            else
            {
                var result = (from p in _context.Products
                              where p.ProductName.ToLower().Contains(productName.ToLower())
                              select new ProductAutoCompleteViewModel
                              {
                                  ProductId = p.Id,
                                  ProductName = p.ProductName
                              }).ToList();
                return Task.FromResult(result);
            }
        }
    }
}
