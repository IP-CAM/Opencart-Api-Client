#nullable enable
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenCardApiClient
{
    class Program
    {
        private static string BASE_URL = "https://lift-store.ir/";
        static async Task Main(string[] args)
        {
            Console.WriteLine("OpenCart Api Client");

            var http = new HttpClient(new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (a, b, c, d) => true
            });

            var token = await LoginAndGetTokenAsync(http);

            await GetProductsFromHatraApiAsync(http, token);
            await GetProductsAsync(http, token);
            await GetCustomersAsync(http, token);

            Console.ReadKey();
        }

        private static async Task GetProductsFromHatraApiAsync(HttpClient http, string apiToken)
        {
            var route = $"{BASE_URL}index.php?route=api/hatra/getproducts&start=0&limit=10&token={apiToken}";
            var response = await http.GetAsync(route);
            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine("products by hatra api :\n" + content + "\n\n\n");
        }

        private static async Task GetProductsAsync(HttpClient http, string apiToken)
        {
            var route = $"{BASE_URL}index.php?route=api/cart/products&token={apiToken}";
            var response = await http.GetAsync(route);
            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine("products :\n" + content + "\n\n\n");
        }
        private static async Task GetCustomersAsync(HttpClient http, string apiToken)
        {
            var route = $"{BASE_URL}index.php?route=api/customer&token={apiToken}";
            var response = await http.GetAsync(route);
            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine("customers :\n"+content + "\n\n\n");
        }

        public static async Task<string> LoginAndGetTokenAsync(HttpClient http)
        {
            var loginUrl = $"{BASE_URL}index.php?route=api/hatra/login";
            var key =
                "0Duv2o5u0YydDHTpYTWWWugyONwFsfPYCyDa69HmsBrKRarC86lx8mUaD44eDjykHvLpqOujqR3nXMjDuQgyVVC0yBGqCioDarIG5J05zlFcT3N8P6SYC6Whcx7aRO3pwvNVqmubRj844fUirY8szc1rQobr2Pww65tk2lFyJbWTZvSGTd7lrQGZnbjAb1GiGiOzTseCApaBiNeeB5cSyZMbXxCnXR4bGurBmphufm4MVDoFCE6ZTbDecmA3mtRO";
            var username = "Default";
            var response = await http.PostAsync(loginUrl, new FormUrlEncodedContent(new[] { new KeyValuePair<string?, string?>("key", key), new KeyValuePair<string?, string?>("username", username) }));
            var content = await response.Content.ReadAsStringAsync();
            var token = JsonSerializer.Deserialize<LoginResponseModel>(content).token;

            Console.WriteLine("token : " + token+"\n\n\n");

            return token;
        }
    }


    public class LoginResponseModel
    {
        public string success { get; set; }
        public string token { get; set; }
    }
}
