using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopingAPI.DataLayer.Data;
using ShopingAPI.DataLayer;
using ShopingAPI.BusinessLayer.ViewModel;
using ShopingAPI.DataLayer.Models;

namespace ShopingAPI.WebAPI.WebAPI
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrdersOldController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public OrdersOldController(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;

        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders.ToListAsync();
        }

        
        /// <summary>
        /// Cantain user address and orders cqrt item detals
        /// It will save caet item in orders summary and Orders tables
        /// </summary>
        /// <param name="userAddressViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> AddOrderWithAddress(UserAddressViewModel userAddressViewModel)
        {
            string UserId = "b3601766-eb7e-42dc-8711-1926aa3232d9";
            OrderSummary orderSummary = new OrderSummary();

            try
            {
                decimal TotalSellingPrice = 0;

                string ipAddress = "";
                if (Dns.GetHostAddresses(Dns.GetHostName()).Length > 0)
                {
                    ipAddress = Dns.GetHostAddresses(Dns.GetHostName())[0].ToString();
                }
                orderSummary.IP = ipAddress;
                orderSummary.Id = Guid.NewGuid().ToString();
                orderSummary.UpdatedDateTime = DateTime.Now;
                orderSummary.CreatedDateTime = DateTime.Now;
                orderSummary.OrderStatus = "Completed";
                orderSummary.UserId = UserId;
                orderSummary.InvoiceNo = "PK" + (_context.OrderSummaries.Count()+1).ToString("000000");
                //Add or Update userAddress
               var updateUserAddress = _context.UserAddresses.Where(x => x.UserId == UserId).FirstOrDefault();
                UserAddress userAddress = _mapper.Map<UserAddress>(userAddressViewModel);
                userAddress.UserId = UserId;
                
                if (updateUserAddress != null)
                {
                    var updatedEntity = _mapper.Map(userAddressViewModel, updateUserAddress);
                        updatedEntity.Id = updateUserAddress.Id;
                        updatedEntity.UserId = UserId;
                    userAddress.Id= updateUserAddress.Id;
                    var entity = _context.Update(updatedEntity);
                }
                else
                {
                    userAddress.Id = Guid.NewGuid().ToString();
                    _context.UserAddresses.Add(userAddress);

                }
                orderSummary.UserAddressId = userAddress.Id;
                List<string> ppid = new List<string>();
                ppid = userAddressViewModel.cartIds.Select(x => x.id).ToList();
                var result = await _context.ProductPrices.Where(x => ppid.Contains(x.Id) && x.IsActive == true).Include(p => p.Product).Include(p => p.Unit).ToListAsync();
                List<Order> orders = new List<Order>();
                foreach (var item in result)
                {
                    var quantity = userAddressViewModel.cartIds.Where(x => x.id == item.Id).FirstOrDefault();
                    Order order = new Order();
                    order = _mapper.Map<Order>(item);
                    order.ProductPriceId = order.Id;
                    order.ProductId = item.ProductId;
                    order.Quantity = quantity.count;
                    order.Id= Guid.NewGuid().ToString();
                    order.OrderSummaryId = orderSummary.Id; 

                    orderSummary.TotalAmount =Convert.ToDecimal(orderSummary.TotalAmount) + item.SellingPrice * Convert.ToInt32(quantity.count);
                    TotalSellingPrice = Convert.ToDecimal(TotalSellingPrice) + item.MRPPrice * Convert.ToInt32(quantity.count);
                    orders.Add(order);
                }
                orderSummary.Discount = TotalSellingPrice - orderSummary.TotalAmount;
                orderSummary.Delivery = 10;
            
                _context.OrderSummaries.Add(orderSummary);
                _context.Orders.AddRange(orders);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                    throw;
            }
            return Ok(new { orderSummary.Id });
        }



        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<UserAddressViewModel>> getUserAddress()
        {
            string UserId = "b3601766-eb7e-42dc-8711-1926aa3232d9";

            var userAddress = await  _context.UserAddresses.Where(x => x.UserId == UserId).FirstOrDefaultAsync();
            UserAddressViewModel userAddressView = _mapper.Map<UserAddressViewModel>(userAddress);
            return userAddressView;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderSummaryViewModel>>> getUserOrdersDetails()
        {
            string UserId = "b3601766-eb7e-42dc-8711-1926aa3232d9";

            try
            {
                var orderSummary = await _context.OrderSummaries.Where(x => x.UserId == UserId).
                    Select(x=>new OrderSummaryViewModel
                    {
                        OrderSummaryId = x.Id,
                        OrderDate=x.CreatedDateTime,
                        OrderStatus = x.OrderStatus,
                        TotalAmount = x.TotalAmount,
                        Discount = x.Discount,
                        DeliveryAmount=x.Delivery,
                        UserAddressId = x.UserAddressId,
                        ItemQuantity=x.Orders.Count,
                        InvoiceNo=x.InvoiceNo,
                        DeliveryDate=x.DeliveryDate

                        //Orders = x.Orders.Select(y=>new
                        //{
                        //    OrderId=y.Id,
                        //    ProductName = y.Product.ProductName,
                        //    ProductId = y.ProductId,
                        //    ProductImageUrl = y.Product.ImageUrl,
                        //})

                    }).OrderByDescending(x=>x.OrderDate).ToListAsync();

                return (orderSummary.ToList());

            }
            catch (Exception e)
            {

                throw;
            }
        }
    }
}
