using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
//using Microsoft.CSharp;
using Newtonsoft.Json.Linq;
using EvrythngAPI;

namespace EvrythngConsole
{
   class Program
   {       
       private static IThngService _thngService;
       //private static Thng _myThng;

      static void Main(string[] args)
      {
          // Instantiate ThngService
          _thngService = new ThngService();

          // Create a Thng
          var helloThng = new Thng { name = "Hello" };
          CreateThng(helloThng);

          // Get a Thng
          var thngId = helloThng.Id;
          var myThng = GetThng(thngId);

          // Show Attributes of Thng in console
          PrintThng(myThng);

          // Update description and add tags
          myThng.description = "Console Thng";
          myThng.tags.Add("test");
          myThng.tags.Add("disposable");
          _thngService.UpdateThng(myThng);
          PrintThng(myThng);

          //// Add Properties using Thng
          //myThng.properties.Add(new Property { key = "color", value = "blue" });
          //myThng.properties.Add(new Property { key = "temperature", value = "25 C" });
          //_thngService.UpdateThng(myThng);
          //myThng = _thngService.GetThng(myThng.Id);
          //PrintThng(myThng);

          //// Create/Update Properties using Properties
          //var myProperties = _thngService.GetProperties(myThng.Id);
          //myProperties.Add(new Property { key = "speed", value = "fast", timestamp = new DateTime(2012, 1, 1) });
          //myProperties.Add(new Property { key = "url", value = "http://google.com", timestamp = new DateTime(2011, 1, 1) });
          //_thngService.CreateUpdateProperties(myThng.Id, myProperties);
          
          // Get Thng again
          var myThng2 = _thngService.GetThng(myThng.Id);
          PrintThng(myThng2);
          
          // Delete the Thng
          DeleteThng(myThng.Id);

          Console.Read();
      }

       private static void CreateThng(Thng thngToCreate)
       {
           Console.WriteLine("Creating a new Thng...");
           _thngService.CreateThng(thngToCreate);
           Console.WriteLine("Thng Created at: " + thngToCreate.createdAt.ToString());
       }

       private static void UpdateThng(Thng thngToUpdate)
       {
           Console.WriteLine("Updating a Thng...");
           _thngService.UpdateThng(thngToUpdate);
           Console.WriteLine("Thng Updated at: " + thngToUpdate.updatedAt);
       }

       private static Thng GetThng(string thngId)
       {
           Console.WriteLine("Getting a Thng...");
           return _thngService.GetThng(thngId);
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
               Console.WriteLine(" - " + p.key + " : " + p.value + " timestamp: " + p.timestamp.ToString());
           }
           Console.WriteLine("location: ");
           if (thngToPrint.location != null)
           {
               Console.WriteLine(" - latitude: " + thngToPrint.location.latitude);
               Console.WriteLine(" - longitude: " + thngToPrint.location.latitude);
               Console.WriteLine(" - timestamp: " + thngToPrint.location.timestamp.ToString());
           }           
       }

       private static void DeleteThng(string thngId)
       {
           Console.WriteLine("Deleting Thng...");
           _thngService.DeleteThng(thngId);
       }
   }
}
