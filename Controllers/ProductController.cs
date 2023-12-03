//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.Extensions.Configuration;
//using System.Data;
//using Bitirme.Models;

//namespace Bitirme.Controllers
//{
//    public class ProductController : Controller
//    {
//        private readonly IConfiguration _configuration;

//        public ProductController(IConfiguration configuration)
//        {
//            _configuration = configuration;
//        }

//        public IActionResult Index()
//        {
//            var productList = new List<SelectListItem>();

//            using (var connection = new Microsoft.Data.SqlClient.SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
//            {
//                connection.Open();
//                string query = "SELECT * FROM tbl_products";
//                using (Microsoft.Data.SqlClient.SqlCommand command = new Microsoft.Data.SqlClient.SqlCommand(query, connection))
//                {
//                    using (Microsoft.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
//                    {
//                        while (reader.Read())
//                        {
//                            var product = new Explore
//                            {
//                                // ProductModel sınıfını oluşturduğunuzdan emin olun ve aşağıdaki alanları düzenleyin
//                                ProductId = reader.GetInt32(reader.GetOrdinal("Id")),
//                                ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
//                                Country = reader.GetString(reader.GetOrdinal("Country")),
//                                City = reader.GetString(reader.GetOrdinal("City")),
//                                Address = reader.GetString(reader.GetOrdinal("Address")),
//                                ProductKg = reader.GetString(reader.GetOrdinal("ProductKg")),
//                                Note = reader.GetString(reader.GetOrdinal("Note"))
//                            };
//                            productList.Add(product);
//                        }
//                    }
//                }
//            }

//            return View(productList);
//        }
//    }
//}