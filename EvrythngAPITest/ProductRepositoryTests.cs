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
    }
}
