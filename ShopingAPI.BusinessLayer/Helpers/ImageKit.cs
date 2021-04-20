using Imagekit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ShopingAPI.BusinessLayer.Helpers
{
    public class ImageKitClass
    {
        private readonly ServerImagekit imagekit;
        public ImageKitClass()
        {
            imagekit = new ServerImagekit("public_0nkC4AqsdxiHI1aZLphtt1H7d0g=", "private_kXENeAklGkYf46xZegNcJScwxLM=", "https://ik.imagekit.io/en9grhpyzhz/", "path");
        }

        public async Task<string[]> GetProductUri(string url)
        {
         List<string> urlsresult =new List<string>();
            if (!string.IsNullOrEmpty(url))
            {
                string[] urls = url.ToString().TrimEnd(',').Split(',');


                foreach (var item in urls)
                {
                    try
                    {
                        string[] fileName = item.Split("en9grhpyzhz");
                        string imgUrl = imagekit.Url(new Transformation()).Path(fileName[1]).Signed(true).ExpireSeconds(200000).Generate();
                        urlsresult.Add(imgUrl);
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                }
                return await Task.FromResult(urlsresult.ToArray());
            }
            urlsresult.Add("assets/images/no-product-image.jpg");
            urlsresult.Add("assets/images/no-product-image.jpg");
            return await Task.FromResult(urlsresult.ToArray());
        }

        public async Task<string[]> GetProductThumbnailUri(string url)
        {
            List<string> urlsresult = new List<string>();
            if (!string.IsNullOrEmpty(url))
            {
                string[] urls = url.ToString().TrimEnd(',').Split(',');
                    try
                    {
                        urlsresult.Add(urls[1]);
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                return await Task.FromResult(urlsresult.ToArray());
            }
            urlsresult.Add("assets/images/no-product-image.jpg");
            return await Task.FromResult(urlsresult.ToArray());
        }
    }
}
