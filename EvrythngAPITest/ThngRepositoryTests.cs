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


namespace EvrythngAPITest
{
    /// <summary>
    /// Summary description for ThngRepositoryTests
    /// </summary>
    [TestClass]
    public class ThngRepositoryTests
    {
        #region Private Members

        private TestContext testContextInstance;
        private static string _apiKey;
        private static string _apiBaseAddress;
        private Thng _baseTestThng;
        private static IThngRepository _sut;

        #endregion Private Members

        #region Private Methods

        private void DeleteTestThng()
        {
            using (var httpClient = new HttpClient() { BaseAddress = new Uri(_apiBaseAddress) })
            {
                // Set Authorization Header
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(_apiKey);

                var task = httpClient.DeleteAsync("thngs/" + _baseTestThng.Id)
                    .ContinueWith((taskwithmsg) =>
                    {
                        var response = taskwithmsg.Result;
                        var jsonTask = response.Content.ReadAsStringAsync();
                        jsonTask.Wait();
                        var jsonResult = jsonTask.Result;
                        Console.Write(jsonResult.ToString());

                        if (!response.IsSuccessStatusCode)
                        {
                            Console.WriteLine("FAIL.  StatusCode: " + response.StatusCode.ToString());
                            Console.WriteLine("Reason: " + response.ReasonPhrase);
                        }
                        else
                        {
                            //dynamic evrythngResponse = JObject.Parse(jsonResult);
                            Console.WriteLine("Response StatusCode: " + response.StatusCode);
                            _baseTestThng = null;
                        }

                    });
                task.Wait();

            }


        }

        private void CreateBaseTestThng()
        {
            _baseTestThng = new Thng();

            using (var httpClient = new HttpClient() { BaseAddress = new Uri(_apiBaseAddress) })
            {
                // Set Authorization Header
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(_apiKey);

                // Use dynamic JObject to create Json
                dynamic dynamicThng = new JObject();
                dynamicThng.name = "Unit Test Thng";

                var content = new StringContent(dynamicThng.ToString());
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var task = httpClient.PostAsync("thngs", content)
                    .ContinueWith((taskwithmsg) =>
                    {
                        var response = taskwithmsg.Result;
                        var jsonTask = response.Content.ReadAsStringAsync();
                        jsonTask.Wait();
                        var jsonResult = jsonTask.Result;
                        Console.Write(jsonResult.ToString());

                        if (!response.IsSuccessStatusCode)
                        {
                            Console.WriteLine("FAIL.  StatusCode: " + response.StatusCode.ToString());
                            Console.WriteLine("Reason: " + response.ReasonPhrase);
                        }
                        else
                        {
                            dynamic evrythngResponse = JObject.Parse(jsonResult);
                            _baseTestThng.Id = evrythngResponse.id;
                            _baseTestThng.name = evrythngResponse.name;
                        }

                    });
                task.Wait();


            }
        }

        #region Sandbox Code
        private void CreateNewTestThng()
        {
            using (var httpClient = new HttpClient() { BaseAddress = new Uri(_apiBaseAddress) })
            {
                // Set Authorization Header
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(_apiKey);
                
                // Create a Thng
                var thngToTest = new Thng();
                thngToTest.name = "My test thng 5";
                thngToTest.description = " a thng to test, then dispose";
                thngToTest.tags = new List<string> { "new", "test", "disposable" };                
                thngToTest.properties = new List<Property>();
                thngToTest.properties.Add( new Property(){ key = "color", value = "black"});
                thngToTest.properties.Add( new Property(){ key = "cost", value = "0"});
                thngToTest.location = new Location { latitude = 37, longitude = 108, timestamp = new DateTime(1974, 3, 15) };

                // Use dynamic JObject to create Json
                dynamic dynamicThng = new JObject();
                dynamicThng.name = thngToTest.name;

                // Create a new JObject for Thng.properties
                dynamic dynamicProperties = new JObject();
                foreach (var p in thngToTest.properties)
                {
                    var jProp = new JProperty(p.key, p.value);
                    dynamicProperties.Add(jProp);
                }
                dynamicThng.properties = dynamicProperties;

                // Likewise for Thng.Location
                dynamic dynamicLocation = new JObject();
                dynamicLocation.latitude = thngToTest.location.latitude;
                dynamicLocation.longitude = thngToTest.location.longitude;
                dynamicLocation.timestamp = Utilities.MillisecondsSinceEpoch(thngToTest.location.timestamp.Value);
                dynamicThng.location = dynamicLocation; 
                
                Console.WriteLine(dynamicThng.ToString());

                var content = new StringContent(dynamicThng.ToString());
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                                                
                var task = httpClient.PostAsync("thngs", content)                
                    .ContinueWith((taskwithmsg) =>
                    {
                        var response = taskwithmsg.Result;
                        if (!response.IsSuccessStatusCode)
                        {
                            Console.WriteLine("FAIL.  StatusCode: " + response.StatusCode.ToString());
                            Console.WriteLine("Reason: " + response.ReasonPhrase);
                        }

                        var jsonTask = response.Content.ReadAsStringAsync();
                        jsonTask.Wait();
                        var jsonResult = jsonTask.Result;
                        Console.Write(jsonResult.ToString());
                    });
                task.Wait();

            }
        }
        #endregion Sandbox Code

