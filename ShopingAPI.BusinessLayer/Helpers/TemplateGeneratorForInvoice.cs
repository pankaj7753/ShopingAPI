using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopingAPI.BusinessLayer.ViewModel;
using ShopingAPI.DataLayer.Models;

namespace ShopingAPI.BusinessLayer.Helpers
{
    public static class TemplateGeneratorForInvoice
    {
        public static string GetHTMLStringForInvoice(string invoiceId, OrderSummary orderSummary)
        {

            AddressViewModel shopAddress = JsonConvert.DeserializeObject<AddressViewModel>(orderSummary.ShopAddressDetails);
            AddressViewModel userAddress = JsonConvert.DeserializeObject<AddressViewModel>(orderSummary.UserAddressDetails);
            List<VendorProductViewModel> products = JsonConvert.DeserializeObject<List<VendorProductViewModel>>(orderSummary.OrdersDetails);
            string currencyType = string.Empty;
            if (orderSummary.CurrencyType == "₹")
            {
                currencyType = "&#x20b9";
            }
            else
            {
                currencyType = orderSummary.CurrencyType;
            }
            var sb = new StringBuilder();
            sb.Append(@"<table border='0' cellpadding='1' cellspacing='1' style='width: 100%'>
    <tbody>
        <tr>
            <td>
                <div>
                    <span style='font-size:14px;'><a class='pt-2 d-inline-block' data-abc='true' href='https://bbbootstrap.com/snippets/index.html' style='box-sizing: border-box; color: rgb(0, 123, 255); text-decoration-line: none; background-color: transparent; font-family: -apple-system, BlinkMacSystemFont, &quot;Segoe UI&quot;, Roboto, &quot;Helvetica Neue&quot;, Arial, sans-serif, &quot;Apple Color Emoji&quot;, &quot;Segoe UI Emoji&quot;, &quot;Segoe UI Symbol&quot;; font-size: 16px; display: inline-block !important; padding-top: 0.5rem !important;'>BBBootstrap.com</a></span>
                </div>
            </td>
            <td style='width: 30%;'>
                <div class='mb-0' style='box-sizing: border-box; margin-top: 0px; font-family: -apple-system, BlinkMacSystemFont, &quot;Segoe UI&quot;, Roboto, &quot;Helvetica Neue&quot;, Arial, sans-serif, &quot;Apple Color Emoji&quot;, &quot;Segoe UI Emoji&quot;, &quot;Segoe UI Symbol&quot;; font-weight: 500; line-height: 1.2; color: rgb(33, 37, 41); font-size: 20px; margin-bottom: 0px !important;'>
                    <span style='font-size:14px;'>Invoice #");
            sb.Append(orderSummary.InvoiceNo);
            sb.Append(@"</span>
                </div>
                <div class='mb-0' style='box-sizing: border-box; margin-top: 0px; font-family: -apple-system, BlinkMacSystemFont, &quot;Segoe UI&quot;, Roboto, &quot;Helvetica Neue&quot;, Arial, sans-serif, &quot;Apple Color Emoji&quot;, &quot;Segoe UI Emoji&quot;, &quot;Segoe UI Symbol&quot;; font-weight: 500; line-height: 1.2; color: rgb(33, 37, 41); font-size: 20px; margin-bottom: 0px !important;'>
                    <span style='font-size:14px;'>Date: ");
            sb.Append(orderSummary.UpdatedDateTime.ToString("dd MMM,yyyy"));
            sb.Append(@"</span>
                </div>
            </td>
        </tr>
    </tbody>
</table>
<hr />
<table border='0' cellpadding='1' cellspacing='1' style='width: 100%'>
    <tbody>
        <tr>
            <td style='width: 60%;'>
                <div class='mb-3' style='box-sizing: border-box; margin-top: 0px; font-family: &quot;Circular Std Medium&quot;; font-weight: 500; line-height: 26px; color: rgb(61, 64, 92); font-size: 15px; margin-right: 0px; margin-left: 0px; margin-bottom: 1rem !important;'>
                    <span style='font-size:14px;'><strong>From:</strong></span>
                </div>");

            sb.Append(@"<div>");
            sb.Append(@"<div><span style='font-size:14px;'><strong>");
            sb.Append(shopAddress.ShopCode);//Shop Name
            sb.Append(", ");//Shop Name
            sb.Append(shopAddress.ShopName);//Shop Name
            sb.Append(@"</strong></span></div>");
            sb.Append(@"<div><span style='font-size:14px;'>");
            sb.Append(shopAddress.Address1); //Address 1
            sb.Append(@"</span></div>");

            sb.Append(@"<div><span style='font-size:14px;'>");
            sb.Append(shopAddress.Address2); //Address 2
            sb.Append(@"</span></div>");

            sb.Append(@"<div><span style='font-size:14px;'>Email: ");
            sb.Append(shopAddress.Email); //Email 1
            sb.Append(@"</span></div>");

            sb.Append(@"<div><span style='font-size:14px;'>Phone: ");
            sb.Append(shopAddress.MobileNumber); //Mobile 1
            sb.Append(@"</span></div>");

            sb.Append(@"</div>");
            sb.Append(@"</td>");
            sb.Append(@"<td style='width: 50%;'>
                <div class='mb-3' style='box-sizing: border-box; margin-top: 0px; font-family: &quot;Circular Std Medium&quot;; font-weight: 500; line-height: 26px; color: rgb(61, 64, 92); font-size: 15px; margin-right: 0px; margin-left: 0px; margin-bottom: 1rem !important;'>
                    <span style='font-size:14px;'><strong>To:</strong></span>
                </div>");
            sb.Append(@"<div><span style='font-size:14px;'><strong>");
            sb.Append(userAddress.FullName);//Shop Name
            sb.Append(@"</strong></span></div>");
            sb.Append(@"<div><span style='font-size:14px;'>");
            sb.Append(userAddress.Address1); //Address 1
            sb.Append(@"</span></div>");

            sb.Append(@"<div><span style='font-size:14px;'>");
            sb.Append(userAddress.Address2); //Address 2
            sb.Append(@"</span></div>");

            sb.Append(@"<div><span style='font-size:14px;'>Email: ");
            sb.Append(userAddress.Email); //Email 1
            sb.Append(@"</span></div>");

            sb.Append(@"<div><span style='font-size:14px;'>Phone: ");
            sb.Append(userAddress.MobileNumber); //Mobile 1
            sb.Append(@"</span></div>");
            sb.Append(@"</td>
        </tr>
    </tbody>
</table>
<hr />
<table align='center' border='0' cellpadding='1' cellspacing='1' style='width: 100%'>
    <thead>
        <tr>
            <th scope='col' style='text-align: center; width: 7%;'>
                <div>
                    <span style='font-size:14px;'>Sl.No</span>
                </div>
            </th>
            <th scope='col' style='text-align: left;'>
                <div>
                    <span style='font-size:14px;'><span style='color: rgb(33, 37, 41); font-family: -apple-system, BlinkMacSystemFont, &quot;Segoe UI&quot;, Roboto, &quot;Helvetica Neue&quot;, Arial, sans-serif, &quot;Apple Color Emoji&quot;, &quot;Segoe UI Emoji&quot;, &quot;Segoe UI Symbol&quot;;'>Item</span></span>
                </div>
            </th>
            <th scope='col' style='width: 15%; text-align: left;'>
                <div>
                    <span style='font-size:14px;'><strong><span style='color: rgb(33, 37, 41); font-family: -apple-system, BlinkMacSystemFont, &quot;Segoe UI&quot;, Roboto, &quot;Helvetica Neue&quot;, Arial, sans-serif, &quot;Apple Color Emoji&quot;, &quot;Segoe UI Emoji&quot;, &quot;Segoe UI Symbol&quot;;'>Price</span></strong></span>
                </div>
            </th>
            <th scope='col' style='width: 8%; text-align: left;'>
                <div>
                    <span style='font-size:14px;'><span style='color: rgb(33, 37, 41); font-family: -apple-system, BlinkMacSystemFont, &quot;Segoe UI&quot;, Roboto, &quot;Helvetica Neue&quot;, Arial, sans-serif, &quot;Apple Color Emoji&quot;, &quot;Segoe UI Emoji&quot;, &quot;Segoe UI Symbol&quot;;'>Qty</span></span>
                </div>
            </th>
            <th scope='col' style='width: 20%; text-align: left;'>
                <div>
                    <span style='font-size:14px;'><span style='color: rgb(33, 37, 41); font-family: -apple-system, BlinkMacSystemFont, &quot;Segoe UI&quot;, Roboto, &quot;Helvetica Neue&quot;, Arial, sans-serif, &quot;Apple Color Emoji&quot;, &quot;Segoe UI Emoji&quot;, &quot;Segoe UI Symbol&quot;;'>Sub Total</span></span>
                </div>
            </th>
        </tr>
    </thead>
    <tbody>");
            #region Forearch for items
            int count = 1;
            foreach (var item in products)
            {
                sb.Append(@"<tr>");
                sb.Append(@"<td style='text-align: center;'><div><span style='font-size:14px;'>");
                sb.Append(count);
                sb.Append(@"</span></div></td>");
                sb.Append(@"<td><div><span style='font-size:14px;'>");
                sb.Append(item.ProductName);
                sb.Append(@"</span></div></td>");
                sb.Append(@"<td><div><span style='font-size:14px;'>");
                sb.Append(currencyType);
                sb.Append(" ");
                sb.Append(item.Selling);
                sb.Append(@"</span></div></td>");
                sb.Append(@"<td><div><span style='font-size:14px;'>");
                sb.Append(item.ItemCount);
                sb.Append(@"</span></div></td>");
                sb.Append(@"<td><div><span style='font-size:14px;'>");
                sb.Append(currencyType);
                sb.Append(" ");
                sb.Append(item.SubTotalOnSelling);
                sb.Append(@"</span></div></td>");
                sb.Append(@"</tr>");
                count++;
            }
            #endregion

            sb.Append(@"</tbody>
</table>
<hr />");

            #region Total Item and Total Amount -- table-- tbody

            sb.Append(@"<table align='right' border='0' cellpadding='1' cellspacing='1' style='width: 100%'>");
            sb.Append(@"<tbody>
                    <tr>
                        <td>
                            <div>
                                &nbsp;
                            </div>
                        </td>
                        <td style='width: 43%;'>
                            <div>
                                <span style='font-size:14px;'><strong>Total Qty: ");
            sb.Append(orderSummary.ItemCount);
            sb.Append(@"</strong></span>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div>
                            &nbsp;
                        </div>
                    </td>
                    <td>
                        <div>
                            <span style='font-size:14px;'><strong>Grand Total Amount: ");
            sb.Append(currencyType);
            sb.Append(" ");
            sb.Append(orderSummary.TotalAmount);
            sb.Append(@"</strong></span>
                        </div>
                    </td>
                </tr>
            </tbody>");
            sb.Append(@"</table>");
            #endregion
            sb.Append(@"<p>&nbsp;</p>");

            return sb.ToString();
        }
    }
}
