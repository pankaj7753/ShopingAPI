using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ShopingAPI.BusinessLayer.Helpers;
using ShopingAPI.BusinessLayer.ViewModel;
using ShopingAPI.DataLayer.Data;
using ShopingAPI.DataLayer.Models;

namespace ShopingAPI.BusinessLayer.Repository
{
    public interface IUserRepository
    {
        Task<bool> CreateUserOrdersTable(string tblName);
        Task<string> GetUserOrdersTableOnRegister(string userId);
        Task<string> GetUserOrdersTableName(string userId);
		Task<bool> AddOrderWithAddress(UserAddressViewModel userAddressViewModel, string userId, string userTableName, out string orderSummaryId);
		Task<UserAddressViewModel> GetUserAddress(string userId);
		Task<List<ShopViewModel>> GetUserFavoriteShops(string userId);
		Task<bool> AddUserFavoriteShop(string userId,string shopId);
		Task<bool> RemoveUserFavoriteShop(string userId,string shopId);
	}

    public class UserRepository: Repository, IUserRepository
    {
        private readonly ImageKitClass _documentStorage;

        public UserRepository(DataContext _context, IMapper _mapper) : base(_context, _mapper)
        {
            _documentStorage = new ImageKitClass();
        }

        [Obsolete]
        public Task<bool> AddOrderWithAddress(UserAddressViewModel userAddressViewModel,string userId, string userTableName, out string orderSummaryId)
        {
			string ipAddress = "";
			if (Dns.GetHostAddresses(Dns.GetHostName()).Length > 0)
			{
				ipAddress = Dns.GetHostAddresses(Dns.GetHostName())[0].ToString();
			}
			var jsonSerializerSettings = new JsonSerializerSettings
			{
				ContractResolver = new CamelCasePropertyNamesContractResolver()
			};
			#region Add or Update userAddress
			var updateUserAddress = _context.UserAddresses.Where(x => x.UserId == userId).FirstOrDefault();
				UserAddress userAddress = _mapper.Map<UserAddress>(userAddressViewModel);
				userAddress.UserId = userId;

				if (updateUserAddress != null)
				{
					var updatedEntity = _mapper.Map(userAddressViewModel, updateUserAddress);
					updatedEntity.UserId = userId;
					userAddress.Id = updateUserAddress.Id;
					var entity = _context.Update(updatedEntity);
				}
				else
				{
					userAddress.Id = Guid.NewGuid().ToString();
					_context.UserAddresses.Add(userAddress);

				}
			#endregion
			AddressViewModel userAddressModel = _mapper.Map<AddressViewModel>(userAddress);
			OrderSummary orderSummary = new OrderSummary()
			{
				IP = ipAddress,
				Id = Guid.NewGuid().ToString(),
				UpdatedDateTime = DateTime.Now,
				CreatedDateTime = DateTime.Now,
				OrderStatusId = (int)EnumOrderStatus.UnderPreparing,
				UserId = userId,
				InvoiceNo = "IN" + (_context.OrderSummaries.Count() + 1).ToString("000000"),
				UserAddressDetails= Newtonsoft.Json.JsonConvert.SerializeObject(userAddressModel, jsonSerializerSettings),
				IsActive = true,
				PromoCode="",
				ModifyBy=userId,
				PaymentStatus="",
				ShopId=userAddressViewModel.CartProducts[0].ShopId
				
			};
			if (userAddressViewModel.CartProducts.Length > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (var item in userAddressViewModel.CartProducts)
				{
					stringBuilder.Append("'");
					stringBuilder.Append(item.ProductId);
					stringBuilder.Append("',");
				}
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
				string whereClose = $" where v.IsActive = true and v.IsOutStock= false and v.id in({stringBuilder})";

				var shop = _context.Shops.Where(x => x.Id == userAddressViewModel.CartProducts[0].ShopId).FirstOrDefault();
				if (shop == null)
				{
					orderSummaryId = "";
					return null;
				}
				
				AddressViewModel shopAddressModel = _mapper.Map<AddressViewModel>(shop);
				var shopAddressJson = Newtonsoft.Json.JsonConvert.SerializeObject(shopAddressModel, jsonSerializerSettings);


				var result = _context.VendorProducts.FromSqlRaw($"select v.Id,v.IsOutStock,v.ProductId,v.ProductName,v.ProductUrl,v.Selling,v.MRP,v.Discount,u.UnitType,v.Description, '{shop.CurrencyType}' as CurrencyType " +
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
								ProductUrl = _documentStorage.GetProductThumbnailUri(x.ProductUrl).Result,
								Selling = x.Selling,
								UnitType = x.UnitType,
								CurrencyType=x.CurrencyType
							}).AsQueryable().ToList();
				var products = (from x in result
								join p in userAddressViewModel.CartProducts on x.Id equals p.ProductId
								select new VendorProductViewModel
								{
									ProductName = x.ProductName,
									Description = x.Description,
									Discount = x.Discount,
									IsOutStock = x.IsOutStock,
									MRP = x.MRP,
									ProductUrl = x.ProductUrl,
									Selling = x.Selling,
									UnitType = x.UnitType,
									ItemCount = p.ItemCount,
									SubTotalOnSelling=p.ItemCount *x.Selling,
									SubTotalOnMRP=p.ItemCount *x.MRP,
									CurrencyType = x.CurrencyType
								}).ToList();
				orderSummary.OrdersDetails = Newtonsoft.Json.JsonConvert.SerializeObject(products, jsonSerializerSettings);
				orderSummary.DeliveryCost = shop.DeliveryCost==null?0:shop.DeliveryCost;
				orderSummary.Discount = products.Sum(x => x.SubTotalOnMRP) - products.Sum(x => x.SubTotalOnSelling);
				orderSummary.TotalAmount = products.Sum(x => x.SubTotalOnSelling);
				orderSummary.ItemCount = products.Sum(x => x.ItemCount.Value);
				orderSummary.CurrencyType = shop.CurrencyType;
				orderSummary.ShopId = shop.Id;
				orderSummary.OrderStatusId = shop.IsPriceVisible == false ? (int)EnumOrderStatus.PriceHiddenByShopper : (int)EnumOrderStatus.UnderPreparing;
				orderSummary.ShopAddressDetails = shopAddressJson;
				using (var scope = _context.Database.BeginTransaction())
				{
					
                    try
                    {
						var vendorOrder = $"insert into VendorOrder.Vendor{shop.tableName}Orders(Id,IP,IsActive,CreatedDateTime,UpdatedDateTime,ModifyBy,OrderStatusId,ItemCount,DeliveryCost,TotalAmount," +
											$"Discount,PromoCode,DeliveryStatus,DeliveryDate,InvoiceNo,PaymentStatus,UserId,UserAddressDetails,ShopAddressDetails,OrdersDetails,CurrencyType,ShopId)" +
											$" values('{orderSummary.Id}','{orderSummary.IP}','{orderSummary.IsActive}','{orderSummary.CreatedDateTime}','{orderSummary.UpdatedDateTime}','{orderSummary.ModifyBy}','{orderSummary.OrderStatusId}',{orderSummary.ItemCount},{orderSummary.DeliveryCost},{orderSummary.TotalAmount}," +
											$"{orderSummary.Discount},'{orderSummary.PromoCode}','{orderSummary.DeliveryStatus}',null,'{orderSummary.InvoiceNo}','{orderSummary.PaymentStatus}','{orderSummary.UserId}','{orderSummary.UserAddressDetails}','{shopAddressJson}','{orderSummary.OrdersDetails}','{shop.CurrencyType}','{shop.Id}')";

						var userOrder = $"insert into UserOrders.User{userTableName}Orders(Id,IP,IsActive,CreatedDateTime,UpdatedDateTime,ModifyBy,OrderStatusId,ItemCount,DeliveryCost,TotalAmount," +
											$"Discount,PromoCode,DeliveryStatus,DeliveryDate,InvoiceNo,PaymentStatus,UserId,UserAddressDetails,ShopAddressDetails,OrdersDetails,CurrencyType,ShopId)" +
											$" values('{orderSummary.Id}','{orderSummary.IP}','{orderSummary.IsActive}','{orderSummary.CreatedDateTime}','{orderSummary.UpdatedDateTime}','{orderSummary.ModifyBy}','{orderSummary.OrderStatusId}',{orderSummary.ItemCount},{orderSummary.DeliveryCost},{orderSummary.TotalAmount}," +
											$"{orderSummary.Discount},'{orderSummary.PromoCode}','{orderSummary.DeliveryStatus}',null,'{orderSummary.InvoiceNo}','{orderSummary.PaymentStatus}','{orderSummary.UserId}','{orderSummary.UserAddressDetails}','{shopAddressJson}','{orderSummary.OrdersDetails}',N'{shop.CurrencyType}','{shop.Id}')";
						using (var command = _context.Database.GetDbConnection().CreateCommand())
						{
							command.CommandText = vendorOrder +";"+ userOrder;
							_context.Database.OpenConnection();
							command.Transaction = _context.Database.CurrentTransaction.GetDbTransaction();
							command.ExecuteNonQuery();
						}
						_context.OrderSummaries.Add(orderSummary);
						_context.SaveChanges();
						scope.Commit();
					}
					catch (Exception ex)
                    {
						scope.Rollback();
                        throw;
						int? i;
                    }
					
				}

				//var myDictionary = new Dictionary<string, Func<DbContext, IQueryable>>()
				//	{
				//		{ "TblStudents", ( DbContext context ) => context.Set<OrderSummary>() }
				//	};

				//var dbSet = myDictionary["TblStudents"].Invoke(_context);

				
			}
			orderSummaryId = orderSummary.Id;
			return Task.FromResult(true);
        }