        #endregion Private Methods


        public ThngRepositoryTests()
        {
            _apiKey = ConfigurationManager.AppSettings["APIKey"].ToString();
            _apiBaseAddress = ConfigurationManager.AppSettings["APIBaseAddress"].ToString();
            _sut = new ThngRepository();
            //CreateBaseTestThng();
        }

        
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize() 
        {
            //CreateBaseTestThng();
            //Assert.IsNotNull(_baseTestThng);
        }
        
        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup() 
        {
            //DeleteTestThng();
            //Assert.IsNull(_baseTestThng);
        }
        
        #endregion

        #region Create Tests

        [TestMethod]
        public void CreateWithNameAndDescriptionSucceeds()
        {
            var thngToCreate = new Thng { name = "create thng", description = "create test" };

            _sut.CreateThng(thngToCreate);

            Assert.IsFalse(string.IsNullOrEmpty(thngToCreate.Id));
            Assert.IsNotNull(thngToCreate.createdAt);
            Assert.IsNotNull(thngToCreate.updatedAt);

            _sut.DeleteThng(thngToCreate.Id);

        }

        [TestMethod]
        public void CreateWithTagsSucceeds()
        {
            var thngToCreate = new Thng { name = "create thng", description = "create test" };
            thngToCreate.tags = new List<string> { "test", "unit", "necessary" };

            _sut.CreateThng(thngToCreate);

            Assert.IsFalse(string.IsNullOrEmpty(thngToCreate.Id));
            Assert.IsNotNull(thngToCreate.tags);
            Assert.IsTrue(thngToCreate.tags.Count == 3);

            _sut.DeleteThng(thngToCreate.Id);

        }

        [TestMethod]
        public void CreateWithNullTagsSucceeds()
        {
            var thngToCreate = new Thng { name = "create thng", description = "create test" };
            thngToCreate.tags = null;

            _sut.CreateThng(thngToCreate);

            Assert.IsFalse(string.IsNullOrEmpty(thngToCreate.Id));
            Assert.IsNull(thngToCreate.tags);

            _sut.DeleteThng(thngToCreate.Id);

        }

        [TestMethod]
        public void CreateWithPropertiesSucceeds()
        {
            var thngToCreate = new Thng { name = "create thng", description = "create test" };
            thngToCreate.properties = new List<Property>()
            {
                new Property{ key = "node1", value = "on" },
                new Property{ key = "node2", value = "off" }
            };

            _sut.CreateThng(thngToCreate);

            Assert.IsFalse(string.IsNullOrEmpty(thngToCreate.Id));
            Assert.IsNotNull(thngToCreate.properties);
            Assert.IsTrue(thngToCreate.properties.Count == 2);

            _sut.DeleteThng(thngToCreate.Id);
        }

        [TestMethod]
        public void CreateWithNullPropertiesSucceeds()
        {
            var thngToCreate = new Thng { name = "create thng", description = "create test" };
            thngToCreate.properties = null;

            _sut.CreateThng(thngToCreate);

            Assert.IsFalse(string.IsNullOrEmpty(thngToCreate.Id));
            Assert.IsNull(thngToCreate.properties);

            _sut.DeleteThng(thngToCreate.Id);
        }

        [TestMethod]
        public void CreateWithLocationSucceeds()
        {
            var thngToCreate = new Thng { name = "create thng", description = "create test" };

            thngToCreate.location = new Location { latitude = 30.45, longitude = -50.99 };
            _sut.CreateThng(thngToCreate);

            Assert.IsFalse(string.IsNullOrEmpty(thngToCreate.Id));
            Assert.IsNotNull(thngToCreate.location);

            _sut.DeleteThng(thngToCreate.Id);
        }

