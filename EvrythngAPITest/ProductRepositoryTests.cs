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
            var productOne = CreateTestProduct();            
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
            var productOne = CreateTestProduct();            
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
            var productOne = CreateTestProduct();
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
            var productOne = CreateTestProduct();
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
            var productOne = CreateTestProduct();            
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

        [TestMethod]
        public void UpdateProduct_CannotAddProperty()
        {
            // Arrange
            var productOne = CreateTestProduct();
            _sut.CreateProduct(productOne);

            // Act
            productOne.properties.Add(new Property { key = "size", value = "large" });
            
            _sut.UpdateProduct(productOne);

            // Assert
            var updatedProduct = _sut.GetProduct(productOne.Id);            
            Assert.AreNotEqual(2, updatedProduct.properties.Count);

            _sut.DeleteProduct(productOne.Id);
        }

        [TestMethod]
        public void UpdateProduct_CannotModifiyProperty()
        {
            // Arrange
            var productOne = CreateTestProduct();
            _sut.CreateProduct(productOne);

            // Act
            // Try to change 'color' property from 'red' to 'blue'
            productOne.properties[0].value = "blue";

            _sut.UpdateProduct(productOne);

            // Assert
            var updatedProduct = _sut.GetProduct(productOne.Id);
            // The 'color' Property will not have changed - still 'red', not 'blue'
            Assert.AreEqual("red", updatedProduct.properties[0].value);

            _sut.DeleteProduct(productOne.Id);
        }

        [TestMethod]
        public void UpdateProduct_CannotRemoveProperty()
        {
            // Arrange
            var productOne = CreateTestProduct();
            _sut.CreateProduct(productOne);

            // Act
            // Try to change 'color' property from 'red' to 'blue'
            productOne.properties.RemoveAt(0);

            _sut.UpdateProduct(productOne);

            // Assert
            var updatedProduct = _sut.GetProduct(productOne.Id);
            // There will still be one Property, even though we tried to remove it
            Assert.AreNotEqual(0, updatedProduct.properties.Count);

            _sut.DeleteProduct(productOne.Id);
        }

        [TestMethod]
        public void UpdateProduct_AddIdentifier()
        {
            // Arrange
            var productOne = CreateTestProduct();
            _sut.CreateProduct(productOne);

            // Act
            productOne.identifiers.Add(new Identifier { key = "size", value = "large" });

            _sut.UpdateProduct(productOne);

            // Assert
            var updatedProduct = _sut.GetProduct(productOne.Id);            
            Assert.AreEqual(2, updatedProduct.identifiers.Count);

            _sut.DeleteProduct(productOne.Id);
        }

        [TestMethod]
        public void UpdateProduct_ModifyIdentifier()
        {
            // Arrange
            var productOne = CreateTestProduct();
            _sut.CreateProduct(productOne);

            // Act
            productOne.identifiers[0].value = "small";

            _sut.UpdateProduct(productOne);

            // Assert
            var updatedProduct = _sut.GetProduct(productOne.Id);
            Assert.AreEqual("small", updatedProduct.identifiers[0].value);

            _sut.DeleteProduct(productOne.Id);
        }

        [TestMethod]
        public void UpdateProduct_RemoveIdentifier()
        {
            // Arrange
            var productOne = CreateTestProduct();
            _sut.CreateProduct(productOne);

            // Act
            productOne.identifiers.RemoveAt(0);

            _sut.UpdateProduct(productOne);

            // Assert
            var updatedProduct = _sut.GetProduct(productOne.Id);
            Assert.AreEqual(0, updatedProduct.identifiers.Count);

            _sut.DeleteProduct(productOne.Id);
        }

        #region Properties Tests

        [TestMethod]
        public void GetPropertiesWhenPropertiesExistSucceeds()
        {
            // Arrange
            var getPropsProduct = new Product { fn = "Get Product Properties Test" };
            getPropsProduct.properties.Add(new Property { key = "color", value = "white", timestamp = DateTime.Now });
            getPropsProduct.properties.Add(new Property { key = "speed", value = "fast", timestamp = DateTime.Now });
            _sut.CreateProduct(getPropsProduct);
            var originalUpdateDate = getPropsProduct.updatedAt;

            // Act
            Assert.IsFalse(string.IsNullOrEmpty(getPropsProduct.Id));
            var myProperties = _sut.GetProperties(getPropsProduct.Id);

            // Assert
            Assert.IsTrue(getPropsProduct.properties.Count == 2);
            Assert.IsTrue(myProperties.Count == 2);
            Assert.AreEqual<string>(getPropsProduct.properties[0].value, myProperties[0].value);
            Assert.AreEqual<string>(getPropsProduct.properties[1].value, myProperties[1].value);
            Assert.IsNotNull(myProperties[0].timestamp);
            Assert.IsNotNull(myProperties[0].timestamp);

            _sut.DeleteProduct(getPropsProduct.Id);
        }

        [TestMethod]
        public void GetProperties_NoProperties_ReturnsEmptyList()
        {            
            // Arrange
            var getPropsProduct = new Product { fn = "Get Product Properties Test" };
            _sut.CreateProduct(getPropsProduct);
            var originalUpdateDate = getPropsProduct.updatedAt;

            // Act
            Assert.IsFalse(string.IsNullOrEmpty(getPropsProduct.Id));
            var myProperties = _sut.GetProperties(getPropsProduct.Id);

            // Assert            
            Assert.IsNotNull(myProperties);
            Assert.IsTrue(myProperties.Count == 0);

            _sut.DeleteProduct(getPropsProduct.Id);
        }

        [TestMethod]
        public void CreateUpdateNewPropertiesSucceeds()
        {
            // Arrange
            var createpropsProduct = new Product { fn = "Get Product Properties Test" };
            createpropsProduct.properties.Add(new Property { key = "color", value = "white", timestamp = DateTime.Now });
            _sut.CreateProduct(createpropsProduct);
            var originalUpdateDate = createpropsProduct.updatedAt;                        

            var propsToCreate = new List<Property>();
            propsToCreate.Add(new Property { key = "weight", value = "165", timestamp = DateTime.Now });
            propsToCreate.Add(new Property { key = "height", value = "68", timestamp = DateTime.Now });
            propsToCreate.Add(new Property { key = "length", value = "46", timestamp = DateTime.Now });

            // Act
            var result = _sut.CreateUpdateProperties(createpropsProduct.Id, propsToCreate);

            // Assert
            Assert.IsNotNull(result);
            // The returned Properties are only those that were sent in the Create/Update request,
            // not the total number.
            Assert.IsTrue(result.Count == 3);

            // Total Properties on the Product should now be 4
            var productWithMoreProps = _sut.GetProduct(createpropsProduct.Id);
            Assert.IsTrue(productWithMoreProps.properties.Count == 4);

            // Also GetProperties should return 4
            var propsAgain = _sut.GetProperties(createpropsProduct.Id);
            Assert.IsTrue(propsAgain.Count == 4);

            _sut.DeleteProduct(createpropsProduct.Id);
        }

        /// <summary>
        /// This test proves that updating a property using the /properties endpoint 
        /// does not add a new property - just updates it, nor does it add history to a property.
        /// </summary>
        [TestMethod]
        public void UpdateExistingPropertyValueAtPropertiesEndpoint()
        {
            // Arrange
            var updatePropsProduct = new Product { fn = "Get Product Properties Test" };
            updatePropsProduct.properties.Add(new Property { key = "color", value = "white", timestamp = DateTime.Now });
            updatePropsProduct.properties.Add(new Property { key = "speed", value = "fast", timestamp = DateTime.Now });
            _sut.CreateProduct(updatePropsProduct);
            var originalUpdateDate = updatePropsProduct.updatedAt;                        

            // Act
            Assert.IsFalse(string.IsNullOrEmpty(updatePropsProduct.Id));
            var myProperties = _sut.GetProperties(updatePropsProduct.Id);

            // Assert            
            Assert.IsTrue(myProperties.Count == 2);

            var colorProperty = myProperties.Find(p => p.key == "color");
            colorProperty.value = "blue";

            // Update the "color" property and check
            var result = _sut.CreateUpdateProperties(updatePropsProduct.Id, myProperties);
            Assert.IsTrue(result.Count == 2);
            var newColor = result.Find(p => p.key == "color");
            Assert.AreEqual<string>("blue", newColor.value);

            // Get all properties - what's there?
            var propsAgain = _sut.GetProperties(updatePropsProduct.Id);
            Assert.IsTrue(propsAgain.Count == 2);

            // Get the PropertyHistory for 'color'
            var colorPropertyHistory = _sut.GetPropertyHistory(updatePropsProduct.Id, "color");
            Assert.IsTrue(colorPropertyHistory.Count == 1);

            _sut.DeleteProduct(updatePropsProduct.Id);

        }

        [TestMethod]
        public void GetPropertyHistorySucceeds()
        {
            // Arrange
            var testProduct = new Product { fn = "Get Product Properties Test" };
            testProduct.properties.Add(new Property { key = "color", value = "white", timestamp = DateTime.Now });
            _sut.CreateProduct(testProduct);
            var originalUpdateDate = testProduct.updatedAt;
                        
            // Act
            Assert.IsFalse(string.IsNullOrEmpty(testProduct.Id));
            var propertyHistory = _sut.GetPropertyHistory(testProduct.Id, "color");

            // Assert
            Assert.IsNotNull(propertyHistory);
            Assert.IsTrue(propertyHistory.Count == 1);
            Assert.AreEqual("white", propertyHistory[0].value);

            _sut.DeleteProduct(testProduct.Id);

        }

        /// <summary>
        /// Updating a single property adds history with new value and timestamp.
        /// </summary>
        [TestMethod]
        public void UpdatePropertyAtPropertyEndpointAddsHistory()
        {
            // Arrange
            var testProduct = new Product { fn = "Get Product Properties Test" };
            testProduct.properties.Add(new Property { key = "color", value = "white", timestamp = DateTime.Now });
            _sut.CreateProduct(testProduct);
            var originalUpdateDate = testProduct.updatedAt;

            Assert.IsFalse(string.IsNullOrEmpty(testProduct.Id));
            var propertyHistory = _sut.GetPropertyHistory(testProduct.Id, "color");
            Assert.IsNotNull(propertyHistory);
            Assert.IsTrue(propertyHistory.Count == 1);
            Assert.AreEqual("white", propertyHistory[0].value);

            // Act
            var propertyToUpdate = propertyHistory[0];
            propertyToUpdate.value = "violet";
            propertyToUpdate.timestamp = DateTime.Now.AddDays(-1);
            _sut.UpdateProperty(testProduct.Id, propertyToUpdate);
            // Get History a second time
            propertyHistory = _sut.GetPropertyHistory(testProduct.Id, "color");

            // Assert
            Assert.IsTrue(propertyHistory.Count == 2);

            _sut.DeleteProduct(testProduct.Id);

        }

        /// <summary>
        /// Updating a single property with multiple values
        /// </summary>
        [TestMethod]
        public void UpdatePropertyWithMultipleValuesAndTimestamps()
        {
            // Arrange
            var testProduct = new Product { fn = "Update Product Properties Test" };
            var whiteTime = DateTime.Now.AddMinutes(1);
            testProduct.properties.Add(new Property { key = "color", value = "white", timestamp = whiteTime });
            _sut.CreateProduct(testProduct);
            var originalUpdateDate = testProduct.updatedAt;

            Assert.IsFalse(string.IsNullOrEmpty(testProduct.Id));
            var propertyHistory = _sut.GetPropertyHistory(testProduct.Id, "color");
            Assert.IsNotNull(propertyHistory);
            Assert.IsTrue(propertyHistory.Count == 1);
            Assert.AreEqual("white", propertyHistory[0].value);

            // Act
            // Add multiple property values
            var blueTime = DateTime.Now;
            var redTime = DateTime.Now.AddHours(1);
            var greenTime = DateTime.Now.AddHours(2);

            var blue = new Property { key = "color", value = "blue", timestamp = blueTime };
            var red = new Property { key = "color", value = "red", timestamp = redTime };
            var green = new Property { key = "color", value = "green", timestamp = greenTime };

            propertyHistory.Add(blue);
            propertyHistory.Add(red);
            propertyHistory.Add(green);
            _sut.UpdateProperty(testProduct.Id, propertyHistory);

            Assert.IsTrue(propertyHistory.Count == 4);

            // Get History a second time
            var propertyHistory2 = _sut.GetPropertyHistory(testProduct.Id, "color");
            var colorWhite = propertyHistory2.Find(p => p.value == "white");
            var colorBlue = propertyHistory2.Find(p => p.value == "blue");
            var colorRed = propertyHistory2.Find(p => p.value == "red");
            var colorGreen = propertyHistory2.Find(p => p.value == "green");

            // Assert
            Assert.IsTrue(propertyHistory2.Count == 4);
            Assert.IsNotNull(colorWhite);
            Assert.IsNotNull(colorBlue);
            Assert.IsNotNull(colorRed);
            Assert.IsNotNull(colorGreen);

            _sut.DeleteProduct(testProduct.Id);

        }

        /// <summary>
        /// Get a single property and specify a time interval
        /// </summary>
        [TestMethod]
        public void GetPropertyUsingTimeInterval()
        {
            // Arrange
            var testProduct = new Product { fn = "Update Product Properties Test" };
            var whiteTime = DateTime.Now;
            testProduct.properties.Add(new Property { key = "color", value = "white", timestamp = whiteTime });
            _sut.CreateProduct(testProduct);
            var originalUpdateDate = testProduct.updatedAt;

            Assert.IsFalse(string.IsNullOrEmpty(testProduct.Id));
            var propertyHistory = _sut.GetPropertyHistory(testProduct.Id, "color");
            Assert.IsNotNull(propertyHistory);
            Assert.IsTrue(propertyHistory.Count == 1);
            Assert.AreEqual("white", propertyHistory[0].value);

            // Act
            // Add 3 more colors with specified timestamps

            var blueTime = new DateTime(2008, 1, 1);
            var redTime = blueTime.AddYears(3);
            var greenTime = blueTime.AddYears(4);

            var blue = new Property { key = "color", value = "blue", timestamp = (DateTime?)blueTime };
            var red = new Property { key = "color", value = "red", timestamp = (DateTime?)redTime };
            var green = new Property { key = "color", value = "green", timestamp = (DateTime?)greenTime };

            propertyHistory.Add(blue);
            propertyHistory.Add(red);
            propertyHistory.Add(green);
            _sut.UpdateProperty(testProduct.Id, propertyHistory);

            Assert.IsTrue(propertyHistory.Count == 4);

            // Get History in time - the request should only return 2 colors
            var beginTime = new DateTime(2010, 1, 1);
            var endTime = new DateTime(2013, 1, 1);
            var propertyHistory2 = _sut.GetPropertyHistory(testProduct.Id, "color", (DateTime?)beginTime, (DateTime?)endTime);

            var colorRed = propertyHistory2.Find(p => p.value == "red");
            var colorGreen = propertyHistory2.Find(p => p.value == "green");

            // Assert  - only Red and Green should be returned
            Assert.IsTrue(propertyHistory2.Count == 2);
            Assert.IsNotNull(colorRed);
            Assert.IsNotNull(colorGreen);

            _sut.DeleteProduct(testProduct.Id);

        }

        [TestMethod]
        public void DeleteSingleProperty()
        {
            // Arrange
            var testProduct = new Product { fn = "Update Product Properties Test" };
            testProduct.properties.Add(new Property { key = "color", value = "white", timestamp = DateTime.Now });
            testProduct.properties.Add(new Property { key = "weight", value = "165", timestamp = DateTime.Now });
            testProduct.properties.Add(new Property { key = "height", value = "68", timestamp = DateTime.Now });
            testProduct.properties.Add(new Property { key = "length", value = "46", timestamp = DateTime.Now });

            _sut.CreateProduct(testProduct);
            var originalUpdateDate = testProduct.updatedAt;
            var originalProperties = _sut.GetProperties(testProduct.Id);

            // Act                        
            _sut.DeleteProperty(testProduct.Id, "color");
            var propertiesAfterDelete = _sut.GetProperties(testProduct.Id);

            // Assert
            Assert.AreEqual(4, originalProperties.Count);
            Assert.AreEqual(3, propertiesAfterDelete.Count);

            _sut.DeleteProduct(testProduct.Id);
        }




        #endregion Properties Tests

        [TestMethod]
        public void CleanUp()
        {

            var myProducts = _sut.GetProducts();

            foreach (Product p in myProducts)
            {
                if (string.Compare("51d80ea0e4b0f5b53dd932b5", p.Id) != 0)
                {
                    _sut.DeleteProduct(p.Id);
                }
            }

            var cleanedProducts = _sut.GetProducts();
            Assert.AreEqual(1, cleanedProducts.Count);

        }
    }
}
