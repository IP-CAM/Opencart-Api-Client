#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace OpenCardApiClient
{
    class Program
    {
        //private static string BASE_URL = "https://localhost/";
        private static string BASE_URL = "https://lift-store.ir/";
        //private static string key = "NjXcqVVthcBo1wvqhAaAY7tfZDUmM3tOQ7aOKdPfllsxGp8HLQSG4WTZHbRyLoxV6GziUktVq5gd3ivC2qNpMVnirHlRib5oEDE9FwqSbfx15swQkjTOJLVjhjwOOD3n2OSVjHkssh7mCan5Qg4k5h3Mbl4pN2xvrwHhNl4nToRf8CM8ib9ToLKF6L0muTb5bMHw7mVeYz1XFTiNKm1ejDaZnwnMzbkbNr5x2Sg3uMzOzac3hTQGydJ24In1RX3n";
        private static string key = "vrNHX91XzHPKrswdgyY70OgHRpFAxFjb68B2ILcCiew8sjJu4GNvdGKn2cgyQFXcqadqrzz8cS77sJOGG8zcdv0pK6ZRalWbeRVPh9IGFcdZelu8jFv3D7qbj9N4m8E06go8Jpzxzb4Ukqu756ElVmH7mqHZuAshMAjux58r7xKG6axRWpwdSwsGomgnMyRHTnP5qbyoqf3x5YRMEhQuN9TWgyCJr0e2tBp7vQmXdQWVu8TiZkUgIbhSxIeBbWmp";
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

                Console.WriteLine($"\n\nSelect Operation:\n1.Get Products\n2.Get Customers\n3.Get Order Statuses\n4.Get Orders\n5.Update Product Price And Quantity\n6.Exit\n");
                var i = Console.ReadLine();
                if (int.TryParse(i, out var index))
                {
                    switch (index)
                    {
                        case 1:
                            await GetProductsFromHatraApiAsync(http, token);
                            break;
                        case 2:
                            await GetCustomersFromHatraApiAsync(http, token);
                            break;
                        case 3:
                            await GetOrderStatusesFromHatraApiAsync(http, token);
                            break;
                        case 4:
                            await GetOrdersFromHatraApiAsync(http, token);
                            break;
                        case 5:
                            await UpdateProductsAsync(http, token);
                            break;
                        case 6:
                            return;
                        default:
                            Console.WriteLine("\nInvalid Operation... Try Again!");
                            break;
                    }
                }


            }
        }


        private static async Task UpdateProductsAsync(HttpClient http, string apiToken)
        {
            var route = $"{BASE_URL}index.php?route=api/hatra/updateProduct&token={apiToken}";
            var products = new ProductViewModel[]
            {
                new ProductViewModel { Id= 28,Price=232323,Quantity=2522222  },
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
        private static async Task GetCustomersFromHatraApiAsync(HttpClient http, string apiToken)
        {
            var route = $"{BASE_URL}index.php?route=api/hatra/getcustomers&start=0&limit=10&token={apiToken}";
            var response = await http.GetAsync(route);
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine("customers by hatra api :\n" + JToken.Parse(content).ToString(Formatting.Indented) + "\n\n\n");
        }
        private static async Task GetOrderStatusesFromHatraApiAsync(HttpClient http, string apiToken)
        {
            var route = $"{BASE_URL}index.php?route=api/hatra/getorderstatuses&token={apiToken}";
            var response = await http.GetAsync(route);
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine("order statuses by hatra api :\n" + JToken.Parse(content).ToString(Formatting.Indented) + "\n\n\n");
        }
        private static async Task GetOrdersFromHatraApiAsync(HttpClient http, string apiToken)
        {
            var route = $"{BASE_URL}index.php?route=api/hatra/getorders&start=0&limit=10&token={apiToken}"; /*&status = 1*/
            var response = await http.GetAsync(route);
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine("orders by hatra api :\n" + JToken.Parse(content).ToString(Formatting.Indented) + "\n\n\n");
        }
        public static async Task<string> LoginAndGetTokenAsync(HttpClient http)
        {
            var loginUrl = $"{BASE_URL}index.php?route=api/hatra/login";

            var username = "Default";
            var response = await http.PostAsync(loginUrl, new FormUrlEncodedContent(new[] { new KeyValuePair<string?, string?>("key", key), new KeyValuePair<string?, string?>("username", username) }));
            var content = await response.Content.ReadAsStringAsync();
            var token = JsonSerializer.Deserialize<LoginResponseModel>(content)?.token;

            Console.WriteLine("\nlogged in successfully. token : " + token + "\n");

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
