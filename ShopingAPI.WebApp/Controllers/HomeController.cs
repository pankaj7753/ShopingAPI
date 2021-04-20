using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShopingAPI.DataLayer.Models;

namespace ShopingAPI.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConverter _converter;

        public HomeController(ILogger<HomeController> logger, IConverter converter)
        {
            _logger = logger;
            _converter = converter;

        }

        public IActionResult Index()
        {

            return View();
        }

        public IActionResult Privacy()
        {
            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                Margins = new MarginSettings { Top = 10 },
                PaperSize = PaperKind.A4,
                DocumentTitle="PDF Report"
            };
            var objectSettings = new ObjectSettings()
            {
                PagesCount = true,
                //HtmlContent = TemplateGeneratorForInvoice.GetHTMLString(),
                Page="https://code-maze.com",
                WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets", "invoiceStyleSheet.css") },
                HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true, Spacing = 2.812 },
                FooterSettings = { FontName = "Arial", FontSize = 9, Center = "Report Header" }
            };

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }


            };
            var file = _converter.Convert(pdf);
            return File(file, "applicatio/pdf","Invoice.pdf");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
