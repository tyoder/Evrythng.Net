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
using System.Net.Http.Headers;
using System.Linq;

namespace EvrythngAPITest
{
    [TestClass]
    public class ProductRepositoryTests
    {
        #region Private Members

        private TestContext testContextInstance;
        private static string _apiKey;
        private static string _apiBaseAddress;        
        private static IProductRepository _sut;

        #endregion Private Members

        public ProductRepositoryTests()
        {
            _apiKey = ConfigurationManager.AppSettings["APIKey"].ToString();
            _apiBaseAddress = ConfigurationManager.AppSettings["APIBaseAddress"].ToString();
            _sut = new ProductRepository();

        }

        #region Private Methods

        private Product CreateTestProduct()
        {
            var productOne = new Product();
            productOne.fn = "prod1";
            productOne.brand = "ford";
            productOne.description = "my first prod";
            productOne.categories.Add("cars");
            productOne.tags.Add("trucks");
            productOne.photos.Add("http://www.google.com");
            productOne.properties.Add(new Property { key = "color", value = "red" });
            productOne.identifiers.Add(new Identifier { key = "vin", value = "123" });
            productOne.url = "http://www.google.com";

            return productOne;
        }
        
        private bool StringListsAreEqual(List<string> list1, List<string> list2)
        {
            if (list1.Count != list2.Count)
                return false;

            for (int i = 0; i < list1.Count; i++)
            {
                if (string.Compare(list1[i], list2[i]) != 0)
                    return false;
            }

            return true;
        }

        #endregion Private Methods

        [TestMethod]
        public void CreateProductWithSimplePropertiesSucceeds()
        {
            // Arrange
            var productToCreate = new Product { 
                fn = "test product", 
                description = "product for unit test", 
                brand = "Kraft",
                url = "http://www.google.com"
            };
            
            // Act
            _sut.CreateProduct(productToCreate);

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(productToCreate.Id));
            Assert.IsNotNull(productToCreate.createdAt);
            Assert.IsNotNull(productToCreate.updatedAt);
            // Now GET the newly created Product and make sure attributes are correct
            var createdProduct = _sut.GetProduct(productToCreate.Id);
            Assert.AreEqual(productToCreate.fn, createdProduct.fn);
            Assert.AreEqual(productToCreate.description, createdProduct.description);
            Assert.AreEqual(productToCreate.brand, createdProduct.brand);
            Assert.AreEqual(productToCreate.url, createdProduct.url);
                        
            _sut.DeleteProduct(productToCreate.Id);
        }

        [TestMethod]
        public void InstantiatesProductListAreNotNull()
        {
            var productToCreate = new Product { fn = "test product" };

            Assert.IsNotNull(productToCreate.identifiers);
            Assert.IsNotNull(productToCreate.photos);
            Assert.IsNotNull(productToCreate.tags);
            Assert.IsNotNull(productToCreate.properties);
            Assert.IsNotNull(productToCreate.categories);

            productToCreate = null;
        }

        [TestMethod]
        public void CreateProductWithCategoriesSucceeds()
        {
            // Arrange
            var productToCreate = new Product { fn = "test product" };
            productToCreate.categories.Add("bolts");
            productToCreate.categories.Add("nuts");

            // Act
            _sut.CreateProduct(productToCreate);

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(productToCreate.Id));
            var createdProduct = _sut.GetProduct(productToCreate.Id);
            Assert.IsTrue(StringListsAreEqual(productToCreate.categories, createdProduct.categories));

