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
        private static string BASE_URL = "https://localhost/opencart/";
        static async Task Main(string[] args)
        {
            Console.WriteLine("OpenCart Api Client");

            var http = new HttpClient(new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (a, b, c, d) => true
            });

            var api_token = await LoginAndGetTokenAsync(http);

            await GetProductsFromHatraApiAsync(http, api_token);
            await GetProductsAsync(http, api_token);
            await GetCustomersAsync(http, api_token);

            Console.ReadKey();
        }

        private static async Task GetProductsFromHatraApiAsync(HttpClient http, string apiToken)
        {
            var route = $"{BASE_URL}index.php?route=api/hatra/getproducts&start=0&limit=1&api_token={apiToken}";
            var response = await http.GetAsync(route);
            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine("products by hatra api :\n" + content + "\n\n\n");
        }

        private static async Task GetProductsAsync(HttpClient http, string apiToken)
        {
            var route = $"{BASE_URL}index.php?route=api/cart/products&api_token={apiToken}";
            var response = await http.GetAsync(route);
            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine("products :\n" + content + "\n\n\n");
        }
        private static async Task GetCustomersAsync(HttpClient http, string apiToken)
        {
            var route = $"{BASE_URL}index.php?route=api/customer&api_token={apiToken}";
            var response = await http.GetAsync(route);
            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine("customers :\n"+content + "\n\n\n");
        }

        public static async Task<string> LoginAndGetTokenAsync(HttpClient http)
        {
            var loginUrl = $"{BASE_URL}index.php?route=api/login";
            var key =
                "qd1gBelYZn5MQCfDh0znIsEMlmpPk1h3KYcs6mL1ZE5VD9EN6Aqcpryt3jGGatppJ3ZCBbuFc2ftwfWAMs44cbBVZMWL0nWjvJNTu5b3742YPsrBib38c7rPgpP8xfGjccb8EZwt2ppcEtK2MLrWu7xrZqKvNBqM3JZ05R0960V1asmwmRzbmybhAWl2TkNCM3QlOYmHw76s7psghBgb1M9CXPWUfsQIPM8e0lzF2Dt8ZFh9ISeRRz2lVBq7xRvF";
            var username = "Default";
            var response = await http.PostAsync(loginUrl, new FormUrlEncodedContent(new[] { new KeyValuePair<string?, string?>("key", key), new KeyValuePair<string?, string?>("username", username) }));
            var content = await response.Content.ReadAsStringAsync();
            var api_token = JsonSerializer.Deserialize<LoginResponseModel>(content).api_token;

            Console.WriteLine("api_token : " + api_token+"\n\n\n");

            return api_token;
        }
    }


    public class LoginResponseModel
    {
        public string success { get; set; }
        public string api_token { get; set; }
    }
}
