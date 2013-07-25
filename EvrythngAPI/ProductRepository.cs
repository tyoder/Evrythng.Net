using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.CSharp;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using EvrythngAPI;
using System.Net.Http.Headers;
using System.Web;

namespace EvrythngAPI
{
    public class ProductRepository : IProductRepository
    {

        #region Private Variables
        private static string _apiKey;
        private static string _apiBaseAddress;
        private static HttpClient _httpClient;
        #endregion Private Variables
         
        public ProductRepository()
        {
            _apiKey = ConfigurationManager.AppSettings["APIKey"].ToString();
            _apiBaseAddress = ConfigurationManager.AppSettings["APIBaseAddress"].ToString();
            _httpClient = new HttpClient() { BaseAddress = new Uri(_apiBaseAddress) };
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(_apiKey);
        }

        #region Private Methods

        private JObject ConvertProductToJObject(Product productToConvert)
        {
            // Create a dynamic JObject for more controlled serialization
            dynamic dynamicProduct = new JObject();

            // Name should never be null - checked by service layer
            dynamicProduct.fn = productToConvert.fn;

            // Get Description
            if (!string.IsNullOrEmpty(productToConvert.description))
            {
                dynamicProduct.description = productToConvert.description;
            }

            return dynamicProduct;

        }

        private void ConvertJObjectToProduct(dynamic jObjectToConvert, Product product)
        {
            product.Id = jObjectToConvert.id;
            product.fn = jObjectToConvert.fn;
            product.description = jObjectToConvert.description;

            product.createdAt = Utilities.DateTimeSinceEpoch((long)jObjectToConvert.createdAt);
            product.updatedAt = Utilities.DateTimeSinceEpoch((long)jObjectToConvert.updatedAt);
        }

        #endregion Private Methods


        public void CreateProduct(Product product)
        {
            // Convert Product to JObject
            var productAsJobject = ConvertProductToJObject(product);

            // Serialize JObject to Json
            string productJson = JsonConvert.SerializeObject(productAsJobject, Formatting.Indented);

            // Set content of request
            var content = new StringContent(productJson);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var task = _httpClient.PostAsync("products", content)
                .ContinueWith((taskwithmsg) =>
                {
                    var response = taskwithmsg.Result;
                    // throws AggregateException if not a success code
                    response.EnsureSuccessStatusCode();

                    var jsonTask = response.Content.ReadAsStringAsync();
                    jsonTask.Wait();
                    var jsonResult = jsonTask.Result;

                    // Evrythng API returns the created Product with Id and timestamps
                    dynamic jObject = JObject.Parse(jsonResult);

                    // Convert dynamic object to Product
                    ConvertJObjectToProduct(jObject, product);

                });
            task.Wait();
            
            
            //throw new NotImplementedException();
        }
    }
}
