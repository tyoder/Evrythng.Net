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
      private static IProductService _productService;

      static void Main(string[] args)
      {
          // Comment or uncomment the lines below to test Thngs or Products

          TestThngs();
          //TestProducts();          
      }

      private static void TestProducts()
      {
          string productId;
          _productService = new ProductService();

          // Create a Product
          var prod1 = new Product { fn = "first prod" };
          _productService.CreateProduct(prod1);

          // Grab its Id after it is created
          productId = prod1.Id;
          prod1 = null;

          // Get the Product we just created
          var prod2 = _productService.GetProduct(productId);
          PrintProduct(prod2);

          // Add some attributes and update
          prod2.description = "Console Product";
          prod2.brand = "Acme";
          prod2.url = "http://myproducturl/home.aspx";
          prod2.categories.Add("test category");
          prod2.categories.Add("console");
          prod2.tags.Add("test");
          prod2.tags.Add("disposable");
          prod2.photos.Add("http://www.somephotourl.com/photo1.jpg");
          prod2.photos.Add("http://www.somephotourl.com/photo2.jpg");
          prod2.identifiers.Add(new Identifier { key = "color", value = "red" });
          prod2.identifiers.Add(new Identifier { key = "size", value = "small" });
          // Add Properties (you cannot specify a timestamp if Properties are added this way)
          prod2.properties.Add(new Property { key = "speed", value = "fast" });
          prod2.properties.Add(new Property { key = "density", value = "thick" });

          _productService.UpdateProduct(prod2);
          prod2 = null;

          // Retrieve it again and print
          var prod3 = _productService.GetProduct(productId);
          PrintProduct(prod3);
          prod3 = null;

          // Create/Update Properties using Properties endpoint instead of Product (you CAN specify a timestamp this way)
          var myProperties = _productService.GetProperties(productId);
          myProperties.Add(new Property { key = "type", value = "entity", timestamp = new DateTime(2012, 1, 1) });
          myProperties.Add(new Property { key = "parent", value = "parent product", timestamp = new DateTime(2011, 1, 1) });
          _productService.CreateUpdateProperties(productId, myProperties);

          // Now get the Properties again
          var updatedProperties = _productService.GetProperties(productId);
          PrintProperties(updatedProperties);

          // Update the 'speed' property
          // First retrieve the property which actually brings back a collection (history) of values
          var speedPropertyList = _productService.GetPropertyHistory(productId, "speed");
          // Since this is actually a collection, add a new property with the same key to the collection and update
          speedPropertyList.Add(new Property { key = "speed", value = "slow", timestamp = DateTime.Now });
          _productService.CreateUpdateProperties(productId, speedPropertyList);

          // Now, when you get the Product again, the 'speed' property will only have the latest value ('slow')
          var prod4 = _productService.GetProduct(productId);
          PrintProduct(prod4);
          // Same result if you just get all properties
          var prod4Properties = _productService.GetProperties(productId);
          PrintProperties(prod4Properties);
          prod4Properties = null;
          prod4 = null;

          // Get and update a single property
          var typeProperty = _productService.GetPropertyHistory(productId, "type")[0];
          typeProperty.value = "animal";
          _productService.UpdateProperty(productId, typeProperty);
          var prod5Properties = _productService.GetProperties(productId);
          // Print the Properties - type property should now be 'animal'
          PrintProperties(prod5Properties);

          // Delete the Product
          Console.WriteLine();
          Console.WriteLine("Deleting Product...");
          _productService.DeleteProduct(productId);
          Console.WriteLine("Product deleted.");

          Console.Read();
      }

      private static void TestThngs()
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

       private static void PrintProduct(Product productToPrint)
       {
           Console.WriteLine();
           Console.WriteLine("Printing Product with Id: " + productToPrint.Id);
           Console.WriteLine("fn: " + productToPrint.fn);
           Console.WriteLine("description: " + productToPrint.description);
           Console.WriteLine("brand: " + productToPrint.brand);
           Console.WriteLine("url: " + productToPrint.url);
           Console.WriteLine("createdAt: " + productToPrint.createdAt);
           Console.WriteLine("updatedAt: " + productToPrint.updatedAt);
           Console.WriteLine("tags: ");
           foreach (var t in productToPrint.tags)
           {
               Console.WriteLine(" - " + t);
           }
           Console.WriteLine("categories: ");
           foreach (var c in productToPrint.categories)
           {
               Console.WriteLine(" - " + c);
           }
           Console.WriteLine("photos: ");
           foreach (var p in productToPrint.photos)
           {
               Console.WriteLine(" - " + p);
           }
           Console.WriteLine("properties: ");
           foreach (var p in productToPrint.properties)
           {
               Console.WriteLine(" - " + p.key + " : " + p.value);
           }
           Console.WriteLine("identifiers: ");
           foreach (var i in productToPrint.identifiers)
           {
               Console.WriteLine(" - " + i.key + " : " + i.value);
           }
       }
      
   }
}