            _sut.DeleteProduct(productToCreate.Id);
        }
        
        [TestMethod]
        public void CreateProductWithTagsSucceeds()
        {
            var productToCreate = new Product { fn = "test product" };
            productToCreate.tags.Add("bolts");
            productToCreate.tags.Add("nuts");

            _sut.CreateProduct(productToCreate);

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(productToCreate.Id));
            var createdProduct = _sut.GetProduct(productToCreate.Id);
            Assert.IsTrue(StringListsAreEqual(productToCreate.tags, createdProduct.tags));

            _sut.DeleteProduct(productToCreate.Id);
        }

        [TestMethod]
        public void CreateProductWithPhotosSucceeds()
        {
            var productToCreate = new Product { fn = "test product" };
            productToCreate.photos.Add("http://arduino.cc/en/uploads/Main/ArduinoUno_R3_Front_450px.jpg");
            productToCreate.photos.Add("http://arduino.cc/en/uploads/Main/arduino_due_in_hand.jpg");

            _sut.CreateProduct(productToCreate);

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(productToCreate.Id));
            var createdProduct = _sut.GetProduct(productToCreate.Id);
            Assert.IsTrue(StringListsAreEqual(productToCreate.photos, createdProduct.photos));

            _sut.DeleteProduct(productToCreate.Id);
        }

        [TestMethod]
        public void CreateProductWithPropertiesSucceeds()
        {
            // Arrange
            var productToCreate = new Product { fn = "test product" };
            productToCreate.properties.Add(new Property { key = "color", value = "red" } );
            productToCreate.properties.Add(new Property { key = "weight", value = "180" });

            // Act
            _sut.CreateProduct(productToCreate);

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(productToCreate.Id));
            var createdProduct = _sut.GetProduct(productToCreate.Id);
            Assert.IsTrue(createdProduct.properties.Count == 2);

            var colorProperty = (from p in createdProduct.properties
                                    where p.key == "color"
                                    select p).FirstOrDefault();

            Assert.IsTrue(colorProperty.value == "red");

            var weightProperty = (from p in createdProduct.properties
                                  where p.key == "weight"
                                  select p).FirstOrDefault();

            Assert.IsTrue(weightProperty.value == "180");
            
            _sut.DeleteProduct(productToCreate.Id);
        }

        [TestMethod]
        public void CreateProductWithIdentifiersSucceeds()
        {
            // Arrange
            var productToCreate = new Product { fn = "test product" };
            productToCreate.identifiers.Add(new Identifier { key = "isbn", value = "12sd34dg" });
            productToCreate.identifiers.Add(new Identifier { key = "vin", value = "123456677" });

            // Act
            _sut.CreateProduct(productToCreate);

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(productToCreate.Id));
            var createdProduct = _sut.GetProduct(productToCreate.Id);
            Assert.IsTrue(createdProduct.identifiers.Count == 2);

            var isbnIdentifier = (from i in createdProduct.identifiers
                                 where i.key == "isbn"
                                 select i).FirstOrDefault();

            Assert.IsTrue(isbnIdentifier.value == "12sd34dg");

            var vinIdentifier = (from i in createdProduct.identifiers
                                  where i.key == "vin"
                                  select i).FirstOrDefault();

            Assert.IsTrue(vinIdentifier.value == "123456677");

            _sut.DeleteProduct(productToCreate.Id);
        }

        [TestMethod]
        public void GetProductsTest()
        {            
            var beginTime = DateTime.Now.AddMinutes(-5.0);

            var productOne = CreateTestProduct();
            
            var productTwo = new Product();
            productTwo.fn = "prod2";
            productTwo.brand = "mazda";
            productTwo.description = "my second prod";
            productTwo.categories.Add("cars");
            productTwo.tags.Add("trucks");
            productTwo.photos.Add("http://www.google.com");
            productTwo.properties.Add(new Property { key = "color", value = "blue" });
            productTwo.identifiers.Add(new Identifier { key = "vin", value = "456" });
            productTwo.url = "http://www.google.com";

            _sut.CreateProduct(productOne);
            _sut.CreateProduct(productTwo);

            var endTime = DateTime.Now.AddMinutes(5);

            var myProducts = _sut.GetProducts();
           
            var prod1 = (from p in myProducts
                          where p.fn == "prod1" && p.createdAt > beginTime && p.createdAt < endTime
                          select p).FirstOrDefault();

            var prod2 = (from p in myProducts
                         where p.fn == "prod2" && p.createdAt > beginTime && p.createdAt < endTime
                         select p).FirstOrDefault();

            Assert.AreEqual("ford", prod1.brand);
            Assert.AreEqual("mazda", prod2.brand);
                                    
            _sut.DeleteProduct(productOne.Id);
            _sut.DeleteProduct(productTwo.Id);

        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void GetNonexistantProductThrows()
        {
            var notProduct = _sut.GetProduct("does1not2exist3");
        }

        [TestMethod]
        public void UpdateProduct_fnBrandDescriptionUrl()
        {
            // Arrange
            var productOne = new Product();
            productOne.fn = "prod1a";
            productOne.brand = "forda";
            productOne.description = "my first prod a";
            productOne.categories.Add("cars");
            productOne.tags.Add("trucks");
            productOne.photos.Add("http://www.google.com");
            productOne.properties.Add(new Property { key = "color", value = "red" });
            productOne.identifiers.Add(new Identifier { key = "vin", value = "123" });
            productOne.url = "http://www.google.com";

            _sut.CreateProduct(productOne);
            
            // Act
            productOne.fn = "test me";
            productOne.description = "changed desc";
            productOne.brand = "chevy";
            productOne.url = "http://www.yahoo.com";
            _sut.UpdateProduct(productOne);

            // Assert
            var updatedProduct = _sut.GetProduct(productOne.Id);
            Assert.AreEqual<string>("test me", updatedProduct.fn);
            Assert.AreEqual<string>("changed desc", updatedProduct.description);
            Assert.AreEqual<string>("chevy", updatedProduct.brand);
            Assert.AreEqual<string>("http://www.yahoo.com", updatedProduct.url);

            _sut.DeleteProduct(productOne.Id);

        }

        [TestMethod]
        public void UpdateProduct_WithNullValues()
        {
            // Arrange
            var productOne = new Product();
            productOne.fn = "prod1a";
            productOne.brand = "forda";
            productOne.description = "my first prod a";
            productOne.categories.Add("cars");
            productOne.tags.Add("trucks");
            productOne.photos.Add("http://www.google.com");
            productOne.properties.Add(new Property { key = "color", value = "red" });
            productOne.identifiers.Add(new Identifier { key = "vin", value = "123" });
            productOne.url = "http://www.google.com";

            _sut.CreateProduct(productOne);

            // Act
            productOne.fn = null;
            productOne.description = null;
            productOne.brand = null;
            productOne.url = null;
            _sut.UpdateProduct(productOne);

            // Assert
            var updatedProduct = _sut.GetProduct(productOne.Id);
            // Repository will not allow fn to be null or empty.  The ProductService should not allow this,
            // so it should never get this far, but just in case...
            Assert.AreNotEqual<string>(string.Empty, updatedProduct.fn);
            // Description and brand can be empty
            Assert.AreEqual<string>(string.Empty, updatedProduct.description);
            Assert.AreEqual<string>(string.Empty, updatedProduct.brand);
            Assert.AreEqual<string>(string.Empty, updatedProduct.url);

            _sut.DeleteProduct(productOne.Id);

        }

        [TestMethod]
        public void UpdateProduct_AddCategoriesTagsPhotos()
        {
            // Arrange
            var productOne = new Product();
            productOne.fn = "prod1a";
            productOne.brand = "forda";
            productOne.description = "my first prod a";
            productOne.categories.Add("cars");
            productOne.tags.Add("trucks");
            productOne.photos.Add("http://www.google.com");
            productOne.properties.Add(new Property { key = "color", value = "red" });
            productOne.identifiers.Add(new Identifier { key = "vin", value = "123" });
            productOne.url = "http://www.google.com";

            _sut.CreateProduct(productOne);

            // Act
            productOne.categories.Add("boats");
            productOne.tags.Add("used");
            productOne.photos.Add("http://www.flicker.com");
            _sut.UpdateProduct(productOne);

            // Assert
            var updatedProduct = _sut.GetProduct(productOne.Id);            
            Assert.AreEqual(2, updatedProduct.categories.Count);
            Assert.AreEqual(2, updatedProduct.tags.Count);
            Assert.AreEqual(2, updatedProduct.photos.Count);

            _sut.DeleteProduct(productOne.Id);
        }

        [TestMethod]
        public void UpdateProduct_ChangeCategoriesTagsPhotos()
        {
            // Arrange
            var productOne = new Product();
            productOne.fn = "prod1a";
            productOne.brand = "forda";
            productOne.description = "my first prod a";
            productOne.categories.Add("cars");
            productOne.tags.Add("trucks");
            productOne.photos.Add("http://www.google.com");
            productOne.properties.Add(new Property { key = "color", value = "red" });
            productOne.identifiers.Add(new Identifier { key = "vin", value = "123" });
            productOne.url = "http://www.google.com";

            _sut.CreateProduct(productOne);

            // Act
            productOne.categories[0] = "boats";
            productOne.tags[0] = "used";
            productOne.photos[0] = "http://www.flicker.com";
            _sut.UpdateProduct(productOne);

            // Assert
            var updatedProduct = _sut.GetProduct(productOne.Id);            
            Assert.AreEqual("boats", updatedProduct.categories[0]);
            Assert.AreEqual("used", updatedProduct.tags[0]);
            Assert.AreEqual("http://www.flicker.com", updatedProduct.photos[0]);

            _sut.DeleteProduct(productOne.Id);
        }

        [TestMethod]
        public void UpdateProduct_RemoveCategoriesTagsPhotos()
        {
            // Arrange
            var productOne = new Product();
            productOne.fn = "prod1a";
            productOne.brand = "forda";
            productOne.description = "my first prod a";
            productOne.categories.Add("cars");
            productOne.tags.Add("trucks");
            productOne.photos.Add("http://www.google.com");
            productOne.properties.Add(new Property { key = "color", value = "red" });
            productOne.identifiers.Add(new Identifier { key = "vin", value = "123" });
            productOne.url = "http://www.google.com";

            _sut.CreateProduct(productOne);

            // Act
            productOne.categories.Remove("cars");
            productOne.tags.Remove("trucks");
            productOne.photos.Remove("http://www.google.com");
            _sut.UpdateProduct(productOne);

            // Assert
            var updatedProduct = _sut.GetProduct(productOne.Id);
            // The ProductService should not allow Categories to be null, but Categories could be empty
            Assert.AreEqual(0, updatedProduct.categories.Count);
            Assert.AreEqual(0, updatedProduct.tags.Count);
            Assert.AreEqual(0, updatedProduct.photos.Count);
           
            _sut.DeleteProduct(productOne.Id);
        }

        //[TestMethod]
        //public void CleanUp()
        //{

        //    var myProducts = _sut.GetProducts();

        //    foreach (Product p in myProducts)
        //    {
        //        if (string.Compare("51d80ea0e4b0f5b53dd932b5", p.Id) != 0)
        //        {
        //            _sut.DeleteProduct(p.Id);
        //        }
        //    }

        //}
    }
}
