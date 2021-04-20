using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopingAPI.BusinessLayer.Helpers;
using ShopingAPI.BusinessLayer.ViewModel;
using ShopingAPI.DataLayer.Data;
using ShopingAPI.DataLayer.Models;

namespace ShopingAPI.BusinessLayer.Repository
{
    public interface IOrderRepository
    {
        Task<string> OrderCancelByOrderSummaryId(string orderSummaryId, string userId, string userTableName);
        Task<string> OrderReadyForDeliveryByOrderSummaryId(string orderSummaryId, string userId);
        Task<string> OrderDeliveryByOrderSummaryId(string orderSummaryId, string userId, string userTableName);
        Task<IEnumerable<OrderStatus>> GetOrdersStatus();
        Task<IEnumerable<OrderSummaryViewModel>> OrdersByUserId(SearchOrdersViewModel searchOrders,string userId,string UserTableName);
        Task<IEnumerable<OrderSummaryViewModel>> OrdersByVendorId(SearchOrdersViewModel searchOrders,string userId);
        Task<string> DownLoadInvoiceByUserInvoiceId(string invoiceId,string userId, string userTableName);
        Task<string> DownLoadInvoiceByVendorInvoiceId(string invoiceId,string userId);

    }

    public class OrderRepository:Repository, IOrderRepository
    {
        public OrderRepository(DataContext _context, IMapper _mapper) : base(_context, _mapper)
        {

        }

       
        public Task<string> DownLoadInvoiceByUserInvoiceId(string invoiceId, string userId,string userTableName)
        {
            OrderSummary orderSummary = _context.OrderSummaries.FromSqlRaw($"select '' as statusname,* from UserOrders.User{userTableName}Orders where id='{invoiceId}' and userId='{userId}'").FirstOrDefault();
            if (orderSummary.OrderStatusId != (int)EnumOrderStatus.Delivered)
            {
                return Task.FromResult("Requested order not found.");
            }
            return Task.FromResult(TemplateGeneratorForInvoice.GetHTMLStringForInvoice(invoiceId, orderSummary));
        }

        public Task<string> DownLoadInvoiceByVendorInvoiceId(string invoiceId, string userId)
        {
            var shop = _context.Shops.Where(x => x.UserId == userId).FirstOrDefault();
            OrderSummary orderSummary = _context.OrderSummaries.FromSqlRaw($"select '' as statusname,* from VendorOrder.Vendor{shop.tableName}Orders where id='{invoiceId}' and shopId='{shop.Id}'").FirstOrDefault();
            if (orderSummary.OrderStatusId != (int)EnumOrderStatus.Delivered)
            {
                return Task.FromResult("Requested order not found.");
            }
            return Task.FromResult(TemplateGeneratorForInvoice.GetHTMLStringForInvoice(invoiceId, orderSummary));
        }

        public async Task<IEnumerable<OrderStatus>> GetOrdersStatus()
        {
            return await _context.OrderStatuses.ToListAsync();
                
         }

        public Task<string> OrderCancelByOrderSummaryId(string orderSummaryId, string userId, string userTableName)
        {
            string userTable = userId.Replace("-", "");
            var orderSummary = _context.OrderSummaries.FromSqlRaw($"select '' as statusname, * from UserOrders.User{userTableName}Orders where id='{orderSummaryId}' and userId='{userId}'").FirstOrDefault();
            if (orderSummary != null)
            {
                if (orderSummary.OrderStatusId == ((int)EnumOrderStatus.Delivered))
                {
                    return Task.FromResult("Order alreday Delivered.");
                }
                var shop = _context.Shops.Find(orderSummary.ShopId);
                var vendorOrderUpdate = $"update VendorOrder.Vendor{shop.tableName}Orders set OrderStatusId={(int)EnumOrderStatus.Cancel} where id='{orderSummaryId}'";
                var userOrderUpdate = $"update UserOrders.User{userTableName}Orders set OrderStatusId={(int)EnumOrderStatus.Cancel} where id='{orderSummaryId}'";
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = vendorOrderUpdate + ";" + userOrderUpdate;
                    _context.Database.OpenConnection();
                    command.ExecuteNonQuery();
                }
                return Task.FromResult("Order has been cancelled.");
            }
            else
            {
                return Task.FromResult("Requested order not found.");
            }
        }

