using ClosedXML.Excel;
using GemBox.Document;
using GymEShopAdminApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GymEShopAdminApplication.Controllers
{
    public class OrderController : Controller
    {

        public OrderController()
        {
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");
        }
        public IActionResult Index()
        {
            HttpClient client = new HttpClient();

            string URL = "https://localhost:44355/api/Admin/GetAllActiveOrders";

            HttpResponseMessage response = client.GetAsync(URL).Result;

            var data = response.Content.ReadAsAsync<List<Order>>().Result;

            return View(data);
        }

        public IActionResult Details(Guid Id)
        {
            HttpClient client = new HttpClient();

            string URL = "https://localhost:44355/api/Admin/GetOrderDetails";

            var model = new
            {
                Id = Id
            };

            HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync(URL, content).Result;

            var data = response.Content.ReadAsAsync<Order>().Result;

            return View(data);
        }

        public FileContentResult CreateInvoice(Guid Id)
        {
            HttpClient client = new HttpClient();

            string URL = "https://localhost:44355/api/Admin/GetOrderDetails";

            var model = new
            {
                Id = Id
            };

            HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync(URL, content).Result;

            var result = response.Content.ReadAsAsync<Order>().Result;
            var template = Path.Combine(Directory.GetCurrentDirectory(), "CreatedInvoice.docx");
            var wordDocument = DocumentModel.Load(template);


            wordDocument.Content.Replace("{{OrderNum}}", result.Id.ToString());
            wordDocument.Content.Replace("{{UserName}}", result.User.UserName);

            StringBuilder sb = new StringBuilder();

            var total = 0.0;
            foreach (var item in result.Products)
            {
                sb.AppendLine(item.SelectedProduct.ProductName + " with quantity " + item.Quantity + " and with price" + item.SelectedProduct.Price + "$");
                total = item.Quantity * item.SelectedProduct.Price;
            }
            wordDocument.Content.Replace("{{AllProducts}}", sb.ToString());
            wordDocument.Content.Replace("{{TotalPrice}}", total.ToString());

            var stream = new MemoryStream();
            wordDocument.Save(stream, new PdfSaveOptions());

            return File(stream.ToArray(), new PdfSaveOptions().ContentType, "ExportedInvoice.pdf");
        }

        [HttpGet]
        public FileContentResult ExportAllOrders()
        {
            string fileName = "Orders.xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            using (var workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet = workbook.Worksheets.Add("All Orders");

                worksheet.Cell(1, 1).Value = "Order Id";
                worksheet.Cell(1, 2).Value = "Custumer Email";


                HttpClient client = new HttpClient();


                string URI = "https://localhost:44309/api/Admin/GetOrders";

                HttpResponseMessage responseMessage = client.GetAsync(URI).Result;

                var result = responseMessage.Content.ReadAsAsync<List<Order>>().Result;

                for (int i = 1; i <= result.Count(); i++)
                {
                    var item = result[i - 1];

                    worksheet.Cell(i + 1, 1).Value = item.Id.ToString();
                    worksheet.Cell(i + 1, 2).Value = item.User.Email;

                    for (int p = 0; p < item.Products.Count(); p++)
                    {
                        worksheet.Cell(1, p + 3).Value = "Product-" + (p + 1);
                        worksheet.Cell(i + 1, p + 3).Value = item.Products.ElementAt(p).SelectedProduct.ProductName;
                    }
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(content, contentType, fileName);
                }

            }
        }
    }
}
