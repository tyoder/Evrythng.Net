using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using EvrythngAPI;

namespace EvrythngConsole
{
   
   /// <summary>
   /// Before running this console application, you must obtain an API key from Evrythng
   /// and place it in the App.config file.
   /// Getting started with an API key, etc:  https://dev.evrythng.com/documentation/start
   /// </summary> 
   class Program
   {       
      private static IThngService _thngService;       

      static void Main(string[] args)
      {
          string thngId;
          
          // Instantiate the ThngService
          _thngService = new ThngService();          

          // Create a Thng
          var thng1 = new Thng { name = "Hello" };
          _thngService.CreateThng(thng1);
          
          // Grab its Id after it is created
          thngId = thng1.Id;
          PrintThng(thng1);
          thng1 = null;

          // Get the Thng we just created
          var thng2 = _thngService.GetThng(thngId);
          
          // Add a description and tags, then update thng
          thng2.description = "Console Thng";
          thng2.tags.Add("test");
          thng2.tags.Add("disposable");
          _thngService.UpdateThng(thng2);
          PrintThng(thng2);

          // Add Properties using Thng (you cannot specify a timestamp if properties are added this way)
          thng2.properties.Add(new Property { key = "color", value = "blue" });
          thng2.properties.Add(new Property { key = "temperature", value = "25 C" });
          _thngService.UpdateThng(thng2);
          thng2 = null;

          // Retrieve it again
          var thng3 = _thngService.GetThng(thngId);
          PrintThng(thng3);
          thng3 = null;

          // Create/Update Properties using Properties instead of Thng (you CAN specify a timestamp this way)
          var myProperties = _thngService.GetProperties(thngId);
          myProperties.Add(new Property { key = "speed", value = "fast", timestamp = new DateTime(2012, 1, 1) });
          myProperties.Add(new Property { key = "url", value = "http://google.com", timestamp = new DateTime(2011, 1, 1) });
          _thngService.CreateUpdateProperties(thngId, myProperties);

          // Now get the Properties again
          var updatedProperties = _thngService.GetProperties(thngId);
          PrintProperties(updatedProperties);

          // Update the 'speed' property
          // First retrieve the property which actually brings back a collection (history) of values
          var speedPropertyList = _thngService.GetPropertyHistory(thngId, "speed");
          // Since this is actually a collection, add a new property with the same key to the collection and update
          speedPropertyList.Add(new Property { key = "speed", value = "slow", timestamp = DateTime.Now });
          _thngService.CreateUpdateProperties(thngId, speedPropertyList);

          // Now, when you get the Thng again, the 'speed' property will only have the latest value ('slow')
          var thng4 = _thngService.GetThng(thngId);
          PrintThng(thng4);
          // Same result if you just get all properties
          var thng4Properties = _thngService.GetProperties(thngId);
          PrintProperties(thng4Properties);
          
          // But, if you retrieve the 'speed' property, you get a history of the values
          var speedPropertyUpdatedList = _thngService.GetPropertyHistory(thngId, "speed");
          PrintProperties(speedPropertyUpdatedList);

          // Add a Location to a Thng
          // To add a Location to an existing Thng, use CreateUpdateLocations
          var locationCollection = new List<Location>();          
          locationCollection.Add(new Location { latitude = 30, longitude = 40, timestamp = new DateTime(2012, 4, 15) });
          _thngService.CreateUpdateLocations(thngId, locationCollection);

          // Now there is a Location history - similar to property history
          var locationHistory = _thngService.GetLocations(thngId);
          PrintLocations(locationHistory);

          // Add another location
          locationHistory.Add(new Location { latitude = 35, longitude = 45, timestamp = new DateTime(2012, 4, 16) });
          _thngService.CreateUpdateLocations(thngId, locationHistory);
          var updatedLocations = _thngService.GetLocations(thngId);
          // now there should be two locations at different times
          PrintLocations(updatedLocations);
                                        
          // Delete the Thng
          Console.WriteLine("Deleting Thng...");
          _thngService.DeleteThng(thngId);

          Console.Read();
      }

      private static void PrintLocations(List<Location> locationHistory)
      {
          Console.WriteLine("Locations:");
          foreach (var l in locationHistory)
          {
              Console.WriteLine(" - latitude: " + l.latitude + "; longitude: " + l.longitude + "; timestamp: " + l.timestamp);                 
          }
      }       

       private static void PrintThng(Thng thngToPrint)
       {
           Console.WriteLine();
           Console.WriteLine("Printing Thng with Id: " + thngToPrint.Id);
           Console.WriteLine("name: " + thngToPrint.name);
           Console.WriteLine("description: " + thngToPrint.description);
           Console.WriteLine("createdAt: " + thngToPrint.createdAt);
           Console.WriteLine("updatedAt: " + thngToPrint.updatedAt);
           Console.WriteLine("tags: ");
           foreach (var t in thngToPrint.tags)
           {
               Console.WriteLine(" - " + t);
           }
           Console.WriteLine("properties: ");
           foreach (var p in thngToPrint.properties)
           {
               Console.WriteLine(" - " + p.key + " : " + p.value);
           }
           Console.WriteLine("location: ");
           if (thngToPrint.location != null)
           {
               Console.WriteLine(" - latitude: " + thngToPrint.location.latitude);
               Console.WriteLine(" - longitude: " + thngToPrint.location.latitude);
               Console.WriteLine(" - timestamp: " + thngToPrint.location.timestamp.ToString());
           }           
       }

       private static void PrintProperties(List<Property> properties)
       {
           Console.WriteLine();
           Console.WriteLine("Properties:");
           foreach (var p in properties)
           {
               Console.WriteLine(" - " + p.key + ": " + p.value + " timestamp: " + p.timestamp);
           }

       }
      
   }
}
