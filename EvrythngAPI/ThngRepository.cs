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
    /// <summary>
    /// The ThngRepository interacts directly with the Evrythng API and provides methods for 
    /// working with Thngs, Properties, and Locations.
    /// </summary>
    public class ThngRepository : IThngRepository
    {

        #region Private Variables
        private static string _apiKey;
        private static string _apiBaseAddress;
        private static HttpClient _httpClient;
        #endregion Private Variables

        #region Constructor
        public ThngRepository()
        {
            _apiKey = ConfigurationManager.AppSettings["APIKey"].ToString();
            _apiBaseAddress = ConfigurationManager.AppSettings["APIBaseAddress"].ToString();
            _httpClient = new HttpClient() { BaseAddress = new Uri(_apiBaseAddress) };
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(_apiKey);            
        }
        #endregion

        #region Private Helper Methods

        private void ConvertJArrayToLocations(JArray locationsArray, List<Location> locationsList)
        {           
            foreach (dynamic l in locationsArray)
            {
                var location = new Location();
                location.latitude = l.latitude;
                location.longitude = l.longitude;
                location.timestamp = Utilities.DateTimeSinceEpoch((long)l.timestamp);                

                locationsList.Add(location);
            }
        }

        private JArray ConvertLocationsToJArray(List<Location> locations)
        {
            var locationsArray = new JArray();

            foreach (Location l in locations)
            {
                dynamic jObject = new JObject();
                jObject.latitude = l.latitude;
                jObject.longitude = l.longitude;
                if (l.timestamp != null)
                {
                    jObject.timestamp = Utilities.MillisecondsSinceEpoch(l.timestamp);
                }

                locationsArray.Add(jObject);
            }

            return locationsArray;
        }                              
        
        private JObject ConvertThngToJObject(Thng thngToConvert)
        {
            // Create a dynamic JObject for more controlled serialization
            dynamic dynamicThng = new JObject();

            // Name should never be null - checked by service layer
            dynamicThng.name = thngToConvert.name;

            // Get ProductId
            if (!string.IsNullOrEmpty(thngToConvert.productId))
            {
                dynamicThng.product = thngToConvert.productId ?? string.Empty;
            }
                        
            // Get Description
            dynamicThng.description = thngToConvert.description ?? string.Empty;           

            // Convert tags
            var thngTags = new JProperty("tags", thngToConvert.tags);
            dynamicThng.Add(thngTags);
                        
            // Create a new JObject for Thng.properties
            dynamic propertiesObject = new JObject();
            foreach (var p in thngToConvert.properties)
            {
                var jProp = new JProperty(p.key, p.value);
                propertiesObject.Add(jProp);
            }
            dynamicThng.properties = propertiesObject;

            // Convert Location
            if (thngToConvert.location != null)
            {
                dynamic locationObject = new JObject();
                locationObject.latitude = thngToConvert.location.latitude;
                locationObject.longitude = thngToConvert.location.longitude;
                dynamicThng.location = locationObject;
            }

            return dynamicThng;

        }

        private void ConvertJObjectToThng(dynamic jObjectToConvert, Thng returnThng)
        {
            returnThng.Id = jObjectToConvert.id;
            returnThng.name = jObjectToConvert.name;

            returnThng.description = jObjectToConvert.description;
            returnThng.productId = jObjectToConvert.product;

            if (jObjectToConvert.tags != null)
            {
                returnThng.tags = jObjectToConvert.tags.ToObject<List<string>>();
            }

            if (jObjectToConvert.properties != null)
            {
                returnThng.properties = new List<Property>();
                if (jObjectToConvert.properties != null)
                {
                    foreach (var p in jObjectToConvert.properties)
                    {
                        // p is a Newtonsoft JProperty and has "Name" and "Value"
                        returnThng.properties.Add(new Property
                        {
                            key = p.Name,
                            value = p.Value,
                            timestamp = null                                    
                        });
                    }
                }
            }

            if (jObjectToConvert.location != null)
            {
                returnThng.location = new Location { latitude = jObjectToConvert.location.latitude, longitude = jObjectToConvert.location.longitude };
            }

            returnThng.createdAt = Utilities.DateTimeSinceEpoch((long)jObjectToConvert.createdAt);
            returnThng.updatedAt = Utilities.DateTimeSinceEpoch((long)jObjectToConvert.updatedAt);

        }

        #endregion Private Helper Methods

        #region Create Thng

        public void CreateThng(Thng thng)
        {            
            // Convert Thng to JObject
            var thngAsJobject = ConvertThngToJObject(thng);
            //if (string.IsNullOrEmpty(thngAsJobject.Property("product").Value.ToString()))
            //{
            //    thngAsJobject.Remove("product");
            //}
            
            // Serialize JObject to Json
            string thngJson = JsonConvert.SerializeObject(thngAsJobject, Formatting.Indented);                        

            // Set content of request
            var content = new StringContent(thngJson);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            
            var task = _httpClient.PostAsync("thngs", content)
                .ContinueWith((taskwithmsg) =>
                {
                    var response = taskwithmsg.Result;
                    // throws AggregateException if not a success code
                    response.EnsureSuccessStatusCode();
                    
                    var jsonTask = response.Content.ReadAsStringAsync();
                    jsonTask.Wait();
                    var jsonResult = jsonTask.Result;                    

                    // Evrythng API returns the created Thng with Id and timestamps
                    dynamic jObject = JObject.Parse(jsonResult);

                    // Convert dynamic object to Thng
                    ConvertJObjectToThng(jObject, thng);                    
                                     
                });
            task.Wait();

        }

        #endregion Create Thng

        #region Get Thngs

        public List<Thng> GetThngs()
        {
            List<Thng> myThngs = null;
            
            var task = _httpClient.GetAsync("thngs")
                .ContinueWith((taskwithmsg) =>
                {
                    var response = taskwithmsg.Result;
                    // throws AggregateException if not a success code
                    response.EnsureSuccessStatusCode();

                    var jsonTask = response.Content.ReadAsStringAsync();
                    jsonTask.Wait();
                    var jsonResult = jsonTask.Result;

                    dynamic thngs = JArray.Parse(jsonResult) as JArray;
                    myThngs = new List<Thng>();

                    foreach (dynamic t in thngs)
                    {
                        var myThng = new Thng();
                        ConvertJObjectToThng(t, myThng);
                        myThngs.Add(myThng);
                    }                                        
                });
            task.Wait();

            return myThngs;

        }

        #endregion Get Thngs

        #region Get Thng

        public Thng GetThng(string thngId)
        {
            Thng myThng = new Thng { Id = thngId };

            var task = _httpClient.GetAsync("thngs/" + thngId)
                .ContinueWith((taskwithmsg) =>
                {
                    var response = taskwithmsg.Result;
                    // throws AggregateException if not a success code
                    response.EnsureSuccessStatusCode();

                    var jsonTask = response.Content.ReadAsStringAsync();
                    jsonTask.Wait();
                    var jsonResult = jsonTask.Result;

                    // Evrythng API returns the created Thng with Id and timestamps
                    dynamic jObject = JObject.Parse(jsonResult);

                    // Convert dynamic object to Thng
                    ConvertJObjectToThng(jObject, myThng);
                });
            task.Wait();

            return myThng;
        }

        #endregion Get Thng

        #region Update Thng

        public void UpdateThng(Thng thng)
        {
            // Convert Thng to JObject
            var thngAsJobject = ConvertThngToJObject(thng);

            // Serialize JObject to Json
            string thngJson = JsonConvert.SerializeObject(thngAsJobject, Formatting.Indented);

            // Set content of request
            var content = new StringContent(thngJson);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            
            var task = _httpClient.PutAsync("thngs/" + thng.Id, content)
                .ContinueWith((taskwithmsg) =>
                {
                    var response = taskwithmsg.Result;
                    // throws AggregateException if not a success code
                    response.EnsureSuccessStatusCode();

                    var jsonTask = response.Content.ReadAsStringAsync();
                    jsonTask.Wait();
                    var jsonResult = jsonTask.Result;

                    // Evrythng API returns the created Thng with Id and timestamps
                    dynamic jObject = JObject.Parse(jsonResult);

                    // Convert dynamic object to Thng
                    ConvertJObjectToThng(jObject, thng);

                });
            task.Wait();

        }

        #endregion Update Thng

        #region Delete Thng

        public void DeleteThng(string thngId)
        {
            var task = _httpClient.DeleteAsync("thngs/" + thngId)
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

        #endregion Delete Thng

        #region Properties

        public List<Property> GetProperties(string thngId)
        {

            var endpointAddress = string.Format("thngs/{0}/properties", thngId);
            
            List<Property> thngProperties = null;

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
                    thngProperties = Utilities.ConvertJArrayToProperties(props);
                                                       
                });
            task.Wait();

            return thngProperties;

        }

        /// <summary>
        /// This method returns a list because if a Property value has changed several times, 
        /// then getting that Property will yield a list of past values sorted last to first.
        /// </summary>
        /// <param name="thngId">Id of the Thng</param>
        /// <param name="propertyKey">The key for the property</param>
        /// <returns>A list of property values over time.</returns>
        public List<Property> GetPropertyHistory(string thngId, string propertyKey)
        {
            var endpointAddress = string.Format("thngs/{0}/properties/{1}", thngId, propertyKey);

            List<Property> propertyHistory = null;

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
                    propertyHistory = Utilities.ConvertJArrayToProperties(props, propertyKey);

                                                       
                });
            task.Wait();

            return propertyHistory;
        }

        /// <summary>
        /// This method returns a list because if a Property value has changed several times, 
        /// then getting that Property will yield a list of past values sorted last to first.
        /// </summary>
        /// <param name="thngId">Id of the Thng</param>
        /// <param name="propertyKey">The key for the property</param>
        /// <returns>A list of property values over time.</returns>
        public List<Property> GetPropertyHistory(string thngId, string propertyKey, DateTime? beginDateTime, DateTime? endDateTime)
        {
            var beginMilliseconds = Utilities.MillisecondsSinceEpoch(beginDateTime);
            var endMilliseconds = Utilities.MillisecondsSinceEpoch(endDateTime);

            var endpointAddress = string.Format("thngs/{0}/properties/{1}?from={2}&to={3}", thngId, propertyKey, beginMilliseconds, endMilliseconds);

            List<Property> propertyHistory = null;

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
                    propertyHistory = Utilities.ConvertJArrayToProperties(props, propertyKey);


                });
            task.Wait();

            return propertyHistory;
        }

        public List<Property> CreateUpdateProperties(string thngId, List<Property> properties)
        {
            var returnedProperties = new List<Property>();
            var propertiesArray = Utilities.ConvertPropertiesToJArray(properties);

            // Set content of request
            var content = new StringContent(propertiesArray.ToString());
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            //  /thngs/{ThngID}/properties
            var propertyUpdateUrl = string.Format(@"/thngs/{0}/properties", thngId);
            var task = _httpClient.PutAsync(propertyUpdateUrl, content)
                .ContinueWith((taskwithmsg) =>
                {
                    var response = taskwithmsg.Result;
                    // throws AggregateException if not a success code
                    response.EnsureSuccessStatusCode();

                    var jsonTask = response.Content.ReadAsStringAsync();
                    jsonTask.Wait();
                    var jsonResult = jsonTask.Result;

                    // Evrythng API returns the array of Properties
                    dynamic jArray = JArray.Parse(jsonResult) as JArray;

                    // Convert JArray to Properties
                    returnedProperties = Utilities.ConvertJArrayToProperties(jArray);
                    
                });
            task.Wait();

            return returnedProperties;


        }

        /// <summary>
        /// This method updates an existing Property with multiple values
        /// </summary>
        /// <param name="thngId">Thng Id</param>
        /// <param name="properties">Updated property values</param>
        public void UpdateProperty(string thngId, List<Property> properties)
        {
            var propertyKey = properties[0].key;
            dynamic propertiesJArray = Utilities.ConvertPropertiesToJArray(properties);                       

            // Set content of request
            var content = new StringContent(propertiesJArray.ToString());
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            //  /thngs/{ThngID}/properties/{key}
            var propertyUpdateUrl = string.Format(@"/thngs/{0}/properties/{1}", thngId, propertyKey);
            var task = _httpClient.PutAsync(propertyUpdateUrl, content)
                .ContinueWith((taskwithmsg) =>
                {
                    var response = taskwithmsg.Result;
                    // throws AggregateException if not a success code
                    response.EnsureSuccessStatusCode();

                    var jsonTask = response.Content.ReadAsStringAsync();
                    jsonTask.Wait();
                    var jsonResult = jsonTask.Result;

                    dynamic props = JArray.Parse(jsonResult) as JArray;
                    properties = (List<Property>)Utilities.ConvertJArrayToProperties(props);                                       

                });
            task.Wait();
            
        }

        /// <summary>
        /// This method updates an existing property with one value
        /// </summary>
        /// <param name="thngId">Thng Id</param>
        /// <param name="property">Property to update</param>
        public void UpdateProperty(string thngId, Property property)
        {
            
            dynamic propertyAsJObject = new JObject();
            propertyAsJObject.value = property.value;
            if (property.timestamp != null)
            {
                propertyAsJObject.timestamp = Utilities.MillisecondsSinceEpoch(property.timestamp);
            }

            dynamic propertiesJArray = new JArray();
            propertiesJArray.Add(propertyAsJObject);
                        
            // Set content of request
            var content = new StringContent(propertiesJArray.ToString());
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            //  /thngs/{ThngID}/properties/{key}
            var propertyUpdateUrl = string.Format(@"/thngs/{0}/properties/{1}", thngId, property.key);
            var task = _httpClient.PutAsync(propertyUpdateUrl, content)
                .ContinueWith((taskwithmsg) =>
                {
                    var response = taskwithmsg.Result;
                    // throws AggregateException if not a success code
                    response.EnsureSuccessStatusCode();

                    var jsonTask = response.Content.ReadAsStringAsync();
                    jsonTask.Wait();
                    var jsonResult = jsonTask.Result;

                    dynamic props = JArray.Parse(jsonResult) as JArray;
                    var propertyHistory = (List<Property>)Utilities.ConvertJArrayToProperties(props, property.key);

                    // Grab the updated timestamp
                    var updatedProperty = propertyHistory.Find(p => p.value == property.value);
                    property.timestamp = updatedProperty.timestamp;
                   
                });
            task.Wait();

            //throw new NotImplementedException();
        }

        public void DeleteProperty(string thngId, string propertyKey)
        {
            var propertyDeleteUrl = string.Format(@"/thngs/{0}/properties/{1}", thngId, propertyKey);

            var task = _httpClient.DeleteAsync(propertyDeleteUrl)
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

        public void DeleteProperty(string thngId, string propertyKey, DateTime? endDateTime)
        {
            var endTimeMilliseconds = Utilities.MillisecondsSinceEpoch(endDateTime);
            var propertyDeleteUrl = string.Format(@"/thngs/{0}/properties/{1}?to={2}", thngId, propertyKey, endTimeMilliseconds);

            var task = _httpClient.DeleteAsync(propertyDeleteUrl)
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

        #endregion Properties

        #region Locations

        public List<Location> GetLocations(string thngId)
        {
            var endpointAddress = string.Format("thngs/{0}/location", thngId);

            var returnedLocations = new List<Location>();

            var task = _httpClient.GetAsync(endpointAddress)
                .ContinueWith((taskwithmsg) =>
                {
                    var response = taskwithmsg.Result;
                    // throws AggregateException if not a success code
                    response.EnsureSuccessStatusCode();

                    var jsonTask = response.Content.ReadAsStringAsync();
                    jsonTask.Wait();
                    var jsonResult = jsonTask.Result;

                    dynamic locationJArray = JArray.Parse(jsonResult) as JArray;
                    ConvertJArrayToLocations(locationJArray, returnedLocations);
                });
            task.Wait();

            return returnedLocations;
        }

        public List<Location> CreateUpdateLocations(string thngId, List<Location> locations)
        {
            var returnedLocations = new List<Location>();
            var locationsJArray = ConvertLocationsToJArray(locations);

            // Set content of request
            var content = new StringContent(locationsJArray.ToString());
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            //  /thngs/{ThngID}/properties
            var propertyUpdateUrl = string.Format(@"/thngs/{0}/location", thngId);
            var task = _httpClient.PutAsync(propertyUpdateUrl, content)
                .ContinueWith((taskwithmsg) =>
                {
                    var response = taskwithmsg.Result;
                    // throws AggregateException if not a success code
                    response.EnsureSuccessStatusCode();

                    var jsonTask = response.Content.ReadAsStringAsync();
                    jsonTask.Wait();
                    var jsonResult = jsonTask.Result;

                    // Evrythng API returns the array of Locations
                    dynamic jArray = JArray.Parse(jsonResult) as JArray;

                    // Convert JArray to Locations
                    ConvertJArrayToLocations(jArray, returnedLocations);

                });
            task.Wait();

            return returnedLocations;
        }

        public void DeleteLocations(string thngId)
        {
            var propertyDeleteUrl = string.Format(@"/thngs/{0}/location", thngId);

            var task = _httpClient.DeleteAsync(propertyDeleteUrl)
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
        
        public void DeleteLocations(string thngId, DateTime? endDateTime)
        {
            var endTimeMilliseconds = Utilities.MillisecondsSinceEpoch(endDateTime);
            var locationsDeleteUrl = string.Format(@"/thngs/{0}/location?to={1}", thngId, endTimeMilliseconds);

            var task = _httpClient.DeleteAsync(locationsDeleteUrl)
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

        #endregion Locations
    }
}
