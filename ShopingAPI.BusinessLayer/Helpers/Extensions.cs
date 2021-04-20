using Microsoft.AspNetCore.Http;

namespace ShopingAPI.BusinessLayer.Helpers
{
    public static class Extensions
    {
        public static void AddApplicationError(this HttpResponse response,string message)
        {
            response.Headers.Add("Application-Error",message);
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin","*");
        }
    }
    public class AppSettings
    {
        public string Secret { get; set; }
        public int Minutes { get; set; }
        public string Message { get; set; }
        public string BlobConnection { get; set; }
        public string CommingSoonImageUrl { get; set; }
        public string ShopCode { get; set; }
    }
}
