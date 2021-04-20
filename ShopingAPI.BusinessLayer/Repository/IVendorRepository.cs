using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ShopingAPI.BusinessLayer.Helpers;
using ShopingAPI.BusinessLayer.ViewModel;
using ShopingAPI.DataLayer.Data;
using ShopingAPI.DataLayer.Models;
using Rectangle = System.Drawing.Rectangle;

namespace ShopingAPI.BusinessLayer.Repository
{
    public interface IVendorRepository
    {
        Task<bool> AddVendorProducts(List<ProductViewModel> products, string vendorId);
        Task<bool> UpdateVendorProducts(List<VendorProductViewModel> products, string vendorId);
		Task<List<VendorProductViewModel>> GetVendorProducts(string userId, SearchProductViewModel searchProduct);
		Task<List<VendorProductViewModel>> GetVendorProductByShopId(SearchShopProductViewModel searchProduct);
		Task<List<VendorProductViewModel>> GetVendorProductById(List<ProductAddToCart> productAddToCarts);
		Task<List<ShopViewModel>> GetVendorShops(SearchShops searchShops);
		Task<bool> CreateVendorOrdersTable(string tblName);
		Task<List<ProductAutoCompleteViewModel>> SearchProductByName(string productName, string userId);
		Task<List<ProductAutoCompleteViewModel>> SearchProductByName(SearchProductByNameCategoryIdSubCategoryId productName);
		Task<List<ShopViewModel>> FilterShopByNo(string shopNo);
		Task<List<ShopViewModel>> FilterShopByAddres(string address);
		Task<List<CategoryViewModel>> GetShopCategory(string shopId);
        Task<bool> UpdateVendorShops(ShopViewModel shopViewModel, string userId);
        Task<ShopViewModel> GetVendorShopByShopId(string shopId);
        Task<bool> UpdateVendorShopImage(IFormFile file, string userId);
		Task<ShopViewModel> GetVendorShopProfile(string userId);
		Task<bool> AddVendorShopTableName(User user, string modifyBy);
		Task<List<BrandViewModel>> GetShopBrands(string shopId);
		Task<List<BrandViewModel>> GetProductBrands();
    }
	public class VendorRepository : Repository, IVendorRepository
    {
        private readonly ImageKitClass _documentStorage;
		private readonly IOptions<AppSettings> appSettings;

		public VendorRepository(DataContext _context, IMapper _mapper, IOptions<AppSettings> app) : base(_context, _mapper)
        {
            _documentStorage = new ImageKitClass();
			appSettings = app;

		}
        public Task<bool> AddVendorProducts(List<ProductViewModel> products, string vendorId)
        {
			string tblName = string.Empty;
			var shop =_context.Shops.Where(x => x.UserId == vendorId).FirstOrDefault();
            if (shop.tableName == null)
            {
				tblName = _context.DynamicTableNames.Where(x => x.IsActive && x.TableType=="VO").FirstOrDefault().TableName;
				CreateVendorProductTable(tblName);
				CreateVendorOrdersTable(tblName);
            }
            else
            {
				tblName = shop.tableName;
            }
			#region add CategoryIds and SubCategoryIds in shop table
			string[] categoryIds = shop.CategoryIds==null? new string[] { } : shop.CategoryIds.Split(',').ToArray();
			string[] subCategoryIds = shop.SubCategoryIds == null ? new string[] { } : shop.SubCategoryIds.Split(',').ToArray();
			string[] productCategoryIds = products.Select(x => x.CategoryId).ToArray();
			string[] productSubCategoryIds = products.Select(x => x.SubCategoryId).ToArray();
			string[] newShopCategoryIds = productCategoryIds.Except(categoryIds).ToArray().Union(categoryIds).ToArray();
			string[] newShopSubCategoryIds = productSubCategoryIds.Except(subCategoryIds).ToArray().Union(subCategoryIds).ToArray();
			shop.CategoryIds = String.Join(",", newShopCategoryIds);
			shop.SubCategoryIds = String.Join(",", newShopSubCategoryIds);
			#endregion
			AddVenderProducts(tblName, products, vendorId,shop.Id);
			_context.SaveChanges();
			return Task.FromResult(true);
        }

