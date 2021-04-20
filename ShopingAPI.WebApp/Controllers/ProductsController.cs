using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Imagekit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopingAPI.DataLayer.Data;
using ShopingAPI.DataLayer.Models;
using ShopingAPI.BusinessLayer.ViewModel;

namespace ShopingAPI.WebApp.Controllers
{
    [AllowAnonymous]
    public class ProductsController : Controller
    {
        private readonly DataContext _context;
        private readonly ServerImagekit _imageKit;
        public ProductsController(DataContext context)
        {
            _context = context;
            _imageKit= new ServerImagekit("public_0nkC4AqsdxiHI1aZLphtt1H7d0g=", "private_kXENeAklGkYf46xZegNcJScwxLM=", "https://ik.imagekit.io/en9grhpyzhz/", "path");

        }

        // GET: Products
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var dataContext = await _context.Products.Include(p => p.Category).Include(p => p.SubCategory).ToListAsync();
            var products = dataContext.Select(x => new ProductViewModel
              {                 
                CategoryId=x.Category.Name,
                Description=x.Description,
                SubCategoryId=x.SubCategory!=null?x.SubCategory.SubCategoryName:"",
                ProductName=x.ProductName,
                //ImageUrl=_documentStorage.GetBlobUriWithSasToken(x.ImageUrl),
                ProductId=x.Id,
              }).ToList();
            return View(products);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.SubCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        [AllowAnonymous]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["SubCategoryId"] = new SelectList(_context.SubCategories, "Id", "SubCategoryName");
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "BrandName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Create([Bind("ProductName,ImageUrl,CategoryId,SubCategoryId,BrandId,Id,IP,IsActive,CreatedDateTime,UpdatedDateTime,ModifyBy")] AddProductViewModel product)
        {
            string ipAddress = "";
            if (Dns.GetHostAddresses(Dns.GetHostName()).Length > 0)
            {
                ipAddress = Dns.GetHostAddresses(Dns.GetHostName())[0].ToString();
            }
            product.IP = ipAddress;
            product.Id = Guid.NewGuid().ToString();
            product.UpdatedDateTime = DateTime.Now;
            product.CreatedDateTime = DateTime.Now;
            StringBuilder  imageUrl = new StringBuilder();
            int i = 1;
            foreach (var file in product.ImageUrl)
            {
                if (file.Length > 0)
                {
                    string ext=Path.GetExtension(file.FileName);
                    using (var ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        //ImagekitResponse response1 = await _imageKitClass.Path("").UploadAsync(fileBytes, product.Id, ext,null);//_documentStorage.UploadDocumentAsync(fileBytes, file.ContentType, product.CategoryId, product.Id+"-"+i+ext);
                         ImagekitResponse response = await _imageKit.Folder(product.CategoryId).isPrivateFile(true).UseUniqueFileName(false).FileName($"{product.Id}{ext}").UploadAsync(fileBytes);
                        //imageUrl.Append(cloudBlockBlob.Uri.AbsoluteUri);
                        //imageUrl.Append(",");
                    }
                    i++;
                }
            }
            if (ModelState.IsValid)
            {
                Product addProduct = new Product
                {
                    Category=product.Category,
                    CategoryId=product.CategoryId,
                    CreatedDateTime=product.CreatedDateTime,
                    Description=product.Description,
                    Id=product.Id,
                    ImageUrl= imageUrl.ToString().TrimEnd(','),
                    IP=product.IP,
                    IsActive=product.IsActive,
                    ModifyBy=product.ModifyBy,
                    ProductName=product.ProductName,
                    SubCategory=product.SubCategory,
                    SubCategoryId=product.SubCategoryId,
                    UpdatedDateTime=product.UpdatedDateTime,
                    BrandId=product.BrandId
                };
                _context.Add(addProduct);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", product.CategoryId);
            ViewData["SubCategoryId"] = new SelectList(_context.SubCategories, "Id", "Id", product.SubCategoryId);
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "BrandName",product.BrandId);

            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            AddProductViewModel addProductViewModel = new AddProductViewModel
            {
                CategoryId=product.CategoryId,
                SubCategoryId=product.SubCategoryId,
                ProductName=product.ProductName,
                Id=product.Id,
                IsActive=product.IsActive,
                Description=product.Description,
                BrandId=product.BrandId
            };
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            ViewData["SubCategoryId"] = new SelectList(_context.SubCategories, "Id", "SubCategoryName", product.SubCategoryId);
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "BrandName",product.BrandId);
            return View(addProductViewModel);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ProductName,ImageUrl,CategoryId,SubCategoryId,BrandId,Id,IP,IsActive,CreatedDateTime,UpdatedDateTime,ModifyBy")] AddProductViewModel product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Product updateProduct= _context.Products.Find(product.Id);
                    StringBuilder imageUrl = new StringBuilder();
                    int i = 1;
                    if(product.ImageUrl!=null)
                    foreach (var file in product.ImageUrl)
                    {
                        if (file.Length > 0)
                        {
                            string ext = Path.GetExtension(file.FileName);
                            using (var ms = new MemoryStream())
                            {
                                file.CopyTo(ms);
                                var fileBytes = ms.ToArray();
                                    string[] purgeURL = updateProduct.ImageUrl!=null?updateProduct.ImageUrl.Split(','):null;
                                    //ImagekitResponse response = await _imageKitClass.UploadAsync(fileBytes, product.Id, ext,purgeURL);
                                    ImagekitResponse response = await _imageKit.Folder(product.CategoryId).isPrivateFile(true).UseUniqueFileName(false).FileName($"{product.Id}{ext}").UploadAsync(fileBytes);
                                    //_documentStorage.UploadDocumentAsync(fileBytes, file.ContentType, product.CategoryId, product.Id+"-"+i+ext);
                                    //var cloudBlockBlob = await _documentStorage.UploadDocumentAsync(fileBytes, file.ContentType, product.CategoryId, product.Id + "-" + i + ext);
                                    imageUrl.Append(response.URL);
                                    imageUrl.Append(",");
                                    imageUrl.Append(response.Thumbnail);
                                    imageUrl.Append(",");

                                }
                                i++;
                        }
                    }
                    if (imageUrl.Length>0)
                    {
                        updateProduct.ImageUrl = imageUrl.ToString().TrimEnd(',');
                    }
                    string ipAddress = "";
                    if (Dns.GetHostAddresses(Dns.GetHostName()).Length > 0)
                    {
                        ipAddress = Dns.GetHostAddresses(Dns.GetHostName())[0].ToString();
                    }
                    product.IP = ipAddress;
                    product.UpdatedDateTime = DateTime.Now;

                    updateProduct.CategoryId = product.CategoryId;
                    updateProduct.Description = product.Description;
                    updateProduct.IP = product.IP;
                    updateProduct.IsActive = product.IsActive;
                    updateProduct.ModifyBy = product.ModifyBy;
                    updateProduct.ProductName = product.ProductName;
                    updateProduct.SubCategoryId = product.SubCategoryId;
                    updateProduct.UpdatedDateTime = product.UpdatedDateTime;
                    updateProduct.BrandId = product.BrandId;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", product.CategoryId);
            ViewData["SubCategoryId"] = new SelectList(_context.SubCategories, "Id", "Id", product.SubCategoryId);
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "BrandName",product.BrandId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.SubCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(string id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