        [TestMethod]       
        public void CreateWithLongitudeOnlySucceeds()
        {
            var thngToCreate = new Thng { name = "create thng", description = "create test" };

            // This test will succeed because the default value for latitude will be 0.
            thngToCreate.location = new Location { longitude = -50.99 };
            _sut.CreateThng(thngToCreate);

            Assert.IsFalse(string.IsNullOrEmpty(thngToCreate.Id));

            _sut.DeleteThng(thngToCreate.Id);           
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void BadRequestThrowsException()
        {
            var thngToCreate = new Thng();
            thngToCreate.name = null;

            _sut.CreateThng(thngToCreate);
        }

        #endregion Create Tests

        #region Get Tests

        [TestMethod]
        public void GetMyThngsSucceeds()
        {
            var thngToCreate = new Thng { name = "thng get test" };
            _sut.CreateThng(thngToCreate);

            var myThngs = _sut.GetThngs();

            Assert.IsNotNull(myThngs);
            Assert.IsTrue(myThngs.Exists(t => t.Id == thngToCreate.Id));

            _sut.DeleteThng(thngToCreate.Id); 

        }

        [TestMethod]
        public void GetSingleThngSucceeds()
        {
            var thngToCreate = new Thng { name = "single thng get test" };
                
            _sut.CreateThng(thngToCreate);

            var myThng = _sut.GetThng(thngToCreate.Id);

            Assert.IsNotNull(myThng);
            Assert.IsTrue(myThng.Id == thngToCreate.Id);

            _sut.DeleteThng(thngToCreate.Id);

        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void GetNonexistantThngThrows()
        {            
            var myThng = _sut.GetThng("does1not2exist3");                       
        }


        #endregion Get Tests

        #region Update Thng Tests

        [TestMethod]
        public void UpdateNameSucceedsUpdateDateChanged()
        {
            var name1 = "name before update";
            var name2 = "new name";
            var thngToCreate = new Thng { name = name1 };
            _sut.CreateThng(thngToCreate);
            var originalUpdateDate = thngToCreate.updatedAt;

            thngToCreate.name = name2;

            _sut.UpdateThng(thngToCreate);

            Assert.AreEqual<string>(name2, thngToCreate.name);
            Assert.AreNotEqual<DateTime?>(originalUpdateDate, thngToCreate.updatedAt);

            _sut.DeleteThng(thngToCreate.Id);
        }

        #region Tags

        [TestMethod]
        public void UpdateAddTagSucceeds()
        {
            var updateTestThng = new Thng { name = "tag update" };
            _sut.CreateThng(updateTestThng);
            var originalUpdateDate = updateTestThng.updatedAt;

            updateTestThng.tags.Add("Hello");

            _sut.UpdateThng(updateTestThng);

            Assert.AreNotEqual<DateTime?>(originalUpdateDate, updateTestThng.updatedAt);
            Assert.AreEqual<string>("Hello", updateTestThng.tags[0]);

            _sut.DeleteThng(updateTestThng.Id);
        }

        [TestMethod]
        public void UpdateAddSecondTagSucceeds()
        {
            var updateTestThng = new Thng { name = "tag update" };
            updateTestThng.tags.Add("Hello");
            _sut.CreateThng(updateTestThng);
            var originalUpdateDate = updateTestThng.updatedAt;

            updateTestThng.tags.Add("Goodbye");
            _sut.UpdateThng(updateTestThng);

            Assert.AreNotEqual<DateTime?>(originalUpdateDate, updateTestThng.updatedAt);
            Assert.IsTrue(updateTestThng.tags.Count == 2);

            _sut.DeleteThng(updateTestThng.Id);
        }

        [TestMethod]
        public void UpdateATagSucceeds()
        {
            var updateTestThng = new Thng { name = "tag update" };
            updateTestThng.tags.Add("Hello");
            updateTestThng.tags.Add("Goodbye");
            _sut.CreateThng(updateTestThng);
            var originalUpdateDate = updateTestThng.updatedAt;

            updateTestThng.tags[0] = "Replacement";
            _sut.UpdateThng(updateTestThng);

            Assert.AreNotEqual<DateTime?>(originalUpdateDate, updateTestThng.updatedAt);
            Assert.IsTrue(updateTestThng.tags.Count == 2);
            Assert.AreEqual<string>("Replacement", updateTestThng.tags[0]);

            _sut.DeleteThng(updateTestThng.Id);
        }

        [TestMethod]
        public void UpdateReplaceTagsSucceeds()
        {
            var updateTestThng = new Thng { name = "tag update" };
            updateTestThng.tags.Add("Hello");
            updateTestThng.tags.Add("Goodbye");
            _sut.CreateThng(updateTestThng);
            var originalUpdateDate = updateTestThng.updatedAt;

            var newTags = new List<string>{ "Bad", "News", "Bears" };

            updateTestThng.tags = newTags;
            _sut.UpdateThng(updateTestThng);

            var updatedThng = _sut.GetThng(updateTestThng.Id);

            Assert.AreNotEqual<DateTime?>(originalUpdateDate, updatedThng.updatedAt);
            Assert.IsTrue(updatedThng.tags.Count == 3);
            Assert.IsFalse(updatedThng.tags.Contains("Hello"));
            Assert.IsFalse(updatedThng.tags.Contains("Goodbye"));
            Assert.IsTrue(updatedThng.tags.Contains("Bad"));
            Assert.IsTrue(updatedThng.tags.Contains("News"));
            Assert.IsTrue(updatedThng.tags.Contains("Bears"));
            
            _sut.DeleteThng(updateTestThng.Id);
        }

        #endregion Tags

        #region Properties

        [TestMethod]
        public void UpdateRemoveATagSucceeds()
        {
            var updateTestThng = new Thng { name = "tag update" };
            updateTestThng.tags.Add("Hello");
            updateTestThng.tags.Add("Goodbye");
            _sut.CreateThng(updateTestThng);
            var originalUpdateDate = updateTestThng.updatedAt;

            updateTestThng.tags.Remove("Hello");
            _sut.UpdateThng(updateTestThng);

            Assert.AreNotEqual<DateTime?>(originalUpdateDate, updateTestThng.updatedAt);
            Assert.IsTrue(updateTestThng.tags.Count == 1);
            Assert.AreEqual<string>("Goodbye", updateTestThng.tags[0]);

            _sut.DeleteThng(updateTestThng.Id);
        }

        /// <summary>
        /// The API DOES allow a property to be added at the /thngs endpoint if none already exist
        /// </summary>
        [TestMethod]
        public void UpdateAddAPropertyWhenNoneExistsSucceeds()
        {
            var updateTestThng = new Thng { name = "tag update" };            
            _sut.CreateThng(updateTestThng);
            var originalUpdateDate = updateTestThng.updatedAt;

            updateTestThng.properties.Add(new Property { key = "color", value = "white", timestamp = DateTime.Now });
            _sut.UpdateThng(updateTestThng);

            Assert.AreNotEqual<DateTime?>(originalUpdateDate, updateTestThng.updatedAt);
            Assert.IsTrue(updateTestThng.properties.Count == 1);
            Assert.AreEqual<string>("white", updateTestThng.properties[0].value);

            _sut.DeleteThng(updateTestThng.Id);
        }

        /// <summary>
        /// The API does not allow a new property to be added at the /thngs endpoint if properties already exist
        /// </summary>
        [TestMethod]
        public void UpdateAddAPropertyWhenPropertiesExistFails()
        {
            var updateTestThng = new Thng { name = "tag update" };
            updateTestThng.properties.Add(new Property { key = "color", value = "white", timestamp = DateTime.Now });
            _sut.CreateThng(updateTestThng);
            var originalUpdateDate = updateTestThng.updatedAt;

            updateTestThng.properties.Add(new Property { key = "speed", value = "fast", timestamp = DateTime.Now });
            _sut.UpdateThng(updateTestThng);

            Assert.AreNotEqual<DateTime?>(originalUpdateDate, updateTestThng.updatedAt);
            Assert.IsTrue(updateTestThng.properties.Count == 1);
            Assert.AreEqual<string>("white", updateTestThng.properties[0].value);

            _sut.DeleteThng(updateTestThng.Id);
        }

        /// <summary>
        /// The API does not allow updates to Properties at the /thngs endpoint
        /// </summary>
        [TestMethod]
        public void UpdateChangeAPropertyFails()
        {
            var updateTestThng = new Thng { name = "property update" };
            updateTestThng.properties.Add(new Property { key = "color", value = "white", timestamp = DateTime.Now });
            _sut.CreateThng(updateTestThng);
            var originalUpdateDate = updateTestThng.updatedAt;

            updateTestThng.properties[0].value = "black";
            _sut.UpdateThng(updateTestThng);

            Assert.AreNotEqual<DateTime?>(originalUpdateDate, updateTestThng.updatedAt);
            Assert.IsTrue(updateTestThng.properties.Count == 1);
            // The following assertions proves that the property was NOT changed and remains the original value
            Assert.AreNotEqual<string>("black", updateTestThng.properties[0].value);
            Assert.AreEqual<string>("white", updateTestThng.properties[0].value);

            _sut.DeleteThng(updateTestThng.Id);
        }

        /// <summary>
        /// This case never makes it to the API, it throws when trying to create the JObject - probably a good thing
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateAddAPropertyWithSameKeyThrows()
        {
            var updateTestThng = new Thng { name = "tag update" };
            updateTestThng.properties.Add(new Property { key = "color", value = "white", timestamp = DateTime.Now });
            _sut.CreateThng(updateTestThng);
            var originalUpdateDate = updateTestThng.updatedAt;

            updateTestThng.properties.Add(new Property { key = "color", value = "white", timestamp = DateTime.Now });
            _sut.UpdateThng(updateTestThng);

            _sut.DeleteThng(updateTestThng.Id);
        }

        /// <summary>
        /// The API does not allow deletions of Properties at the /thngs endpoint
        /// </summary>
        [TestMethod]
        public void UpdateRemoveAPropertyFails()
        {
            var updateTestThng = new Thng { name = "tag update" };
            updateTestThng.properties.Add(new Property { key = "color", value = "white", timestamp = DateTime.Now });
            _sut.CreateThng(updateTestThng);
            var originalUpdateDate = updateTestThng.updatedAt;

            updateTestThng.properties.RemoveAt(0);
            _sut.UpdateThng(updateTestThng);

            Assert.AreNotEqual<DateTime?>(originalUpdateDate, updateTestThng.updatedAt);
            // The following assertions prove that the original property was NOT removed upon update
            Assert.IsTrue(updateTestThng.properties.Count == 1);
            Assert.AreEqual<string>("white", updateTestThng.properties[0].value);

            _sut.DeleteThng(updateTestThng.Id);

        }

        #endregion Properties

        #region Location

        /// <summary>
        /// The API DOES allow a Location to be added at the /thngs endpoint if none already exist
        /// </summary>
        [TestMethod]
        public void UpdateALocationWhenNoneExistsSucceeds()
        {
            var updateTestThng = new Thng { name = "tag update" };
            _sut.CreateThng(updateTestThng);
            var originalUpdateDate = updateTestThng.updatedAt;

            updateTestThng.location = new Location { longitude = 30, latitude = -30 };
            _sut.UpdateThng(updateTestThng);

            Assert.AreNotEqual<DateTime?>(originalUpdateDate, updateTestThng.updatedAt);
            Assert.IsNotNull(updateTestThng.location);
            Assert.AreEqual(30, updateTestThng.location.longitude);
            Assert.AreEqual(-30, updateTestThng.location.latitude);

            _sut.DeleteThng(updateTestThng.Id);
        }

        /// <summary>
        /// The API DOES NOT allow a Location to be updated at the /thngs endpoint if one already exists
        /// </summary>
        [TestMethod]
        public void UpdateALocationWhenOneExistsFails()
        {
            var updateTestThng = new Thng { name = "tag update" };
            updateTestThng.location = new Location { longitude = 30, latitude = -30 };
            _sut.CreateThng(updateTestThng);
            var originalUpdateDate = updateTestThng.updatedAt;

            updateTestThng.location.longitude = 20;
            updateTestThng.location.latitude = -20;
            _sut.UpdateThng(updateTestThng);

            Assert.AreNotEqual<DateTime?>(originalUpdateDate, updateTestThng.updatedAt);
            Assert.IsNotNull(updateTestThng.location);
            // The following assertions prove that the Location was not updated.
            Assert.AreEqual(30, updateTestThng.location.longitude);
            Assert.AreEqual(-30, updateTestThng.location.latitude);

            _sut.DeleteThng(updateTestThng.Id);
        }

        /// <summary>
        /// The API DOES NOT allow a Location to be deleted at the /thngs endpoint if one already exists
        /// </summary>
        [TestMethod]
        public void UpdateSetLocationToNullDoesNotDeleteLocation()
        {
            var updateTestThng = new Thng { name = "tag update" };
            updateTestThng.location = new Location { longitude = 30, latitude = -30 };
            _sut.CreateThng(updateTestThng);
            var originalUpdateDate = updateTestThng.updatedAt;

            updateTestThng.location = null;            
            _sut.UpdateThng(updateTestThng);

            Assert.AreNotEqual<DateTime?>(originalUpdateDate, updateTestThng.updatedAt);
            Assert.IsNotNull(updateTestThng.location);
            // The following assertions prove that the Location was not deleted.
            Assert.AreEqual(30, updateTestThng.location.longitude);
            Assert.AreEqual(-30, updateTestThng.location.latitude);

            _sut.DeleteThng(updateTestThng.Id);
        }


        #endregion Location


        #endregion Update Thng Tests

        #region Properties Tests

        [TestMethod]
        public void GetPropertiesWhenPropertiesExistSucceeds()
        {
            // Arrange
            var getPropsThng = new Thng { name = "Get Properties Test" };
            getPropsThng.properties.Add(new Property { key = "color", value = "white", timestamp = DateTime.Now });
            getPropsThng.properties.Add(new Property { key = "speed", value = "fast", timestamp = DateTime.Now });
            _sut.CreateThng(getPropsThng);
            var originalUpdateDate = getPropsThng.updatedAt;
            
            // Act
            Assert.IsFalse(string.IsNullOrEmpty(getPropsThng.Id));            
            var myProperties = _sut.GetProperties(getPropsThng.Id);
            
            // Assert
            Assert.IsTrue(getPropsThng.properties.Count == 2);
            Assert.IsTrue(myProperties.Count == 2);
            Assert.AreEqual<string>(getPropsThng.properties[0].value, myProperties[0].value);
            Assert.AreEqual<string>(getPropsThng.properties[1].value, myProperties[1].value);
            Assert.IsNotNull(myProperties[0].timestamp);
            Assert.IsNotNull(myProperties[0].timestamp);            

            _sut.DeleteThng(getPropsThng.Id);
        }

        [TestMethod]
        public void GetProperties_NoProperties_ReturnsEmptyList()
        {
            // Arrange
            var getPropsThng = new Thng { name = "Get Properties Test" };            
            _sut.CreateThng(getPropsThng);
            var originalUpdateDate = getPropsThng.updatedAt;

            // Act
            Assert.IsFalse(string.IsNullOrEmpty(getPropsThng.Id));
            var myProperties = _sut.GetProperties(getPropsThng.Id);

            // Assert
            Assert.IsNotNull(getPropsThng.properties);
            Assert.IsTrue(getPropsThng.properties.Count == 0);
            Assert.IsNotNull(myProperties);
            Assert.IsTrue(myProperties.Count == 0);            

            _sut.DeleteThng(getPropsThng.Id);
        }

        [TestMethod]
        public void CreateUpdateNewPropertiesSucceeds()
        {
            // Arrange
            var createPropsThng = new Thng { name = "Create Properties Test" };
            createPropsThng.properties.Add(new Property { key = "color", value = "white", timestamp = DateTime.Now });

            _sut.CreateThng(createPropsThng);
            var originalUpdateDate = createPropsThng.updatedAt;

            var propsToCreate = new List<Property>();
            propsToCreate.Add(new Property { key = "weight", value = "165", timestamp = DateTime.Now });
            propsToCreate.Add(new Property { key = "height", value = "68", timestamp = DateTime.Now });
            propsToCreate.Add(new Property { key = "length", value = "46", timestamp = DateTime.Now });

            // Act
            var result = _sut.CreateUpdateProperties(createPropsThng.Id, propsToCreate);

            // Assert
            Assert.IsNotNull(result);
            // The returned Properties are only those that were sent in the Create/Update request,
            // not the total number.
            Assert.IsTrue(result.Count == 3);

            // Total Properties on the Thng should now be 4
            var thngWithMoreProps = _sut.GetThng(createPropsThng.Id);            
            Assert.IsTrue(thngWithMoreProps.properties.Count == 4);

            // Also GetProperties should return 4
            var propsAgain = _sut.GetProperties(createPropsThng.Id);
            Assert.IsTrue(propsAgain.Count == 4);

            _sut.DeleteThng(createPropsThng.Id);
        }

        /// <summary>
        /// This test proves that updating a property using the /properties endpoint 
        /// does not add a new property - just updates it, nor does it add history to a property.
        /// </summary>
        [TestMethod]
        public void UpdateExistingPropertyValueAtPropertiesEndpoint()
        {
            // Arrange
            var testThng = new Thng { name = "Get Properties Test" };
            testThng.properties.Add(new Property { key = "color", value = "white", timestamp = DateTime.Now });
            testThng.properties.Add(new Property { key = "speed", value = "fast", timestamp = DateTime.Now });
            _sut.CreateThng(testThng);
            var originalUpdateDate = testThng.updatedAt;

            // Act
            Assert.IsFalse(string.IsNullOrEmpty(testThng.Id));
            var myProperties = _sut.GetProperties(testThng.Id);

            // Assert            
            Assert.IsTrue(myProperties.Count == 2);

            var colorProperty = myProperties.Find(p => p.key == "color");
            colorProperty.value = "blue";

            // Update the "color" property and check
            var result = _sut.CreateUpdateProperties(testThng.Id, myProperties);
            Assert.IsTrue(result.Count == 2);
            var newColor = result.Find(p => p.key == "color");
            Assert.AreEqual<string>("blue", newColor.value);

            // Get all properties - what's there?
            var propsAgain = _sut.GetProperties(testThng.Id);
            Assert.IsTrue(propsAgain.Count == 2);

            // Get the PropertyHistory for 'color'
            var colorPropertyHistory = _sut.GetPropertyHistory(testThng.Id, "color");
            Assert.IsTrue(colorPropertyHistory.Count == 1);

            _sut.DeleteThng(testThng.Id);
                        
        }

        [TestMethod]
        public void GetPropertyHistorySucceeds()
        {
            // Arrange
            var testThng = new Thng { name = "Get Property Test" };
            testThng.properties.Add(new Property { key = "color", value = "white", timestamp = DateTime.Now });            
            _sut.CreateThng(testThng);
            var originalUpdateDate = testThng.updatedAt;

            // Act
            Assert.IsFalse(string.IsNullOrEmpty(testThng.Id));
            var propertyHistory = _sut.GetPropertyHistory(testThng.Id, "color");

            // Assert
            Assert.IsNotNull(propertyHistory);
            Assert.IsTrue(propertyHistory.Count == 1);
            Assert.AreEqual("white", propertyHistory[0].value);

            _sut.DeleteThng(testThng.Id);

        }

        /// <summary>
        /// Updating a single property adds history with new value and timestamp.
        /// </summary>
        [TestMethod]
        public void UpdatePropertyAtPropertyEndpointAddsHistory()
        {
            // Arrange
            var testThng = new Thng { name = "Update Property Test" };
            testThng.properties.Add(new Property { key = "color", value = "white", timestamp = DateTime.Now });
            _sut.CreateThng(testThng);
            var originalUpdateDate = testThng.updatedAt;

            Assert.IsFalse(string.IsNullOrEmpty(testThng.Id));
            var propertyHistory = _sut.GetPropertyHistory(testThng.Id, "color");
            Assert.IsNotNull(propertyHistory);
            Assert.IsTrue(propertyHistory.Count == 1);
            Assert.AreEqual("white", propertyHistory[0].value);

            // Act
            var propertyToUpdate = propertyHistory[0];
            propertyToUpdate.value = "violet";
            propertyToUpdate.timestamp = DateTime.Now.AddDays(-1);
            _sut.UpdateProperty(testThng.Id, propertyToUpdate);
            // Get History a second time
            propertyHistory = _sut.GetPropertyHistory(testThng.Id, "color");

            // Assert
            Assert.IsTrue(propertyHistory.Count == 2);

            _sut.DeleteThng(testThng.Id);

        }

        /// <summary>
        /// Updating a single property with multiple values
        /// </summary>
        [TestMethod]
        public void UpdatePropertyWithMultipleValuesAndTimestamps()
        {
            // Arrange
            var testThng = new Thng { name = "Update Property Test" };
            var whiteTime = DateTime.Now.AddMinutes(1);
            testThng.properties.Add(new Property { key = "color", value = "white", timestamp = whiteTime });
            _sut.CreateThng(testThng);
            var originalUpdateDate = testThng.updatedAt;

            Assert.IsFalse(string.IsNullOrEmpty(testThng.Id));
            var propertyHistory = _sut.GetPropertyHistory(testThng.Id, "color");
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
            _sut.UpdateProperty(testThng.Id, propertyHistory);

            Assert.IsTrue(propertyHistory.Count == 4);            
            
            // Get History a second time
            var propertyHistory2 = _sut.GetPropertyHistory(testThng.Id, "color");
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

            _sut.DeleteThng(testThng.Id);

        }

        /// <summary>
        /// Get a single property and specify a time interval
        /// </summary>
        [TestMethod]
        public void GetPropertyUsingTimeInterval()
        {
            // Arrange
            var testThng = new Thng { name = "Get Property over time Test" };
            var whiteTime = DateTime.Now;
            testThng.properties.Add(new Property { key = "color", value = "white", timestamp = whiteTime });
            _sut.CreateThng(testThng);
            var originalUpdateDate = testThng.updatedAt;

            Assert.IsFalse(string.IsNullOrEmpty(testThng.Id));
            var propertyHistory = _sut.GetPropertyHistory(testThng.Id, "color");
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
            _sut.UpdateProperty(testThng.Id, propertyHistory);

            Assert.IsTrue(propertyHistory.Count == 4);

            // Get History in time - the request should only return 2 colors
            var beginTime = new DateTime(2010, 1, 1);
            var endTime = new DateTime(2013, 1, 1);
            var propertyHistory2 = _sut.GetPropertyHistory(testThng.Id, "color", (DateTime?)beginTime, (DateTime?)endTime);
            
            var colorRed = propertyHistory2.Find(p => p.value == "red");
            var colorGreen = propertyHistory2.Find(p => p.value == "green");

            // Assert  - only Red and Green should be returned
            Assert.IsTrue(propertyHistory2.Count == 2);
            Assert.IsNotNull(colorRed);
            Assert.IsNotNull(colorGreen);

            _sut.DeleteThng(testThng.Id);

        }
        [TestMethod]
        public void DeleteSingleProperty()
        {
            // Arrange
            var testThng = new Thng { name = "Delete Property Test" };
            testThng.properties.Add(new Property { key = "color", value = "white", timestamp = DateTime.Now });
            testThng.properties.Add(new Property { key = "weight", value = "165", timestamp = DateTime.Now });
            testThng.properties.Add(new Property { key = "height", value = "68", timestamp = DateTime.Now });
            testThng.properties.Add(new Property { key = "length", value = "46", timestamp = DateTime.Now });

            _sut.CreateThng(testThng);
            var originalUpdateDate = testThng.updatedAt;
            var originalProperties = _sut.GetProperties(testThng.Id);
           
            // Act                        
            _sut.DeleteProperty(testThng.Id, "color");
            var propertiesAfterDelete = _sut.GetProperties(testThng.Id);

            // Assert
            Assert.AreEqual(4, originalProperties.Count);            
            Assert.AreEqual(3, propertiesAfterDelete.Count);
            
            _sut.DeleteThng(testThng.Id);
        }

        #endregion Properties Tests

        #region Location Tests

        [TestMethod]
        public void GetLocation()
        {
            var getLocationThng = new Thng { name = "location thng", description = "get locations test" };

            getLocationThng.location = new Location { latitude = 30.45, longitude = -50.99 };
            _sut.CreateThng(getLocationThng);

            Assert.IsFalse(string.IsNullOrEmpty(getLocationThng.Id));
            Assert.IsNotNull(getLocationThng.location);

            // Act
            var locations = _sut.GetLocations(getLocationThng.Id);

            // Assert
            Assert.AreEqual(1, locations.Count);
            Assert.AreEqual(30.45, locations[0].latitude);
            Assert.AreEqual(-50.99, locations[0].longitude);

            _sut.DeleteThng(getLocationThng.Id);

        }

        [TestMethod]
        public void GetLocation_NoLocations_ReturnsEmptyList()
        {
            var getLocationThng = new Thng { name = "location thng", description = "get locations test" };            
            _sut.CreateThng(getLocationThng);

            Assert.IsFalse(string.IsNullOrEmpty(getLocationThng.Id));
            Assert.IsNull(getLocationThng.location);

            // Act
            var locations = _sut.GetLocations(getLocationThng.Id);

            // Assert
            Assert.IsNotNull(locations);
            Assert.AreEqual(0, locations.Count);
           
            _sut.DeleteThng(getLocationThng.Id);

        }

        [TestMethod]
        public void CreateUpdateLocations()
        {
            // Arrange
            var createLocationThng = new Thng { name = "location thng", description = "create locations test" };
            createLocationThng.location = new Location { latitude = 30.45, longitude = -50.99 };
            _sut.CreateThng(createLocationThng);
            Assert.IsFalse(string.IsNullOrEmpty(createLocationThng.Id));
            Assert.IsNotNull(createLocationThng.location);

            // Act
            var locationList = new List<Location>();
            locationList.Add(new Location{latitude = 40.0, longitude = -40.0});
            locationList.Add(new Location { latitude = 30.0, longitude = -25.0, timestamp = DateTime.Now });
            var createdLocations = _sut.CreateUpdateLocations(createLocationThng.Id, locationList);
            var allLocations = _sut.GetLocations(createLocationThng.Id);

            // Assert
            Assert.AreEqual(2, createdLocations.Count);
            Assert.AreEqual(3, allLocations.Count);

            _sut.DeleteThng(createLocationThng.Id);
        }

        /// <summary>
        /// This test demonstrates that if you change only the timestamp on a Location, it results
        /// in a second location if you get Locations again.
        /// </summary>
        [TestMethod]
        public void UpdateExistingLocationWithNewTimestamp()
        {
            // Arrange
            var createLocationThng = new Thng { name = "location thng", description = "create locations test" };
            createLocationThng.location = new Location { latitude = 30.45, longitude = -50.99 };
            _sut.CreateThng(createLocationThng);
            Assert.IsFalse(string.IsNullOrEmpty(createLocationThng.Id));
            Assert.IsNotNull(createLocationThng.location);

            // Act
            var locationList = _sut.GetLocations(createLocationThng.Id);
            var locationToUpdate = locationList[0];
            locationToUpdate.timestamp = DateTime.Now;

            locationList = new List<Location> { locationToUpdate };
            var createdLocations = _sut.CreateUpdateLocations(createLocationThng.Id, locationList);
            var allLocations = _sut.GetLocations(createLocationThng.Id);

            // Assert
            Assert.AreEqual(1, createdLocations.Count);
            Assert.AreEqual(2, allLocations.Count);

            _sut.DeleteThng(createLocationThng.Id);

        }

        /// <summary>
        /// This test demonstrates that if you create/update a location with the same timestamp, a new Location
        /// is NOT added.  Uniqueness is combination of latitude, longitude and timestamp.
        /// </summary>
        [TestMethod]
        public void UpdateExistingLocationWithSameTimestampLocationNotAdded()
        {
            // Arrange
            var createLocationThng = new Thng { name = "location thng", description = "create locations test" };
            createLocationThng.location = new Location { latitude = 30.45, longitude = -50.99 };
            _sut.CreateThng(createLocationThng);
            Assert.IsFalse(string.IsNullOrEmpty(createLocationThng.Id));
            Assert.IsNotNull(createLocationThng.location);

            // Act
            var locationList = _sut.GetLocations(createLocationThng.Id);
            var locationToUpdate = locationList[0];
            
            locationList = new List<Location> { locationToUpdate };
            var createdLocations = _sut.CreateUpdateLocations(createLocationThng.Id, locationList);
            var allLocations = _sut.GetLocations(createLocationThng.Id);

            // Assert
            Assert.AreEqual(1, createdLocations.Count);
            Assert.AreEqual(1, allLocations.Count);

            _sut.DeleteThng(createLocationThng.Id);

        }

        [TestMethod]
        public void DeleteAllLocations()
        {
            // Arrange
            var deleteLocationThng = new Thng { name = "delete thng", description = "delete locations test" };
            deleteLocationThng.location = new Location { latitude = 30.45, longitude = -50.99 };
            _sut.CreateThng(deleteLocationThng);
            Assert.IsFalse(string.IsNullOrEmpty(deleteLocationThng.Id));
            Assert.IsNotNull(deleteLocationThng.location);

            var locationList = new List<Location>();
            locationList.Add(new Location { latitude = 40.0, longitude = -40.0 });
            locationList.Add(new Location { latitude = 30.0, longitude = -25.0, timestamp = DateTime.Now });
            var createdLocations = _sut.CreateUpdateLocations(deleteLocationThng.Id, locationList);
            
            // Act
            _sut.DeleteLocations(deleteLocationThng.Id);
            var allLocations = _sut.GetLocations(deleteLocationThng.Id);

            // Assert
            Assert.AreEqual(0, allLocations.Count);

            _sut.DeleteThng(deleteLocationThng.Id);
        }

        [TestMethod]
        public void DeleteLocationsBeforeSpecifiedTime()
        {
            // Arrange
            var deleteLocationThng = new Thng { name = "delete thng", description = "delete locations test" };
            deleteLocationThng.location = new Location { latitude = 30.45, longitude = -50.99 };
            _sut.CreateThng(deleteLocationThng);
            Assert.IsFalse(string.IsNullOrEmpty(deleteLocationThng.Id));
            Assert.IsNotNull(deleteLocationThng.location);

            var locationList = new List<Location>();
            locationList.Add(new Location { latitude = 40.0, longitude = -40.0, timestamp = new DateTime(2011,1,1) });
            locationList.Add(new Location { latitude = 30.0, longitude = -25.0, timestamp = new DateTime(2009,1,1) });
            var createdLocations = _sut.CreateUpdateLocations(deleteLocationThng.Id, locationList);

            // Act
            _sut.DeleteLocations(deleteLocationThng.Id, new DateTime(2010,1,1));
            var allLocations = _sut.GetLocations(deleteLocationThng.Id);

            // Assert
            Assert.AreEqual(2, allLocations.Count);

            _sut.DeleteThng(deleteLocationThng.Id);
        }



        #endregion Location Tests

    }
}