        public async Task<string> GetUserOrdersTableOnRegister(string userId)
        {
			DynamicTableName dynamicTbl = await _context.DynamicTableNames.Where(x => x.IsActive == true && x.TableType == "UO").FirstOrDefaultAsync();
			if (dynamicTbl == null)
			{
				string id = Guid.NewGuid().ToString();
				DynamicTableName dynamicTable = new DynamicTableName
				{
					Id = id,
					TableName = id.Replace("-", ""),
					TableType = "UO",
					ModifyBy = userId
				};
				_context.DynamicTableNames.Add(dynamicTable);
				await CreateUserOrdersTable(dynamicTable.TableName);
				dynamicTbl = dynamicTable;
			}
			return dynamicTbl.TableName;

        }

        public Task<bool> CreateUserOrdersTable(string tblName)
		{
			string sql = @$"CREATE SCHEMA IF NOT EXISTS UserOrders;CREATE TABLE UserOrders.User{tblName}Orders (
							Id varchar(50) NOT NULL CONSTRAINT PK_User{tblName}Orders PRIMARY KEY,
							IP varchar(50) NULL,
							IsActive bool NOT NULL,
							CreatedDateTime timestamptz NOT NULL,
							UpdatedDateTime timestamptz NOT NULL,
							ModifyBy varchar(50) NULL,
							OrderStatusId int NOT NULL,
							ItemCount int NOT NULL,
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
							OrdersDetails varchar(4000) NOT NULL,
							CurrencyType varchar(10),
							ShopId varchar(50) NULL,
							CONSTRAINT FK_User{tblName}Orders_Users_UserId FOREIGN KEY (UserId) REFERENCES AspNetUsers (Id) ON DELETE CASCADE
                    );
					CREATE INDEX index_User{tblName}Orders_PriceHiddenByShopper ON UserOrders.User{tblName}Orders (OrderStatusId,userId) where OrderStatusId={(int)EnumOrderStatus.PriceHiddenByShopper};
					CREATE INDEX index_User{tblName}Orders_UnderPreparing ON UserOrders.User{tblName}Orders (OrderStatusId,userId) where OrderStatusId={(int)EnumOrderStatus.UnderPreparing};
					CREATE INDEX index_User{tblName}Orders_ReadyForDelivery ON UserOrders.User{tblName}Orders (OrderStatusId,userId) where OrderStatusId={(int)EnumOrderStatus.ReadyForDelivery};
					CREATE INDEX index_User{tblName}Orders_Cancel ON UserOrders.User{tblName}Orders (OrderStatusId,userId) where OrderStatusId={(int)EnumOrderStatus.Cancel};
					CREATE INDEX index_User{tblName}Orders_Delivered ON UserOrders.User{tblName}Orders (OrderStatusId,userId) where OrderStatusId={(int)EnumOrderStatus.Delivered};
					CREATE INDEX index_User{tblName}Orders_CreatedDateTime ON UserOrders.User{tblName}Orders using BRIN (CreatedDateTime,userId);
					CREATE INDEX index_User{tblName}Orders_ModifyBy ON UserOrders.User{tblName}Orders using BTREE (ModifyBy);
					";