		public Task<List<VendorProductViewModel>> GetVendorProducts(string userId, SearchProductViewModel searchProduct)
        {
			var shop = _context.Shops.Where(x => x.UserId == userId).FirstOrDefault();
			string tblName = shop.tableName;
			var vendorProductsTable = new DataTable();
			DateTime dateTime = DateTime.Now;
			List<VendorProductViewModel> products = new List<VendorProductViewModel>();
			using (var command = _context.Database.GetDbConnection().CreateCommand())
			{
				int totalSkip = searchProduct.PageSize * (searchProduct.PageNumber - 1);
				StringBuilder sbWhere = new StringBuilder();
				sbWhere.Append($" where IsActive = true and shopId='{shop.Id}'");

				if (!string.IsNullOrEmpty(searchProduct.ProductId))
				{
					sbWhere.Append("and ProductId='").Append(searchProduct.ProductId).Append("' ");
				}
				else if(!string.IsNullOrEmpty(searchProduct.CategoryId) || !string.IsNullOrEmpty(searchProduct.BrandId))
                {
                    if (!string.IsNullOrEmpty(searchProduct.CategoryId))
                    {
						sbWhere.Append("and CategoryId='").Append(searchProduct.CategoryId).Append("' ");
					}
					if (!string.IsNullOrEmpty(searchProduct.SubCategoryId))
                    {
						sbWhere.Append("and SubCategoryId='").Append(searchProduct.SubCategoryId).Append("' ");
					}
					if (!string.IsNullOrEmpty(searchProduct.BrandId))
					{
						sbWhere.Append("and BrandId='").Append(searchProduct.BrandId).Append("' ");
					}

				}
                
				var result = _context.VendorProducts.FromSqlRaw($"select Id,IsOutStock,ProductId,ProductName,ProductUrl,Selling,MRP,ItemCount,ExpiryDate,Discount,UnitId,Description from VendorProduct.Vendor{tblName}Products {sbWhere.ToString()} order by Id OFFSET {totalSkip} ROWS FETCH NEXT {searchProduct.PageSize} ROWS ONLY;")
							.Select(x => new VendorProductViewModel
							{
								Id = x.Id,
								ProductId = x.ProductId,
								ProductName = x.ProductName,
								Description = x.Description,
								Discount = x.Discount,
								IsOutStock = x.IsOutStock,
								MRP = x.MRP,
								ExpiryDate= x.ExpiryDate,
								ItemCount=x.ItemCount.Value,
								ProductUrl =  _documentStorage.GetProductUri(x.ProductUrl).Result,
								Selling = x.Selling,
								UnitId = x.UnitId
							}).AsQueryable();
				products = result.ToList();
			}
			return Task.FromResult(products);
		}

