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
        
        [TestMethod]
        public void CreateProductWithfnAndDescriptionSucceeds()
        {
            var productToCreate = new Product { fn = "test product", description = "product for unit test" };

            _sut.CreateProduct(productToCreate);

            Assert.IsFalse(string.IsNullOrEmpty(productToCreate.Id));
        }
    }
}
