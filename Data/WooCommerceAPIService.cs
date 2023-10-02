using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace ProdUp.Data
{
    public class WooCommerceAPIService
    {
        private const string BaseUrl = "https://www.arcosmetici.it/wp-json/wc/v3/products";
        private const string ConsumerKey = "ck_b2d0bca4260ebdb8cc4f0b3642a7149e9202c371";
        private const string ConsumerSecret = "cs_981b37e37eea450e829bd3b4fca8c3790100ee27";

        public static async Task<List<ProductInfo>> GetProductListAsync()
        {
            using HttpClient client = new();
            var byteArray = Encoding.ASCII.GetBytes($"{ConsumerKey}:{ConsumerSecret}");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            var productInfoList = new List<ProductInfo>();

            try
            {
                HttpResponseMessage response = await client.GetAsync(BaseUrl);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody);

                var products = JsonConvert.DeserializeObject<List<JObject>>(responseBody);

                foreach (var product in products)
                {
                    var info = new ProductInfo
                    {
                        Name = product["name"].ToString(),
                        Type = product["type"].ToString(),
                        Regular_price = product["regular_price"].ToString(),
                        Description = product["description"].ToString(),
                        Short_description = product["short_description"].ToString(),
                        Categories = string.Join(", ", product["categories"].ToObject<List<JObject>>().ConvertAll(c => c["name"].ToString())),
                        Images = string.Join(", ", product["images"].ToObject<List<JObject>>().ConvertAll(i => i["src"].ToString()))
                    };
                    productInfoList.Add(info);
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Errore durante la richiesta: {e.Message}");
            }
            return productInfoList;
        }

        public static async Task<List<ProductInfo>> GetProductInfoAsync()
        {
            using HttpClient client = new();
            var byteArray = Encoding.ASCII.GetBytes($"{ConsumerKey}:{ConsumerSecret}");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            // Dati da inviare
            var data = new
            {
                name = "Premium Quality",
                type = "simple",
                regular_price = "21.99",
                description = "Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Vestibulum tortor quam, feugiat vitae, ultricies eget, tempor sit amet, ante. Donec eu libero sit amet quam egestas semper. Aenean ultricies mi vitae est. Mauris placerat eleifend leo.",
                short_description = "Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas.",
                categories = new[]
                {
                    new { id = 9 },
                    new { id = 14 }
                },
                images = new[]
                {
                    new { src = "http://demo.woothemes.com/woocommerce/wp-content/uploads/sites/56/2013/06/T_2_front.jpg" },
                    new { src = "http://demo.woothemes.com/woocommerce/wp-content/uploads/sites/56/2013/06/T_2_back.jpg" }
                }
            };

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await client.PostAsync(BaseUrl, content);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Errore durante la richiesta: {e.Message}");
            }


            return null;
        }
    }
}