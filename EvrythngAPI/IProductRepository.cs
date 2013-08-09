using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvrythngAPI
{
    public interface IProductRepository
    {
        void CreateProduct(Product product);

        Product GetProduct(string productId);

        List<Product> GetProducts();

        void UpdateProduct(Product product);

        void DeleteProduct(string productId);

        /// <summary>
        /// Gets the properties of a Product
        /// </summary>
        /// <param name="productId">The Id of the Product</param>
        /// <returns>Collection of Properties of a Product</returns>
        List<Property> GetProperties(string productId);

        /// <summary>
        /// Gets a specific property of a Product and returns a history of values with timestamps.
        /// </summary>
        /// <param name="productId">The Id of the Product</param>
        /// <param name="propertyKey">The property key of the desired property</param>
        /// <returns>A specific Product Property</returns>
        List<Property> GetPropertyHistory(string productId, string propertyKey);

        /// <summary>
        /// Gets a specific property of a Product within a specified time interval
        /// </summary>
        /// <param name="productId">The Id of the Product</param>
        /// <param name="propertyKey">The property key of the desired property</param>
        /// <param name="beginDateTime">The "Begin" DateTime</param>
        /// <param name="endDateTime">The "End" DateTime</param>
        /// <returns></returns>
        List<Property> GetPropertyHistory(string productId, string propertyKey, DateTime? beginDateTime, DateTime? endDateTime);

        /// <summary>
        /// Creates or updates properties of a Product
        /// </summary>
        /// <param name="productId">the Id of the Product</param>
        /// <param name="properties">One or more properties to create or update.</param>
        /// <returns>The property or properties created or updated.</returns>
        List<Property> CreateUpdateProperties(string productId, List<Property> properties);

        /// <summary>
        /// Updates a specific property of a Product
        /// </summary>
        /// <param name="productId">The Id of the Product</param>
        /// <param name="property">The Property object with updates</param>
        /// <returns>void - Property passed by reference will be updated</returns>
        void UpdateProperty(string productId, Property property);

        /// <summary>
        /// Updates a specific property of a Product with multiple values
        /// </summary>
        /// <param name="productId">The Id of the Product</param>
        /// <param name="properties">List of properties to update</param>
        /// <returns>void - Properties passed by reference will be updated</returns>
        void UpdateProperty(string productId, List<Property> properties);

        /// <summary>
        /// Deletes a specific Property of a Product, including all values of that Property over time.
        /// </summary>
        /// <param name="productId">The Id of the Product</param>
        /// <param name="propertyKey">The property key of the property to delete</param>
        void DeleteProperty(string productId, string propertyKey);

        /// <summary>
        /// Deletes all values of a Property prior to the specified timestamp.
        /// </summary>
        /// <param name="productId">The Id of the Product</param>
        /// <param name="propertyKey">The property key</param>
        /// <param name="endDateTime">Timestamp before which all property values will be removed</param>
        void DeleteProperty(string productId, string propertyKey, DateTime? endDateTime);
    }
}
