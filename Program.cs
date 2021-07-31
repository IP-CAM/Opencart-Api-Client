#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace OpenCardApiClient
{
    class Program
    {
        private static string BASE_URL = "https://localhost/";
        //private static string BASE_URL = "https://lift-store.ir/";
        private static string key = "d02RIcRfgB3IlbpBcv5zOIvyqiY6krWY2Drq1ZZIeMCbuGkTnux2s2xorPt0aPf8wNNJv8Pq7KlSOiXt7k7xdOXuzlvEP6iaMaPPi6hOJHwuW0D89McNhPIkS5w3SbBLAFTw2DXju3Pp2uV8h6GZ85MBMtEu2W4lavS1WFfCqPgKzeWE0HbWGFOUyzAWeEs4Wqg9s2cpbim9OZTU8XIfBYsPMEwzG6HqBcehFOkFR8lGWUbW7G5vIe5OdCFHinO6";
        //private static string key ="0Duv2o5u0YydDHTpYTWWWugyONwFsfPYCyDa69HmsBrKRarC86lx8mUaD44eDjykHvLpqOujqR3nXMjDuQgyVVC0yBGqCioDarIG5J05zlFcT3N8P6SYC6Whcx7aRO3pwvNVqmubRj844fUirY8szc1rQobr2Pww65tk2lFyJbWTZvSGTd7lrQGZnbjAb1GiGiOzTseCApaBiNeeB5cSyZMbXxCnXR4bGurBmphufm4MVDoFCE6ZTbDecmA3mtRO";
        static async Task Main(string[] args)
        {
            Console.WriteLine("OpenCart Api Client");

            var http = new HttpClient(new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (a, b, c, d) => true
            });

            var token = await LoginAndGetTokenAsync(http);

            while (true)
            {
                await UpdateProductsAsync(http, token);

                Console.ReadKey();
            }
            //await GetProductsAsync(http, token);
            //await GetCustomersAsync(http, token);
            //Console.ReadKey();
        }


        private static async Task UpdateProductsAsync(HttpClient http, string apiToken)
        {
            var route = $"{BASE_URL}index.php?route=api/hatra/updateProduct&token={apiToken}";
            var products = new ProductViewModel[]
            {
                new ProductViewModel { Id= 28,Price=10000,Quantity=25  },
                new ProductViewModel{ Id=12,Price=20000,Quantity=250  },
                new ProductViewModel{ Id=123,Price=30000,Quantity=2500  },
            };
            var props = typeof(ProductViewModel).GetProperties().ToList();
            var result = new List<UpdateProductResponseModel?>();
            foreach (var product in products)
            {
                try
                {
                    var data = props.Select(x =>
                        new KeyValuePair<string, string>(x.Name.ToLower(), x.GetValue(product)?.ToString() ?? string.Empty));

                    var response = await http.PostAsync(route, new FormUrlEncodedContent(data));
                    var content = await response.Content.ReadAsStringAsync();
                    result.Add(JsonSerializer.Deserialize<UpdateProductResponseModel>(content));
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error On Updating Product :" + product.Id + Environment.NewLine + e);
                }
            }

            result.ForEach(Console.WriteLine);
        }
        private static async Task GetProductsFromHatraApiAsync(HttpClient http, string apiToken)
        {
            var route = $"{BASE_URL}index.php?route=api/hatra/getproducts&start=0&limit=10&token={apiToken}";
            var response = await http.GetAsync(route);
            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine("products by hatra api :\n" + JToken.Parse(content).ToString(Formatting.Indented) + "\n\n\n");
        }

        //private static async Task GetProductsAsync(HttpClient http, string apiToken)
        //{
        //    var route = $"{BASE_URL}index.php?route=api/cart/products&token={apiToken}";
        //    var response = await http.GetAsync(route);
        //    var content = await response.Content.ReadAsStringAsync();
        //    Console.WriteLine("products :\n" + JsonConvert.SerializeObject(content, Formatting.None) + "\n\n\n");
        //}
        //private static async Task GetCustomersAsync(HttpClient http, string apiToken)
        //{
        //    var route = $"{BASE_URL}index.php?route=api/customer&token={apiToken}";
        //    var response = await http.GetAsync(route);
        //    var content = await response.Content.ReadAsStringAsync();

        //    Console.WriteLine("customers :\n"+content + "\n\n\n");
        //}

        public static async Task<string> LoginAndGetTokenAsync(HttpClient http)
        {
            var loginUrl = $"{BASE_URL}index.php?route=api/hatra/login";

            var username = "Default";
            var response = await http.PostAsync(loginUrl, new FormUrlEncodedContent(new[] { new KeyValuePair<string?, string?>("key", key), new KeyValuePair<string?, string?>("username", username) }));
            var content = await response.Content.ReadAsStringAsync();
            var token = JsonSerializer.Deserialize<LoginResponseModel>(content)?.token;

            Console.WriteLine("token : " + token + "\n\n\n");

            return token;
        }
    }


    public class LoginResponseModel
    {
        public string success { get; set; }
        public string token { get; set; }
    }
    public class UpdateProductResponseModel
    {
        public bool success { get; set; }
        public string error { get; set; }


        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
    public class ProductViewModel
    {
        public int Id { get; set; }
        public long Price { get; set; }
        public int Quantity { get; set; }
    }
}
