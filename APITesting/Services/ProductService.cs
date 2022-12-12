using APITesting.Domain;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Text.Json;
using static APITesting.Domain.Product;

namespace APITesting.Services
{
    public class ProductService
    {
        private static string configurationFile = "D:/testing/APITesting/APITesting/Configuration/Configuration.json";
        private static readonly IConfigurationRoot configuration = new ConfigurationBuilder().AddJsonFile(configurationFile).Build();
        private readonly string uri = configuration["BASE_URI"];

        private readonly HttpClient httpClient;


        public ProductService()
        {
            httpClient = new(new HttpClientHandler())
            {
                BaseAddress = new Uri(uri)
            };
        }

        public List<Product> GetList()
        {
            var response = httpClient.Send(new HttpRequestMessage(HttpMethod.Get, "products"));

            var content = response.Content.ReadAsStringAsync().Result;

            List<Product> list = new();

            try
            {
                list = JsonConvert.DeserializeObject<List<Product>>(JsonDocument.Parse(content).RootElement.GetRawText());
            }
            catch
            {
                return null;
            }

            return list;
        }

        public Product Get(string id) => GetList().Find(p => p.id == id);

        private ApiResponse GetResponse(HttpResponseMessage response)
        {
            return new(JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result));
        } 

        public ApiResponse Create(Product product)
        {
            ApiResponse apiResponse = new();

            try
            {
                var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(product), Encoding.UTF8, "application/json");
                var response = httpClient.PostAsync("addproduct", content).Result;

                apiResponse = GetResponse(response);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    apiResponse.id = "-1";
                    apiResponse.status = false;
                }
            }
            catch
            {
                apiResponse.id = "-1";
                apiResponse.status = false;
            }

            return apiResponse;
        }

        public bool Update(Product newProduct)
        {
            try
            {
                var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(newProduct), Encoding.UTF8, "application/json");
                var response = httpClient.PostAsync("editproduct", content).Result;

                if ((response.Content.ReadAsStringAsync().Result.Split(':')[1])[1] == '1')
                {
                    return true;
                }
                else
                {
                    return false;
                }

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool Delete(string productId)
        {
            try
            {
                var response = httpClient.Send(new HttpRequestMessage(HttpMethod.Delete, $"deleteproduct?id={productId}"));

                if ((response.Content.ReadAsStringAsync().Result.Split(':')[1])[1] == '1')
                {
                    return true;
                }
                else
                {
                    return false;
                }

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

    }
}