			using (var command = _context.Database.GetDbConnection().CreateCommand())
			{
				command.CommandText = sql;
				_context.Database.OpenConnection();
				command.ExecuteNonQuery();
			}
			return Task.FromResult(true);
		}

        public async Task<UserAddressViewModel> GetUserAddress(string userId)
        {
			var userAddress = await _context.UserAddresses.Where(x => x.UserId == userId).FirstOrDefaultAsync();
			UserAddressViewModel userAddressView = _mapper.Map<UserAddressViewModel>(userAddress);

			return userAddressView;
		}

        public async Task<string> GetUserOrdersTableName(string userId)
        {
			User user= await _context.Users.FindAsync(userId);
			if(user != null)
            {
				return user.TableName;
            }
			throw new Exception("Unbale to process request");
        }

        public Task<List<ShopViewModel>> GetUserFavoriteShops(string userId)
        {

			List<ShopViewModel> shopViews =	(from s in _context.Shops
						where (from f in _context.FavoriteShops where f.UserId== userId && f.IsActive == true select f.ShopId).Contains(s.Id)
						select new ShopViewModel{ 
							Address1=s.Address1,
							Address2=s.Address2,
							City=s.City,
							ShopCode=s.ShopCode,
							ShopName=s.ShopName,
							ShopImageUrl=s.ShopImageUrl,
							ShopId=s.Id
						}).AsQueryable().ToList();
			return Task.FromResult(shopViews);

        }

        public async Task<bool> AddUserFavoriteShop(string userId, string shopId)
        {
			Shop shop = _context.Shops.Find(shopId);
            if (shop != null)
            {
				FavoriteShop favoriteShop = await _context.FavoriteShops.Where(x => x.UserId == userId && x.ShopId == shopId).FirstOrDefaultAsync();
                if (favoriteShop == null)
                {
					FavoriteShop favorite = new FavoriteShop
					{
						Id = Guid.NewGuid().ToString(),
						ShopId = shopId,
						UserId = userId,
						IsActive = true,
					};
					_context.FavoriteShops.Add(favorite);
                }
                else
                {
					favoriteShop.IsActive = true;
				}
				await _context.SaveChangesAsync();
				return true;

			}
			return false;
        }

        public async Task<bool> RemoveUserFavoriteShop(string userId, string shopId)
        {
			Shop shop = _context.Shops.Find(shopId);
			if (shop != null)
			{
				FavoriteShop favoriteShop = await _context.FavoriteShops.Where(x => x.UserId == userId && x.ShopId == shopId).FirstOrDefaultAsync();
				if (favoriteShop != null)
				{
					favoriteShop.IsActive = false;
					await _context.SaveChangesAsync();
					return true;
				}
			}
			return false;
		}
    }
	public enum EnumOrderStatus // same as OrdersStatus table
    {
		PriceHiddenByShopper = 1,
		UnderPreparing,
		ReadyForDelivery,
		Cancel,
		Delivered
	}
}
