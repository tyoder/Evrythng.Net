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

        private List<Property> ConvertJArrayToProperties(JArray propertiesArray)
        {
            var properties = new List<Property>();

            foreach (dynamic p in propertiesArray)
            {
                var myProperty = new Property();
                myProperty.key = p.key;
                myProperty.value = p.value;
                myProperty.timestamp = Utilities.DateTimeSinceEpoch((long)p.timestamp);
                properties.Add(myProperty);
            }

            return properties;
        }

        private JObject ConvertProductToJObject(Product productToConvert)
        {
            // Create a dynamic JObject for more controlled serialization
            dynamic dynamicProduct = new JObject();

            // Name should never be null - checked by service layer
            dynamicProduct.fn = productToConvert.fn;

            // Get Description - if null, set to string.Empty
            dynamicProduct.description = productToConvert.description ?? string.Empty;

            // Get Brand - if null, set to string.Empty
            dynamicProduct.brand = productToConvert.brand ?? string.Empty;

            // Get Url - if null, set to string.Empty
            dynamicProduct.url = productToConvert.url ?? string.Empty;
            
            // Convert categories                            
            var productCategories = new JProperty("categories", productToConvert.categories);
            dynamicProduct.Add(productCategories);
            
            // Convert photos
            var productPhotos = new JProperty("photos", productToConvert.photos);
            dynamicProduct.Add(productPhotos);

            // Convert tags
            var productTags = new JProperty("tags", productToConvert.tags);
            dynamicProduct.Add(productTags);

            //Create a new JObject for Product.properties
            dynamic propertiesObject = new JObject();
            foreach (var p in productToConvert.properties)
            {
                var jProp = new JProperty(p.key, p.value);
                propertiesObject.Add(jProp);
            }
            dynamicProduct.properties = propertiesObject;

            //// Convert Properties
            //if (productToConvert.properties != null && productToConvert.properties.Count > 0)
            //{
            //    // Create a new JObject for Thng.properties
            //    dynamic propertiesObject = new JObject();
            //    foreach (var p in productToConvert.properties)
            //    {
            //        var jProp = new JProperty(p.key, p.value);
            //        propertiesObject.Add(jProp);
            //    }
            //    dynamicProduct.properties = propertiesObject;
            //}

            //// Convert Identifiers
            //if (productToConvert.identifiers != null && productToConvert.identifiers.Count > 0)
            //{
                dynamic identitiesObject = new JObject();
                foreach (var i in productToConvert.identifiers)
                {
                    var jProp = new JProperty(i.key, i.value);
                    identitiesObject.Add(jProp);
                }
                dynamicProduct.identifiers = identitiesObject;
            //}

            return dynamicProduct;

        }

        private void ConvertJObjectToProduct(dynamic jObjectToConvert, Product product)
        {                                    
            product.Id = jObjectToConvert.id;
            product.fn = jObjectToConvert.fn;
            product.description = jObjectToConvert.description;
            product.brand = jObjectToConvert.brand;
            product.url = jObjectToConvert.url;

            if (jObjectToConvert.categories != null)
            {
                product.categories = jObjectToConvert.categories.ToObject<List<string>>();
            }

            if (jObjectToConvert.photos != null)
            {
                product.photos = jObjectToConvert.photos.ToObject<List<string>>();
            }           

            if (jObjectToConvert.tags != null)
            {
                product.tags = jObjectToConvert.tags.ToObject<List<string>>();
            }

            if (jObjectToConvert.properties != null)
            {
                product.properties = new List<Property>();
                if (jObjectToConvert.properties != null)
                {
                    foreach (var p in jObjectToConvert.properties)
                    {
                        // p is a Newtonsoft JProperty and has "Name" and "Value"
                        product.properties.Add(new Property
                        {
                            key = p.Name,
                            value = p.Value,
                            timestamp = null  // When creating Properties this way, no timestamp is returned
                        });
                    }
                }
            }

            if (jObjectToConvert.identifiers != null)
            {
                product.identifiers = new List<Identifier>();
                if (jObjectToConvert.identifiers != null)
                {
                    foreach (var i in jObjectToConvert.identifiers)
                    {
                        // i is a Newtonsoft JProperty and has "Name" and "Value"
                        product.identifiers.Add(new Identifier
                        {
                            key = i.Name,
                            value = i.Value
                        });
                    }
                }
            }

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
                    // The Repository will just create the Product - service layer should also do a GET to sync all attributes 
                    dynamic jObject = JObject.Parse(jsonResult);
                    product.Id = jObject.id;
                    product.createdAt = Utilities.DateTimeSinceEpoch((long)jObject.createdAt);
                    product.updatedAt = Utilities.DateTimeSinceEpoch((long)jObject.updatedAt);                    

                });
            task.Wait();
                                    
        }

        public Product GetProduct(string productId)
        {
            Product retrievedProduct = new Product { Id = productId };

            var task = _httpClient.GetAsync("products/" + productId)
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

                    // Convert dynamic object to Thng
                    ConvertJObjectToProduct(jObject, retrievedProduct);
                });
            task.Wait();

            return retrievedProduct;

        }

        public List<Product> GetProducts()
        {
            List<Product> products = null;

            var task = _httpClient.GetAsync("products")
                .ContinueWith((taskwithmsg) =>
                {
                    var response = taskwithmsg.Result;
                    // throws AggregateException if not a success code
                    response.EnsureSuccessStatusCode();

                    var jsonTask = response.Content.ReadAsStringAsync();
                    jsonTask.Wait();
                    var jsonResult = jsonTask.Result;

                    dynamic productsArray = JArray.Parse(jsonResult) as JArray;
                    products = new List<Product>();

                    foreach (dynamic p in productsArray)
                    {
                        var product = new Product();
                        ConvertJObjectToProduct(p, product);
                        products.Add(product);
                    }
                });
            task.Wait();

            return products;
        }

        public void UpdateProduct(Product product)
        {
            // Convert Product to JObject
            var productAsJobject = ConvertProductToJObject(product);

            // Serialize JObject to Json
            string productJson = JsonConvert.SerializeObject(productAsJobject, Formatting.Indented);

            // Set content of request
            var content = new StringContent(productJson);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var task = _httpClient.PutAsync("products/" + product.Id, content)
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

                    // Convert dynamic object to Thng
                    ConvertJObjectToProduct(jObject, product);

                });
            task.Wait();

        }

        public void DeleteProduct(string productId)
        {
            var task = _httpClient.DeleteAsync("products/" + productId)
                    .ContinueWith((taskwithmsg) =>
                    {
                        var response = taskwithmsg.Result;                                                

                        if (!response.IsSuccessStatusCode)
                        {
                            throw new Exception(string.Format("Status Code: {0}, Reason: {1}", response.StatusCode, response.ReasonPhrase));
                        }
                    });
            task.Wait();
        }

        #region Property Methods

        public List<Property> GetProperties(string productId)
        {

            var endpointAddress = string.Format("products/{0}/properties", productId);

            List<Property> productProperties = null;

            var task = _httpClient.GetAsync(endpointAddress)
                .ContinueWith((taskwithmsg) =>
                {
                    var response = taskwithmsg.Result;
                    // throws AggregateException if not a success code
                    response.EnsureSuccessStatusCode();

                    var jsonTask = response.Content.ReadAsStringAsync();
                    jsonTask.Wait();
                    var jsonResult = jsonTask.Result;

                    dynamic props = JArray.Parse(jsonResult) as JArray;                    
                    productProperties = ConvertJArrayToProperties(props);
                                                           
                });
            task.Wait();

            return productProperties;

        }

        /// <summary>
        /// Gets a specific property of a Product and returns a history of values with timestamps.
        /// </summary>
        /// <param name="productId">The Id of the Product</param>
        /// <param name="propertyKey">The property key of the desired property</param>
        /// <returns>A specific Product Property</returns>
        public List<Property> GetPropertyHistory(string productId, string propertyKey)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a specific property of a Product within a specified time interval
        /// </summary>
        /// <param name="productId">The Id of the Product</param>
        /// <param name="propertyKey">The property key of the desired property</param>
        /// <param name="beginDateTime">The "Begin" DateTime</param>
        /// <param name="endDateTime">The "End" DateTime</param>
        /// <returns></returns>
        public List<Property> GetPropertyHistory(string productId, string propertyKey, DateTime? beginDateTime, DateTime? endDateTime)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Creates or updates properties of a Product
        /// </summary>
        /// <param name="productId">the Id of the Product</param>
        /// <param name="properties">One or more properties to create or update.</param>
        /// <returns>The property or properties created or updated.</returns>
        public List<Property> CreateUpdateProperties(string productId, List<Property> properties)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates a specific property of a Product
        /// </summary>
        /// <param name="productId">The Id of the Product</param>
        /// <param name="property">The Property object with updates</param>
        /// <returns>void - Property passed by reference will be updated</returns>
        public void UpdateProperty(string productId, Property property)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates a specific property of a Product with multiple values
        /// </summary>
        /// <param name="productId">The Id of the Product</param>
        /// <param name="properties">List of properties to update</param>
        /// <returns>void - Properties passed by reference will be updated</returns>
        public void UpdateProperty(string productId, List<Property> properties)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes a specific Property of a Product, including all values of that Property over time.
        /// </summary>
        /// <param name="productId">The Id of the Product</param>
        /// <param name="propertyKey">The property key of the property to delete</param>
        public void DeleteProperty(string productId, string propertyKey)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes all values of a Property prior to the specified timestamp.
        /// </summary>
        /// <param name="productId">The Id of the Product</param>
        /// <param name="propertyKey">The property key</param>
        /// <param name="endDateTime">Timestamp before which all property values will be removed</param>
        public void DeleteProperty(string productId, string propertyKey, DateTime? endDateTime)
        {
            throw new NotImplementedException();
        }

        #endregion Property Methods


    }
}
