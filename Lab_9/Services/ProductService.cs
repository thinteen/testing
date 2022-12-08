using System.Net;
using System.Text;
using System.Text.Json;
using Lab_9.Domain;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Lab_9.Services
{
    public struct ApiResponse
    {
        public string id { get; set; }
        public bool status { get; set; }
    }

    public class ProductService
    {
        private static string configurationFile = "D:/testing/Lab_9/Lab_9/Configurations/Configuration.json";
        private static readonly IConfigurationRoot configuration = new ConfigurationBuilder().AddJsonFile(configurationFile).Build();

        private readonly string uri = configuration["BASE_URI"];
        private readonly string api = configuration["API"];
        private readonly HttpClient httpClient;

        public ProductService()
        {
            httpClient = new(new HttpClientHandler())
            {
                BaseAddress = new Uri(uri)
            };
        }

        private ApiResponse GetResponse(HttpResponseMessage response)
        {
            dynamic content = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);

            ApiResponse apiResponse = new ApiResponse();

            if (content.id != 0)
            {
                apiResponse.id = content.id;
            }
            else
            {
                apiResponse.id = "-1";
            }

            if ((response.Content.ReadAsStringAsync().Result.Split(':')[1])[1] == '1')
            {
                apiResponse.status = true;
            }
            else
            {
                apiResponse.status = false;
            }

            return apiResponse;
        }

        public List<Product>? GetAll()
        {
            var response = httpClient.Send(new HttpRequestMessage(HttpMethod.Get, $"{api}/products"));
            var content = response.Content.ReadAsStringAsync().Result;
            var jsonOutput = JsonDocument.Parse(content).RootElement;

            List<Product> list = new();

            try
            {
                list = JsonConvert.DeserializeObject<List<Product>>(jsonOutput.GetRawText());
            }
            catch
            {
                return null;
            }

            return list;
        }

        public Product Get(string id) => GetAll().Find(p => p.id == id);

        public ApiResponse Create(Product product)
        {
            var apiResponse = new ApiResponse();
            try
            {
                var jsonObject = JsonSerializer.Serialize(product);

                var content = new StringContent(jsonObject, Encoding.UTF8, "application/json");
                var response = httpClient.PostAsync($"{api}/addproduct", content).Result;

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

        public bool Delete(string productId)
        {
            try
            {
                var response = httpClient
                    .Send(new HttpRequestMessage(HttpMethod.Get, $"{api}/deleteproduct?id={productId}"));

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

        public bool Update(Product newProduct)
        {
            try
            {
                var jsonObject = JsonSerializer.Serialize(newProduct);

                var content = new StringContent(jsonObject, Encoding.UTF8, "application/json");
                var response = httpClient.PostAsync($"{api}/editproduct", content).Result;

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
