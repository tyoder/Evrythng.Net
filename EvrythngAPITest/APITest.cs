using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Microsoft.CSharp;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using EvrythngAPI;

namespace EvrythngAPITest
{
   [TestClass]
   public class APITest
   {

       #region Private Members
       private static string _apiKey;
       private static string _apiBaseAddress;

       #endregion Private Members

       public APITest()
       {
           _apiKey = ConfigurationManager.AppSettings["APIKey"].ToString();
           _apiBaseAddress = ConfigurationManager.AppSettings["APIBaseAddress"].ToString();
       }

      
      [TestMethod]
      public void GetMyThngs()
      {
          
          using (var httpClient = new HttpClient() { BaseAddress = new Uri(_apiBaseAddress) })
          {
              var request = new HttpRequestMessage();
              request.Method = HttpMethod.Get;
              request.RequestUri = new Uri(httpClient.BaseAddress + "thngs");
              request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(_apiKey);
              
              var task = httpClient.SendAsync(request)
                  .ContinueWith((taskwithmsg) =>
                      {
                          var response = taskwithmsg.Result;

                          var jsonTask = response.Content.ReadAsStringAsync();
                          jsonTask.Wait();
                          var jsonObject = jsonTask.Result;
                          Console.Write(jsonObject.ToString());
                      });
              task.Wait();
                                        
          }
      }

      [TestMethod]
      public void GetMyThngsAndUseJsonNet()
      {

          using (var httpClient = new HttpClient() { BaseAddress = new Uri(_apiBaseAddress) })
          {
              var request = new HttpRequestMessage();
              request.Method = HttpMethod.Get;
              request.RequestUri = new Uri(httpClient.BaseAddress + "thngs");
              request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(_apiKey);
              var task = httpClient.SendAsync(request)
                  .ContinueWith((taskwithmsg) =>
                  {
                      var response = taskwithmsg.Result;

                      var jsonTask = response.Content.ReadAsStringAsync();
                      jsonTask.Wait();
                      var jsonString = jsonTask.Result;
                      Console.Write("Result as String:\r\n" + jsonString);
                      Console.WriteLine();

                      JArray jArray = JArray.Parse(jsonString) as JArray;
                      dynamic thngs = jArray;

                      foreach (dynamic thng in thngs)
                      {
                          Console.WriteLine();
                          Console.WriteLine("ID: " + thng.id);
                          Console.WriteLine("Created: " + thng.createdAt);
                          Console.WriteLine("Updated: " + thng.updatedAt);
                          Console.WriteLine("Name: " + thng.name);
                          Console.WriteLine("Description: " + thng.description);
                          Console.WriteLine("Location: " + thng.location);
                          Console.WriteLine("Properties: " + thng.properties);
                          Console.WriteLine("Tags: " + thng.tags);
                      }

                  });
              task.Wait();

          }
      }

      //[TestMethod]
      //public void GetMyThngsAndDeserializeToObjects()
      //{

      //    using (var httpClient = new HttpClient() { BaseAddress = new Uri(_apiBaseAddress) })
      //    {
      //        var request = new HttpRequestMessage();
      //        request.Method = HttpMethod.Get;
      //        request.RequestUri = new Uri(httpClient.BaseAddress + "thngs");
      //        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(_apiKey);
      //        var task = httpClient.SendAsync(request)
      //            .ContinueWith((taskwithmsg) =>
      //            {
      //                var response = taskwithmsg.Result;

      //                var jsonTask = response.Content.ReadAsStringAsync();
      //                jsonTask.Wait();
      //                var jsonString = jsonTask.Result;
      //                Console.Write("Result as String:\r\n" + jsonString);
      //                Console.WriteLine();

      //                JArray jArray = JArray.Parse(jsonString) as JArray;
      //                dynamic thngs = jArray;
      //                var myThngs = new List<Thng>();

      //                foreach (dynamic t in thngs)
      //                {
      //                    Console.WriteLine();
      //                    var myThng = new Thng();
      //                    myThng.Id = t.Id;
      //                    var createdMilliSeconds = (double)t.createdAt;
      //                    myThng.createdAt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(createdMilliSeconds);
      //                    var updatedMilliSeconds = (double)t.createdAt;
      //                    myThng.updatedAt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(updatedMilliSeconds);
      //                    myThng.description = t.description;
      //                    myThng.location = new Location { latitude = t.location.latitude, longitude = t.location.longitude };

      //                    myThng.tags = new List<string>();
      //                    foreach (var tag in t.tags)
      //                    {
      //                        myThng.tags.Add(tag.Value);
      //                    }

      //                    //myThng.tags = t.tags;

      //                    myThng.properties = new List<Property>();
      //                    if (t.properties != null)
      //                    {
      //                        foreach (var p in t.properties)
      //                        {
      //                            // p is a Newtonsoft JProperty and has "Name" and "Value"
      //                            myThng.properties.Add(new Property { key = p.Name, value = p.Value });
      //                        }
      //                    }
                          
      //                }

      //            });
      //        task.Wait();

      //    }
      //}




   }
}
