using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShopingAPI.BusinessLayer.Repository;
using ShopingAPI.BusinessLayer.ViewModel;
using ShopingAPI.DataLayer.Data;
using ShopingAPI.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShopingAPI.WebAPI.WebAPI
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrdersController : CommanCtorController
    {
        private readonly IConverter _converter;
        public OrdersController(ISingletonRepository _singleton, ILogger<CommanCtorController> logger,IConverter converter) : base(_singleton,logger)
        {
            _converter = converter;
        }
        [HttpPost]
        [Authorize(Roles =CustomRoles.User)]
        public async Task<ActionResult> AddOrderWithAddress(UserAddressViewModel userAddressViewModel)
        {
            
            string UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string orderSummaryId = string.Empty;

            try
            {
                string userTableName = await _singleton.userRepository.GetUserOrdersTableName(UserId);
               await _singleton.userRepository.AddOrderWithAddress(userAddressViewModel, UserId, userTableName, out orderSummaryId);
            }
            catch (DbUpdateException e)
            {
                throw;
            }
            return Ok(new { orderSummaryId });
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<UserAddressViewModel>> getUserAddress()
        {
            string UserId =  User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (UserId==null)
            {
                return NotFound();
            }
            var userAddress = await _singleton.userRepository.GetUserAddress(UserId);
            return userAddress;
        }

        [HttpPost]
        [Authorize(Roles =CustomRoles.User)]
        public async Task<ActionResult<IEnumerable<OrderSummaryViewModel>>> getUserOrdersDetails(SearchOrdersViewModel searchOrders)
        {
            string UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string userTableName = await _singleton.userRepository.GetUserOrdersTableName(UserId);

            try
            {
               var orderSummary= await _singleton.orderRepository.OrdersByUserId(searchOrders,UserId, userTableName);
                return (orderSummary.ToList());
            }
            catch (Exception e)
            {
                throw;
            }
        }

        [HttpGet]
        [Authorize(Roles = CustomRoles.User)]
        public async Task<ActionResult> AddUserFavoriteShop(string shopId)
        {
            string UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            bool result = await _singleton.userRepository.AddUserFavoriteShop(UserId,shopId);
            return Ok();
        }

        [HttpGet]
        [Authorize(Roles = CustomRoles.User)]
        public async Task<ActionResult> RemoveUserFavoriteShop(string shopId)
        {
            string UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            bool result = await _singleton.userRepository.RemoveUserFavoriteShop(UserId, shopId);
            return Ok();
        }

        [HttpGet]
        [Authorize(Roles = CustomRoles.User)]
        public async Task<ActionResult<IEnumerable<ShopViewModel>>> GetUserFavoriteShops()
        {
            string UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            List<ShopViewModel> shopViews = await _singleton.userRepository.GetUserFavoriteShops(UserId);
            return shopViews;
        }

        [HttpPost]
        [Authorize(Roles =CustomRoles.VendorOrEmployee)]
        public async Task<ActionResult<IEnumerable<OrderSummaryViewModel>>> GetVendorOrdersDetails(SearchOrdersViewModel searchOrders)
        {
            string UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            try
            {
                var orderSummary = await _singleton.orderRepository.OrdersByVendorId(searchOrders, UserId);
                return (orderSummary.ToList());

            }
            catch (Exception e)
            {
                throw;
            }
        }
        [HttpGet]
        [Authorize(Roles =CustomRoles.VendorOrEmployee)]
        public async Task<ActionResult<IEnumerable<OrderSummaryViewModel>>> OrderDelivery(string ordersSummaryId)
        {
            string UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            try
            {
                string userTableName = await _singleton.userRepository.GetUserOrdersTableName(UserId);
                var orderSummary = await _singleton.orderRepository.OrderDeliveryByOrderSummaryId(ordersSummaryId, UserId,userTableName);
                return Ok(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderStatus>>> GetOrdersStatus()
        {
            try
            {
                var orderStatus = await _singleton.orderRepository.GetOrdersStatus();
                return orderStatus.ToList();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        [HttpPost]
        [Authorize(Roles =CustomRoles.User)]
        public async Task<ActionResult> CancelOrderByOrderSummaryId(OrdersSummaryId orders)
        {
            string UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            try
            {
                string userTableName = await _singleton.userRepository.GetUserOrdersTableName(UserId);
                var orderSummary = await _singleton.orderRepository.OrderCancelByOrderSummaryId(orders.orderSummaryId,UserId,userTableName);
                return Ok(new { data=orderSummary });
            }
            catch (Exception e)
            {
                throw;
            }
        }
        [HttpPost]
        [Authorize(Roles =CustomRoles.VendorOrEmployee)]
        public async Task<ActionResult> ReadyForDeliveryOrderByOrderSummaryId(OrdersSummaryId orders)
        {
            string UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            try
            {
                var orderSummary = await _singleton.orderRepository.OrderReadyForDeliveryByOrderSummaryId(orders.orderSummaryId, UserId);
                return Ok(new { data = orderSummary });
            } 
            catch (Exception e)
            {
                throw;
            }
        }
        [HttpPost]
        [Authorize(Roles =CustomRoles.VendorOrEmployee)]
        public async Task<ActionResult> DeliveryByOrderSummaryId(OrdersSummaryId orders)
        {
            string UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            try
            {
                string userTableName = await _singleton.userRepository.GetUserOrdersTableName(UserId);
                var orderSummary = await _singleton.orderRepository.OrderDeliveryByOrderSummaryId(orders.orderSummaryId, UserId,userTableName);
                return Ok(new { data = orderSummary });
            }
            catch (Exception e)
            {
                throw;
            }
        }
        [HttpGet]
        [Authorize(Roles =CustomRoles.User)]
        public async Task<ActionResult> GetInvoiceByUserInvoiceId(string invoiceId)
        {
            string UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            try
            {

                string userTableName = await _singleton.userRepository.GetUserOrdersTableName(UserId);
                var invoiceHtml = await _singleton.orderRepository.DownLoadInvoiceByUserInvoiceId(invoiceId, UserId,userTableName);
                var globalSettings = new GlobalSettings {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    Margins=new MarginSettings { Top=10},
                    PaperSize = PaperKind.A4,
                };
                var objectSettings = new ObjectSettings() {
                    PagesCount = true,
                    HtmlContent = invoiceHtml,
                    HeaderSettings = {FontName="Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true, Spacing = 2.812 },
                    FooterSettings = {FontName="Arial",FontSize=9,Center="Report Header"}
                };
                    
                var pdf = new HtmlToPdfDocument()
                {
                    GlobalSettings=globalSettings,
                    Objects = {objectSettings}
                    
                        
                };
                var file = _converter.Convert(pdf);
                return File(file,"applicatio/pdf","Invoice.pdf");
            }
            catch (Exception e)
            {
                throw;
            }
        }
        [HttpGet]
        [Authorize(Roles =CustomRoles.VendorOrEmployee)]
        public async Task<ActionResult> GetInvoiceByVendorInvoiceId(string invoiceId)
        {
            string UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            try
            {

                var invoiceHtml = await _singleton.orderRepository.DownLoadInvoiceByVendorInvoiceId(invoiceId, UserId);
                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    Margins = new MarginSettings { Top = 10 },
                    PaperSize = PaperKind.A4,
                };
                var objectSettings = new ObjectSettings()
                {
                    PagesCount = true,
                    HtmlContent = invoiceHtml,
                    HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true, Spacing = 2.812 },
                    FooterSettings = { FontName = "Arial", FontSize = 9, Center = "Report Header" }
                };

                var pdf = new HtmlToPdfDocument()
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }


                };
                var file = _converter.Convert(pdf);
                return File(file, "applicatio/pdf", "Invoice.pdf");
            }
            catch (Exception e)
            {
                throw;
            }
        }

    }
   public class OrdersSummaryId
    {
        public string orderSummaryId { get; set; }
    }
}