        public Task<string> OrderDeliveryByOrderSummaryId(string orderSummaryId, string userId, string userTableName)
        {
            var shop = _context.Shops.Where(x=>x.UserId==userId).FirstOrDefault();
            var orderSummary = _context.OrderSummaries.FromSqlRaw($"select '' as statusname,* from VendorOrder.Vendor{shop.tableName}Orders where id='{orderSummaryId}' and shopId='{shop.Id}'").FirstOrDefault();
            if (orderSummary != null)
            {
                if (orderSummary.OrderStatusId == (int)EnumOrderStatus.Cancel)
                {
                    return Task.FromResult("Order has been already cancelled.");
                }
                var vendorOrderUpdate = $"update VendorOrder.Vendor{shop.tableName}Orders set OrderStatusId={(int)EnumOrderStatus.Delivered} where id='{orderSummaryId}'";
                var userOrderUpdate = $"update UserOrders.User{userTableName}Orders set OrderStatusId={(int)EnumOrderStatus.Delivered} where id='{orderSummaryId}' and userId='{userId}'";
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = vendorOrderUpdate + ";" + userOrderUpdate;
                    _context.Database.OpenConnection();
                    command.ExecuteNonQuery();
                }
                return Task.FromResult("Order has been ready for delivery.");
            }
            else
            {
                return Task.FromResult("Requested order not found.");
            }
        }

        public Task<string> OrderReadyForDeliveryByOrderSummaryId(string orderSummaryId, string userId)
        {
            var shop = _context.Shops.Where(x => x.UserId == userId).FirstOrDefault();
            var orderSummary = _context.OrderSummaries.FromSqlRaw($"select '' as statusname,* from VendorOrder.Vendor{shop.tableName}Orders where id='{orderSummaryId}' and shopId='{shop.Id}'").FirstOrDefault();
            if (orderSummary != null)
            {
                if (orderSummary.OrderStatusId == (int)EnumOrderStatus.Cancel)
                {
                    return Task.FromResult("Order has been already cancelled.");
                }
                User user=_context.Users.Find(orderSummary.UserId);
                var vendorOrderUpdate = $"update VendorOrder.Vendor{shop.tableName}Orders set OrderStatusId={(int)EnumOrderStatus.ReadyForDelivery} where id='{orderSummaryId}'";
                var userOrderUpdate = $"update UserOrders.User{user.TableName}Orders set OrderStatusId={(int)EnumOrderStatus.ReadyForDelivery} where id='{orderSummaryId}'";
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = vendorOrderUpdate + ";" + userOrderUpdate;
                    _context.Database.OpenConnection();
                    command.ExecuteNonQuery();
                }
                return Task.FromResult("Order has been Ready for delivery.");
            }else
            { 
                return Task.FromResult("Requested order not found.");
            }
        }

        public async Task<IEnumerable<OrderSummaryViewModel>> OrdersByUserId(SearchOrdersViewModel searchOrders,string userId,string UserTableName)
        {
            StringBuilder whereClose = new StringBuilder();
            int totalSkip = searchOrders.PageSize * (searchOrders.PageNumber - 1);
            if (searchOrders.InvoiceNo != null)
            {
                whereClose.Append($" o.InvoiceNo ='{searchOrders.InvoiceNo}'");
            }
            else
            {
                if(searchOrders.OrderStartDate!=null)
                whereClose.Append($" UserId='{userId}' and DATE_TRUNC('day', CreatedDateTime)>= '{searchOrders.OrderStartDate.Value.Date}' and DATE_TRUNC('day',CreatedDateTime)<='{searchOrders.OrderEndDate.Value.Date}'");
                if (searchOrders.Status != null)
                {
                    whereClose.Append($" and o.OrderStatusId ='{searchOrders.Status}'");
                }
                whereClose.Append($" order by o.CreatedDateTime desc OFFSET {totalSkip} ROWS FETCH NEXT {searchOrders.PageSize} ROWS ONLY;");

            }
            string tab = $"select o.Id,o.CreatedDateTime,o.UpdatedDateTime,'' as UserId,'' as IP,o.OrderStatusId,os.statusname,o.ItemCount,o.IsActive,o.ModifyBy,o.TotalAmount," +
                $"o.Discount,o.DeliveryCost,o.PromoCode,o.DeliveryStatus,o.DeliveryDate,o.InvoiceNo,o.PaymentStatus,o.UserAddressDetails,o.ShopAddressDetails,o.OrdersDetails,o.CurrencyType,o.ShopId,os.StatusName as OrderStatus " +
                $"from UserOrders.User{UserTableName}Orders o left join OrderStatuses os on o.OrderStatusId = os.Id " +
                $"where {whereClose}";
            var orderSummary = await _context.OrderSummaries.FromSqlRaw(tab).ToListAsync();
            List<OrderSummaryViewModel> orderSummaryList = _mapper.Map<List<OrderSummaryViewModel>>(orderSummary);
            return orderSummaryList;
        }

        public async Task<IEnumerable<OrderSummaryViewModel>> OrdersByVendorId(SearchOrdersViewModel searchOrders, string userId)
        {
            var shop = _context.Shops.Where(x=>x.UserId==userId).FirstOrDefault();
            string userTable = shop.tableName;
            
            StringBuilder whereClose = new StringBuilder();
            whereClose.Append($"where o.shopId ='{shop.Id}' ");

            int totalSkip = searchOrders.PageSize * (searchOrders.PageNumber - 1);

            if (!string.IsNullOrEmpty(searchOrders.InvoiceNo))
            {
                whereClose.Append($"and o.InvoiceNo ='{searchOrders.InvoiceNo}'");
            }
            else
            {
                if (searchOrders.OrderStartDate != null)
                    whereClose.Append($"and DATE_TRUNC('day', CreatedDateTime)>= '{searchOrders.OrderStartDate.Value.Date}' and DATE_TRUNC('day',CreatedDateTime)<='{searchOrders.OrderEndDate.Value.Date}'");
                //whereClose.Append($"convert(varchar, CreatedDateTime, 1) between convert(varchar,'{searchOrders.OrderStartDate.Value.Date}',1) and convert(varchar,'{searchOrders.OrderEndDate.Value.Date}',1)");
                if (searchOrders.Status != null)
                {
                    whereClose.Append($" and o.OrderStatusId ='{searchOrders.Status}'");
                }
                whereClose.Append($" order by o.CreatedDateTime desc OFFSET {totalSkip} ROWS FETCH NEXT {searchOrders.PageSize} ROWS ONLY;");

            }
            string tab = $"select o.Id,o.CreatedDateTime,o.UpdatedDateTime,'' as UserId,'' as IP,o.OrderStatusId,os.statusName,o.ItemCount,o.IsActive,o.ModifyBy,o.TotalAmount," +
                $"o.Discount,o.DeliveryCost,o.PromoCode,o.DeliveryStatus,o.DeliveryDate,o.InvoiceNo,o.PaymentStatus,o.UserAddressDetails,o.ShopAddressDetails,o.OrdersDetails,o.CurrencyType,o.ShopId,os.StatusName as OrderStatus " +
                $"from VendorOrder.Vendor{userTable}Orders o left join OrderStatuses os on o.OrderStatusId = os.Id " +
                $"{whereClose};";
            var orderSummary = await _context.OrderSummaries.FromSqlRaw(tab).ToListAsync();
            List<OrderSummaryViewModel> orderSummaryList = _mapper.Map<List<OrderSummaryViewModel>>(orderSummary);
            return orderSummaryList;
        }
    }
}
    