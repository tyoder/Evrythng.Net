using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvrythngAPI
{
    public class ProductService : IProductService
    {
        private IProductRepository _productRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        public ProductService()
        {
            _productRepository = new ProductRepository();
        }


        /// <summary>
        /// Creates a Product
        /// </summary>
        /// <param name="product">Product to Create</param>
        public void CreateProduct(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException("product", "Product cannot be null.");
            }

            if (string.IsNullOrEmpty(product.fn))
            {
                throw new ArgumentException("Product.fn", "Product must have a fn attribute to be created.");
            }

            // Do not allow Generic List attributes to be null when calling the Repository
            if (product.categories == null)
            {
                throw new ArgumentException("Product.categories", "Product.categories must not be null - may be an empty List.");
            }            
            if (product.photos == null)
            {
                throw new ArgumentException("Product.photos", "Product.photos must not be null - may be an empty List.");
            }
            if (product.identifiers == null)
            {
                throw new ArgumentException("Product.identifiers", "Product.identifiers must not be null - may be an empty List.");
            }
            if (product.tags == null)
            {
                throw new ArgumentException("Product.tags", "Product.tags must not be null - may be an empty List.");
            }
            if (product.properties == null)
            {
                throw new ArgumentException("Product.properties", "Product.properties must not be null - may be an empty List.");
            }

            // Create the Product
            _productRepository.CreateProduct(product);

            // Retrieve Product Properties to get accurate createdDate, etc.
            var createdProductProperties = _productRepository.GetProperties(product.Id);           
            product.properties = createdProductProperties;
                        
        }

        /// <summary>
        /// Gets a specific Product by Id
        /// </summary>
        /// <param name="productId">Id of the Product</param>
        /// <returns>A Product object</returns>
        public Product GetProduct(string productId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets all Products for API key
        /// </summary>
        /// <returns>Generic List of Product objects</returns>
        public List<Product> GetProducts()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates a Product
        /// </summary>
        /// <param name="product">Product object to update</param>
        public void UpdateProduct(Product product)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes a Product
        /// </summary>
        /// <param name="productId">Id of Product to delete</param>
        public void DeleteProduct(string productId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the properties of a Product
        /// </summary>
        /// <param name="productId">The Id of the Product</param>
        /// <returns>Collection of Properties of a Product</returns>
        public List<Property> GetProperties(string productId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a specific property of a Product and returns a history of values with timestamps.
        /// </summary>
        /// <param name="productId">The Id of the Product</param>
        /// <param name="propertyKey">The property key of the desired property</param>
        /// <returns>A specific Product Property</returns>
        public List<Property> GetPropertyHistory(string productId, string propertyKey)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a specific property of a Product within a specified time interval
        /// </summary>
        /// <param name="productId">The Id of the Product</param>
        /// <param name="propertyKey">The property key of the desired property</param>
        /// <param name="beginDateTime">The "Begin" DateTime</param>
        /// <param name="endDateTime">The "End" DateTime</param>
        /// <returns></returns>
        public List<Property> GetPropertyHistory(string productId, string propertyKey, DateTime? beginDateTime, DateTime? endDateTime)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates or updates properties of a Product
        /// </summary>
        /// <param name="productId">the Id of the Product</param>
        /// <param name="properties">One or more properties to create or update.</param>
        /// <returns>The property or properties created or updated.</returns>
        public List<Property> CreateUpdateProperties(string productId, List<Property> properties)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates a specific property of a Product
        /// </summary>
        /// <param name="productId">The Id of the Product</param>
        /// <param name="property">The Property object with updates</param>
        /// <returns>void - Property passed by reference will be updated</returns>
        public void UpdateProperty(string productId, Property property)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates a specific property of a Product with multiple values
        /// </summary>
        /// <param name="productId">The Id of the Product</param>
        /// <param name="properties">List of properties to update</param>
        /// <returns>void - Properties passed by reference will be updated</returns>
        public void UpdateProperty(string productId, List<Property> properties)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes a specific Property of a Product, including all values of that Property over time.
        /// </summary>
        /// <param name="productId">The Id of the Product</param>
        /// <param name="propertyKey">The property key of the property to delete</param>
        public void DeleteProperty(string productId, string propertyKey)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes all values of a Property prior to the specified timestamp.
        /// </summary>
        /// <param name="productId">The Id of the Product</param>
        /// <param name="propertyKey">The property key</param>
        /// <param name="endDateTime">Timestamp before which all property values will be removed</param>
        public void DeleteProperty(string productId, string propertyKey, DateTime? endDateTime)
        {
            throw new NotImplementedException();
        }
    }
}