		public static List<T> TableToList<T>(DataTable table)
		{
			List<T> rez = new List<T>();
			foreach (DataRow rw in table.Rows)
			{
				T item = Activator.CreateInstance<T>();
				foreach (DataColumn cl in table.Columns)
				{
					PropertyInfo pi = typeof(T).GetProperty(cl.ColumnName);

					if (pi != null && rw[cl] != DBNull.Value)
					{
						var propType = Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType;
						pi.SetValue(item, Convert.ChangeType(rw[cl], propType), new object[0]);
					}

				}
				rez.Add(item);
			}
			return rez;
		}
		private Task<bool> CreateVendorProductTable(string tblName)
        {
            string sql = @$"CREATE SCHEMA IF NOT EXISTS VendorProduct; CREATE TABLE VendorProduct.Vendor{tblName}Products (
	                    Id	varchar(50),
	                    IP	varchar(50),
	                    IsActive	bool NOT NULL,
	                    IsOutStock	bool NOT NULL,
	                    CreatedDateTime	timestamptz NOT NULL,
	                    UpdatedDateTime	timestamptz NOT NULL,
	                    ExpiryDate	timestamptz NULL,
	                    ModifyBy	varchar(50),
	                    ProductId	varchar(50),
	                    ProductName	varchar(150),
	                    ProductUrl	varchar(2000),
	                    Selling	decimal(10,2),
	                    MRP	decimal(10,2),
	                    Discount	decimal(10,2),
	                    UnitId	int,
						ItemCount int,
	                    Description	varchar(500),
	                    CategoryId	varchar(50),
	                    SubCategoryId	varchar(50),
	                    BrandId	varchar(50),
	                    ShopId	varchar(50),
	                    CONSTRAINT PK_Vendor{tblName}Products PRIMARY KEY(Id)
					    );
                        CREATE INDEX index_Vendor{tblName}Products_ProductId ON VendorProduct.Vendor{tblName}Products using BTREE (IsActive,ShopId,ProductId);
					    CREATE INDEX index_Vendor{tblName}Products_ProductName ON VendorProduct.Vendor{tblName}Products using BTREE (IsActive,ProductName);
					    CREATE INDEX index_Vendor{tblName}Products_CategoryId ON VendorProduct.Vendor{tblName}Products using BTREE ((IsActive,ShopId,CategoryId);
					    CREATE INDEX index_Vendor{tblName}Products_CatExpiryDate ON VendorProduct.Vendor{tblName}Products using BRIN (ExpiryDate) where ExpiryDate is not null;
					    CREATE INDEX index_Vendor{tblName}Products_SubCategoryId ON VendorProduct.Vendor{tblName}Products using BTREE ((IsActive,ShopId,CategoryId,SubCategoryId);
					    CREATE INDEX index_Vendor{tblName}Products_ShopId ON VendorProduct.Vendor{tblName}Products using BTREE ((IsActive,ShopId);
					    CREATE INDEX index_Vendor{tblName}Products_ShopId_BrandId ON VendorProduct.Vendor{tblName}Products using BTREE (IsActive,shopId,BrandId);
					    CREATE INDEX index_Vendor{tblName}Products_ShopId_Cat_BrandId ON VendorProduct.Vendor{tblName}Products using BTREE (IsActive,shopId,CategoryId,BrandId);
					    CREATE INDEX index_Vendor{tblName}Products_ShopId_Cat_SubCat_BrandId ON VendorProduct.Vendor{tblName}Products using BTREE (IsActive,shopId,CategoryId,SubCategoryId,BrandId);";
			//CONSTRAINT FK_Products_Categories_CategoryId FOREIGN KEY(CategoryId) REFERENCES Categories(Id) ON DELETE RESTRICT,
			//CONSTRAINT FK_Products_SubCategories_SubCategoryId FOREIGN KEY(SubCategoryId) REFERENCES SubCategories(Id) ON DELETE RESTRICT

			using (var command = _context.Database.GetDbConnection().CreateCommand())
			{
				command.CommandText = sql;
				_context.Database.OpenConnection();
				command.ExecuteNonQuery();
			}
            return Task.FromResult(true);
        }

        private Task<bool> AddVenderProducts(string tableName, List<ProductViewModel> products,string UserId,string shopId)
        {
			var vendorProductsTable = new DataTable();
			DateTime dateTime = DateTime.Now;
			var productId = products.Select(x => x.ProductId).ToArray();
			var productIds = string.Join(",", productId
											.Select(x => string.Format("'{0}'", x))); 
			

			List<Product> productForImage = _context.Products.Where(x => productId.Contains(x.Id)).ToList();
			using (var command = _context.Database.GetDbConnection().CreateCommand())
			{
				command.CommandText = $"select * from VendorProduct.Vendor{tableName}Products where shopId='{shopId}' and productId in ({productIds});";
				_context.Database.OpenConnection();
				using (var result = command.ExecuteReader())
				{
					StringBuilder sb = new StringBuilder();
					bool IsInsert = false;
					vendorProductsTable.Load(result);
					sb.Append($"insert into VendorProduct.Vendor{tableName}Products (Id,IP,IsActive,IsOutStock,CreatedDateTime,UpdatedDateTime,ExpiryDate,ModifyBy,ProductId,ProductName,ProductUrl,Selling,MRP,ItemCount,Discount,UnitId,Description,CategoryId,SubCategoryId,BrandId,shopId) values");
					foreach (var item in products)
                    {
						bool IsDuplicate = false;
						//ram naramyan
						var vendorProductsByProductId = (from vendorProducts in vendorProductsTable.AsEnumerable()
														 where vendorProducts.Field<string>("ProductId") == item.ProductId
														 select new
														 {
															 MRP = vendorProducts.Field<object>("MRP"),
															 Selling = vendorProducts.Field<object>("Selling"),
															 UnitId = vendorProducts.Field<object>("UnitId"),
														 }).ToList();
						
						foreach (var row in vendorProductsByProductId)
                        {
							if (Convert.ToDecimal(row.MRP) == item.MRP && Convert.ToDecimal(row.Selling) == item.Selling && Convert.ToInt32(row.UnitId) == item.UnitId)
							{
								IsDuplicate = true;
								break;
							}
                        }
                        if (!IsDuplicate)
                        {
							string Id = Guid.NewGuid().ToString();
							decimal discount = (((item.MRP - item.Selling) / item.MRP) * 100);
							string expiryDate=item.ExpiryDate.HasValue? $"'{item.ExpiryDate}'" : "null";
							string itemCount = item.ItemCount != null ? $"{item.ItemCount}" : "null";
							sb.Append($"('{Id}','',{true},{false},'{dateTime}','{dateTime}',{expiryDate},'{UserId}','{item.ProductId}','{item.ProductName}','{productForImage.Where(x=>x.Id==item.ProductId).FirstOrDefault().ImageUrl}',{item.Selling},{item.MRP},{itemCount},{item.Discount},{item.UnitId},'{item.Description}','{item.CategoryId}','{item.SubCategoryId}','{productForImage.Where(x => x.Id == item.ProductId).FirstOrDefault().BrandId}','{shopId}'),");
							IsInsert = true;
						}
					}
					sb.Remove(sb.Length-1,1);
					//sb.Append(";SET ansi_warnings ON");
                    if (IsInsert)
                    {
						command.CommandText = sb.ToString();
						int count=command.ExecuteNonQuery();
					}
				}
			}
			return Task.FromResult(true);
        }

        public Task<bool> UpdateVendorProducts(List<VendorProductViewModel> products, string userId)
        {
			DateTime dateTime = DateTime.Now;
			var shop = _context.Shops.Where(x => x.UserId == userId).FirstOrDefault();
			using (var command = _context.Database.GetDbConnection().CreateCommand())
			{
				string tblName = shop.tableName;
				{
					StringBuilder sb = new StringBuilder();
					foreach (var item in products)
					{
						string expiryDate = item.ExpiryDate.HasValue ? $"'{item.ExpiryDate}'" : "null";
						string itemCount = item.ItemCount != null ? $"{item.ItemCount}" : "null";
						sb.Append($"UPDATE VendorProduct.Vendor{tblName}Products Set IP='',IsOutStock={item.IsOutStock},UpdatedDateTime='{dateTime}',ExpiryDate={expiryDate},ModifyBy='{userId}'" +
							$",ProductId='{item.ProductId}',ProductUrl='',Selling={item.Selling},MRP={item.MRP},ItemCount={itemCount},Discount={item.Discount},UnitId={item.UnitId},Description='{item.Description}' where Id='{item.Id}' and shopId='{shop.Id}';");
					}
					command.CommandText = sb.ToString();
					_context.Database.OpenConnection();
					int count = command.ExecuteNonQuery();
					_context.Database.CloseConnection();
				}
			}
			return Task.FromResult(true);
		}

		public Task<List<ProductAutoCompleteViewModel>> SearchProductByName(string productName,string userId)
		{
			var shop = _context.Shops.Where(x => x.UserId == userId).FirstOrDefault();
			string tblName = shop.tableName;
            try
            {
				if (!string.IsNullOrEmpty(productName))
				{

					var result = _context.VendorProducts.FromSqlRaw($"select DISTINCT ProductName,ProductId from VendorProduct.Vendor{tblName}Products where ProductName like '%{productName}%';")
							.Select(x => new ProductAutoCompleteViewModel
							{
								ProductId = x.ProductId,
								ProductName = x.ProductName
							}).AsQueryable();

					return Task.FromResult(result.ToList());
				}
				else
				{
					var result = _context.VendorProducts.FromSqlRaw($"select ProductName,ProductId from VendorProduct.Vendor{tblName}Products order by ProductId OFFSET{ 5} ROWS;")
							.Select(x => new ProductAutoCompleteViewModel
							{
								ProductId = x.ProductId,
								ProductName = x.ProductName
							}).AsQueryable();
					return Task.FromResult(result.ToList());
				}
			}
            catch (Exception ex)
            {
                throw;
            }
		}


		public Task<List<ProductAutoCompleteViewModel>> SearchProductByName(SearchProductByNameCategoryIdSubCategoryId productName)
		{
			var shop = _context.Shops.Where(x=>x.Id==productName.ShopId).FirstOrDefault();
			try
			{
				if (shop!=null && !string.IsNullOrEmpty(productName.ProductId))
				{

					var result = _context.VendorProducts.FromSqlRaw($"select DISTINCT ProductName,ProductId from VendorProduct.Vendor{shop.tableName}Products where ProductName like '%{productName.ProductId}%' order by ProductName OFFSET 0 ROWS FETCH NEXT 15 rows ONLY;")
							.Select(x => new ProductAutoCompleteViewModel
							{
								ProductId = x.ProductId,
								ProductName = x.ProductName
							}).AsQueryable();

					return Task.FromResult(result.ToList());
				}
				else
				{
					var result = _context.VendorProducts.FromSqlRaw($"select DISTINCT ProductName,ProductId from VendorProduct.Vendor{shop.tableName}Products order by ProductName OFFSET 0 ROWS FETCH NEXT 5 rows ONLY;")
							.Select(x => new ProductAutoCompleteViewModel
							{
								ProductId = x.ProductId,
								ProductName = x.ProductName
							}).AsQueryable();
					return Task.FromResult(result.ToList());
				}
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public Task<bool> CreateVendorOrdersTable(string tblName)
        {
			string sql = @$"CREATE SCHEMA IF NOT EXISTS VendorOrder; CREATE TABLE VendorOrder.Vendor{tblName}Orders (
	                    Id varchar(50) NOT NULL CONSTRAINT PK_Vendor{tblName}Orders PRIMARY KEY,
						IP varchar(50) NULL,
						IsActive	bool NOT NULL,
						CreatedDateTime timestamptz NOT NULL,
						UpdatedDateTime timestamptz NOT NULL,
						ModifyBy varchar(50) NULL,
						OrderStatusId int NOT NULL,
						ItemCount	int NOT NULL,
						DeliveryCost decimal(10,2) NOT NULL,
						TotalAmount decimal(10,2) NOT NULL,
						Discount decimal(10,2) NOT NULL,
						PromoCode varchar(50) NULL,
						DeliveryStatus varchar(50) NULL,
						DeliveryDate timestamptz NULL,
						InvoiceNo varchar(50) NULL,
						PaymentStatus varchar(50) NULL,
						UserId varchar(50) NULL,
						UserAddressDetails varchar(1000) NOT NULL,
						ShopAddressDetails varchar(1000) NOT NULL,
						OrdersDetails text NOT NULL,
						CurrencyType varchar(10),
						ShopId varchar(50) NULL,
						CONSTRAINT FK_Vendor{tblName}Orders_Users_UserId FOREIGN KEY (UserId) REFERENCES AspNetUsers (Id) ON DELETE CASCADE
                    );
					CREATE INDEX index_Vendor{tblName}Orders_PriceHiddenByShopper ON VendorOrder.Vendor{tblName}Orders (OrderStatusId) where OrderStatusId={(int)EnumOrderStatus.PriceHiddenByShopper};
					CREATE INDEX index_Vendor{tblName}Orders_UnderPreparing ON VendorOrder.Vendor{tblName}Orders (OrderStatusId) where OrderStatusId={(int)EnumOrderStatus.UnderPreparing};
					CREATE INDEX index_Vendor{tblName}Orders_ReadyForDelivery ON VendorOrder.Vendor{tblName}Orders (OrderStatusId) where OrderStatusId={(int)EnumOrderStatus.ReadyForDelivery};
					CREATE INDEX index_Vendor{tblName}Orders_Cancel ON VendorOrder.Vendor{tblName}Orders (OrderStatusId) where OrderStatusId={(int)EnumOrderStatus.Cancel};
					CREATE INDEX index_Vendor{tblName}Orders_Delivered ON VendorOrder.Vendor{tblName}Orders (OrderStatusId) where OrderStatusId={(int)EnumOrderStatus.Delivered};
					CREATE INDEX index_Vendor{tblName}Orders_CreatedDateTime ON VendorOrder.Vendor{tblName}Orders using BRIN (CreatedDateTime);
					CREATE INDEX index_Vendor{tblName}Orders_ModifyBy ON VendorOrder.Vendor{tblName}Orders using BTREE (ModifyBy);
				";

			using (var command = _context.Database.GetDbConnection().CreateCommand())
			{
				command.CommandText = sql;
				_context.Database.OpenConnection();
				command.ExecuteNonQuery();
			}
			return Task.FromResult(true);
		}

        public Task<List<ShopViewModel>> GetVendorShops(SearchShops searchShops)
        {
			List<ShopViewModel> shopViews = new List<ShopViewModel>();
			List<Shop> shopIds = new List<Shop>();
			int totalSkip = searchShops.PageSize * (searchShops.PageNumber - 1);

			if (!string.IsNullOrEmpty(searchShops.ShopId))
            {
				shopIds = _context.Shops.Where(x=>x.Id==searchShops.ShopId).Skip(totalSkip).Take(searchShops.PageSize).ToList();
			}
			else if (!string.IsNullOrEmpty(searchShops.City) && searchShops.CategoryId!="null")
            {
				shopIds = _context.Shops.Where(x => x.City.ToLower().Contains(searchShops.City.ToLower()) && x.CategoryIds.Contains(searchShops.CategoryId)).Skip(totalSkip).Take(searchShops.PageSize).Distinct().ToList();
			}
			else if (!string.IsNullOrEmpty(searchShops.City))
            {
				shopIds = _context.Shops.Where(x => x.City.ToLower().Contains(searchShops.City.ToLower())).Skip(totalSkip).Take(searchShops.PageSize).Distinct().ToList();
			}
			else if (searchShops.CategoryId!=null&&searchShops.CategoryId != "null")
			{
				 shopIds = _context.Shops.Where(x =>x.CategoryIds.Contains(searchShops.CategoryId)).Skip(totalSkip).Take(searchShops.PageSize).Distinct().ToList();
			}
            else
            {
				shopIds = _context.Shops.Skip(totalSkip).Take(searchShops.PageSize).Distinct().ToList();
			}
			shopViews = _mapper.Map<List<ShopViewModel>>(shopIds);
			return Task.FromResult(shopViews);
        }
        public Task<List<VendorProductViewModel>> GetVendorProductByShopId(SearchShopProductViewModel searchProduct)
        {
			var shop = _context.Shops.Where(x => x.IsActive == true && x.Id== searchProduct.ShopId).FirstOrDefault();
			int totalSkip = searchProduct.PageSize * (searchProduct.PageNumber - 1);
			string whereClose = $" where IsActive = true and shopId='{shop.Id}'";
			if(searchProduct.BrandId != "null")
            {
				whereClose = whereClose + @$" and BrandId ='{searchProduct.BrandId}' ";
			}
			if (searchProduct.CategoryId != "null")
            {
				whereClose = whereClose + @$" and CategoryId ='{searchProduct.CategoryId}' ";
                if (searchProduct.SubCategoryId != "null")
                {
					whereClose = whereClose + @$" and SubCategoryId ='{searchProduct.SubCategoryId}' ";
				}
			}
			if (shop != null)
            {
				if(!string.IsNullOrEmpty(searchProduct.ProductId))
				whereClose = whereClose + @$" and v.ProductId ='{searchProduct.ProductId}' ";

				if (shop.IsPriceVisible)
				{
					var result = _context.VendorProducts.FromSqlRaw($"select v.Id,v.IsOutStock,v.ProductId,v.ProductName,v.ProductUrl,v.Selling,v.MRP,v.ItemCount,v.Discount,u.UnitType,v.Description,N'{shop.CurrencyType}' as CurrencyType from VendorProduct.Vendor{shop.tableName}Products v join Unit u on v.UnitId = u.Id {whereClose} order by v.Id OFFSET {totalSkip} ROWS FETCH NEXT {searchProduct.PageSize} ROWS ONLY;")
								.Select(x => new VendorProductViewModel
								{
									Id = x.Id,
									ProductId = x.ProductId,
									ProductName = x.ProductName,
									Description = x.Description,
									Discount = x.Discount,
									IsOutStock = x.IsOutStock,
									MRP = x.MRP,
									ItemCount = x.ItemCount.Value,
									ProductUrl = _documentStorage.GetProductUri(x.ProductUrl).Result,
									Selling = x.Selling,
									UnitType = x.UnitType,
									CurrencyType = x.CurrencyType
								}).AsQueryable();

					return Task.FromResult(result.ToList());
                }
                else
                {
					var result = _context.VendorProducts.FromSqlRaw($"select v.Id,v.IsOutStock,v.ProductId,v.ProductName,v.ProductUrl,v.ItemCount,u.UnitType,v.Description,N'{shop.CurrencyType}' as CurrencyType from VendorProduct.Vendor{shop.tableName}Products v join Unit u on v.UnitId = u.Id {whereClose} order by v.Id OFFSET {totalSkip} ROWS FETCH NEXT {searchProduct.PageSize} ROWS ONLY;")
								.Select(x => new VendorProductViewModel
								{
									Id = x.Id,
									ProductId = x.ProductId,
									ProductName = x.ProductName,
									Description = x.Description,
									Discount =0,
									IsOutStock = x.IsOutStock,
									MRP = 0,
									ItemCount = x.ItemCount.Value,
									ProductUrl = _documentStorage.GetProductUri(x.ProductUrl).Result,
									Selling = 0,
									UnitType = x.UnitType,
									CurrencyType = x.CurrencyType
								}).AsQueryable();

					return Task.FromResult(result.ToList());
				}
			}
			
			throw new NotImplementedException();
        }

        public Task<List<VendorProductViewModel>> GetVendorProductById(List<ProductAddToCart> productAddToCarts)
        {
            if (productAddToCarts.Count > 0)
            {
				StringBuilder stringBuilder = new StringBuilder();
                foreach (var item in productAddToCarts)
                {
					stringBuilder.Append("'");
					stringBuilder.Append(item.ProductId);
					stringBuilder.Append("',");
				}
				stringBuilder.Remove(stringBuilder.Length-1, 1);
				string whereClose = $" where v.IsActive = true and v.IsOutStock= false and v.id in({stringBuilder})";

				var shop = _context.Shops.Where(x => x.Id == productAddToCarts[0].ShopId).FirstOrDefault();
                if (shop == null)
                {
					List<VendorProductViewModel> nullList = new List<VendorProductViewModel>();
					return Task.FromResult(nullList);
                }
				if(shop.IsPriceVisible == false)
                {
					var result = _context.VendorProducts.FromSqlRaw($"select v.Id,v.IsOutStock,v.ProductId,v.ProductName,v.ProductUrl,v.ItemCount,u.UnitType,v.Description,N'{shop.CurrencyType}' as CurrencyType " +
					$"from VendorProduct.Vendor{shop.tableName}Products v join Unit u on v.UnitId = u.Id {whereClose};")
							.Select(x => new VendorProductViewModel
							{
								Id = x.Id,
								ProductId = x.ProductId,
								ProductName = x.ProductName,
								Description = x.Description,
								Discount = 0,
								IsOutStock = x.IsOutStock,
								MRP = 0,
								ItemCount=x.ItemCount.Value,
								ProductUrl = _documentStorage.GetProductUri(x.ProductUrl).Result,
								Selling = 0,
								UnitType = x.UnitType,
								CurrencyType = x.CurrencyType
							}).AsQueryable().ToList();
				var products = (from x in result
								join p in productAddToCarts on x.Id equals p.ProductId
								select new VendorProductViewModel
								{
									Id = x.Id,
									ProductId = x.ProductId,
									ProductName = x.ProductName,
									Description = x.Description,
									Discount = x.Discount,
									IsOutStock = x.IsOutStock,
									MRP = x.MRP,
									ItemCount = p.ItemCount,
									ProductUrl = x.ProductUrl,
									Selling = x.Selling,
									UnitType = x.UnitType,
									CurrencyType = x.CurrencyType
								}).ToList();
				return Task.FromResult(products);
                }
                else
                {
					var result = _context.VendorProducts.FromSqlRaw($"select v.Id,v.IsOutStock,v.ProductId,v.ProductName,v.ProductUrl,v.Selling,v.MRP,v.ItemCount,v.Discount,u.UnitType,v.Description,N'{shop.CurrencyType}' as CurrencyType " +
					$"from VendorProduct.Vendor{shop.tableName}Products v join Unit u on v.UnitId = u.Id {whereClose};")
							.Select(x => new VendorProductViewModel
							{
								Id = x.Id,
								ProductId = x.ProductId,
								ProductName = x.ProductName,
								Description = x.Description,
								Discount = x.Discount,
								IsOutStock = x.IsOutStock,
								MRP = x.MRP,
								ItemCount = x.ItemCount.Value,
								ProductUrl = _documentStorage.GetProductUri(x.ProductUrl).Result,
								Selling = x.Selling,
								UnitType = x.UnitType,
								CurrencyType = x.CurrencyType
							}).AsQueryable().ToList();
					var products = (from x in result
									join p in productAddToCarts on x.Id equals p.ProductId
									select new VendorProductViewModel
									{
										Id = x.Id,
										ProductId = x.ProductId,
										ProductName = x.ProductName,
										Description = x.Description,
										Discount = x.Discount,
										IsOutStock = x.IsOutStock,
										MRP = x.MRP,
										ItemCount = p.ItemCount,
										ProductUrl = x.ProductUrl,
										Selling = x.Selling,
										UnitType = x.UnitType,
										CurrencyType = x.CurrencyType
									}).ToList();
					return Task.FromResult(products);
				}
				
			}
            throw new NotImplementedException();
        }

        public Task<List<ShopViewModel>> FilterShopByNo(string shopNo)
        {
			if (string.IsNullOrEmpty(shopNo))
			{
				var result = (from p in _context.Shops
							  select new ShopViewModel
							  {
								  ShopId = p.Id,
								  ShopCode = p.ShopCode
							  }).Take(15).ToList();
				return Task.FromResult(result);
			}
			else
			{
				var result = (from p in _context.Shops
							  where p.ShopCode.ToLower().Contains(shopNo.ToLower())
							  select new ShopViewModel
							  {
								  ShopId = p.Id,
								  ShopCode = p.ShopCode
							  }).Take(15).ToList();
				return Task.FromResult(result);
			}
		}

        public Task<List<ShopViewModel>> FilterShopByAddres(string address)
        {
			if (string.IsNullOrEmpty(address))
			{
				var result = (from p in _context.Shops
							  select new ShopViewModel
							  {
								  City = p.City
							  }).Take(5).ToList();
				return Task.FromResult(result);
			}
			else
			{
				var result = (from p in _context.Shops
							  where p.City.ToLower().Contains(address.ToLower())
							  select new ShopViewModel
							  {
								  City = p.City
							  }).ToList();
				return Task.FromResult(result);
			}
		}

        public async Task<List<CategoryViewModel>> GetShopCategory(string shopId)
        {
			var shop=_context.Shops.Where(x => x.Id == shopId).FirstOrDefault();
			List<CategoryViewModel> categories = new List<CategoryViewModel>();
			if (shop != null)
            {
				string[] categoryIds = shop.CategoryIds==null?null: shop.CategoryIds.Split(',').ToArray();
				List<Category> categories1 = await _context.Categories.Where(x => categoryIds.Contains(x.Id)).ToListAsync();
				categories =  _mapper.Map<List<CategoryViewModel>>(categories1);
				return categories;

			}
			return categories;

		}

		public async Task<List<SubCategoryViewModel>> GetProductSubCategory(string categoryId)
		{
			List<SubCategory> subCategories1 = await _context.SubCategories.Where(x => x.IsActive == true && x.CategoryId == (categoryId == null ? x.CategoryId : categoryId)).ToListAsync();
			List<SubCategoryViewModel> subCategories = _mapper.Map<List<SubCategoryViewModel>>(subCategories1);
			return subCategories;
		}

        public async Task<bool> UpdateVendorShops(ShopViewModel shopViewModel, string userId)
        {
			Shop shop = await _context.Shops.Where(x =>x.UserId == userId ).FirstOrDefaultAsync();
            if (shop != null)
            {
				shopViewModel.ShopImageUrl = shop.ShopImageUrl;
				shopViewModel.ShopId = shop.Id;
				shopViewModel.ShopCode = shop.ShopCode;
				_mapper.Map<ShopViewModel, Shop>(shopViewModel, shop);
				_context.SaveChanges();
				return  true;
			}
			return false;
        }

        public async Task<ShopViewModel> GetVendorShopByShopId(string shopId)
        {
			Shop shop = await _context.Shops.Where(x => x.Id == shopId).FirstOrDefaultAsync();
			if (shop == null)
            {
				ShopViewModel shopViewModel = _mapper.Map<ShopViewModel>(shop);
				return shopViewModel;
            }
            else
            {
				return null;
            }
		    
		}

        public async Task<ShopViewModel> GetVendorShopProfile(string userId)
        {
			Shop shop = await _context.Shops.Where(x => x.UserId == userId).FirstOrDefaultAsync();
			if (shop != null)
			{
				shop.Id = null;
				ShopViewModel shopViewModel = _mapper.Map<ShopViewModel>(shop);
				return shopViewModel;
			}
			else
			{
				return null;
			}
		}
		public async Task<bool> AddVendorShopTableName(User user,string modifyBy)
		{
			Shop shop = await _context.Shops.Where(x => x.UserId == user.Id).FirstOrDefaultAsync();
			DynamicTableName dynamicTbl = await _context.DynamicTableNames.Where(x => x.IsActive == true && x.TableType == "VO").FirstOrDefaultAsync();
			if (dynamicTbl == null)
			{
				string id = Guid.NewGuid().ToString();
				DynamicTableName dynamicTable = new DynamicTableName
				{
					Id = id,
					TableName = id.Replace("-", ""),
					TableType = "VO",
					ModifyBy = modifyBy
				};
				_context.DynamicTableNames.Add(dynamicTable);
				await CreateVendorProductTable(dynamicTable.TableName);
				await CreateVendorOrdersTable(dynamicTable.TableName);
				dynamicTbl = dynamicTable;
			}
			if (shop != null)
			{
				if (shop.tableName == null)
				{
					shop.ModifyBy = modifyBy;
					shop.tableName = dynamicTbl.TableName;
                }
                else
                {
					return true;
				}
			}
			else
			{
				string shopCode = $"{appSettings.Value.ShopCode}{_context.Shops.Count().ToString("0000")}";
				Shop newShop = new Shop
				{
					Id= Guid.NewGuid().ToString(),
					Email=user.Email,
					MobileNumber=user.Mobile,
					IsActive=true,
					ModifyBy=modifyBy,
					DeliveryCost=0,
					ShopCode= shopCode,
					tableName=dynamicTbl.TableName,
					UserId=user.Id,
					ShopImageUrl=appSettings.Value.CommingSoonImageUrl
				};
				_context.Shops.Add(newShop);
			}

			await _context.SaveChangesAsync();
			return true;

		}
		public async Task<bool> UpdateVendorShopImage(IFormFile file, string userId)
		{
			Shop shop = await _context.Shops.Where(x => x.UserId == userId).FirstOrDefaultAsync();
			if (shop != null)
			{

				shop.ShopImageUrl = GenerateThumbnailsBase64(0.5, file);
				_context.SaveChanges();
				return true;
			}
			return false;
		}
		private string GenerateThumbnailsBase64(double scaleFactor, IFormFile file)
		{
			if (file.Length > 0)
			{
				using (var image = System.Drawing.Image.FromStream(file.OpenReadStream()))
				{
					var newWidth = (int)(image.Width * scaleFactor);
					var newHeight = (int)(image.Height * scaleFactor);
					var thumbnailImg = new Bitmap(newWidth, newHeight);
					var thumbGraph = Graphics.FromImage(thumbnailImg);
					thumbGraph.CompositingQuality = CompositingQuality.HighQuality;
					thumbGraph.SmoothingMode = SmoothingMode.HighQuality;
					thumbGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
				    var imageRectangle = new Rectangle(0, 0, newWidth, newHeight);
					thumbGraph.DrawImage(image, imageRectangle);

					using (var ms = new MemoryStream())
					{
						thumbnailImg.Save(ms,image.RawFormat);
						var fileBytes = ms.ToArray();
						string s = Convert.ToBase64String(fileBytes);
						// act on the Base64 data
						return "data:image/png;base64," + s;
					}
				}
			}
			return null;
		}

        public Task<List<BrandViewModel>> GetShopBrands(string shopId)
        {
			Shop shop =_context.Shops.Where(x => x.Id == shopId).FirstOrDefault();
			List<BrandViewModel> brandViewModel = new List<BrandViewModel>();
				if (shop != null)
            {
				string whereClose = $" where v.IsActive = true and v.shopId='{shop.Id}'";
                IQueryable<BrandViewModel> result = _context.Brands.FromSqlRaw($"select distinct v.BrandId,b.Id,b.BrandName" +
                        $" from VendorProduct.Vendor{shop.tableName}Products v join Brands b on b.Id = v.BrandId {whereClose};")
						.Select(x => new BrandViewModel
						{
							Id = x.Id,
							BrandName=x.BrandName
						}).AsQueryable();

				brandViewModel = _mapper.Map<List<BrandViewModel>>(result);
				return  Task.FromResult(brandViewModel);

			}
			else
            {
				return Task.FromResult(brandViewModel);
			}
		}

        public async Task<List<BrandViewModel>> GetProductBrands()
        {
			List<BrandViewModel> brandViewModel = new List<BrandViewModel>();
			var result = await _context.Brands.Where(x => x.IsActive == true).ToListAsync();
			brandViewModel = _mapper.Map<List<BrandViewModel>>(result);
			return brandViewModel;

		}
	}
}
