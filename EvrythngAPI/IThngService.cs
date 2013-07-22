using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvrythngAPI
{
    /// <summary>
    /// This interface specifies the methods/behaviors that a Thng Service must implement. 
    /// The purpose of the service layer is to enforce rules such as non nulls, etc before 
    /// calling the ThngRepository and the actual API.
    /// </summary>
    public interface IThngService
    {
        /// <summary>
        /// Creates a new Thng
        /// </summary>
        /// <param name="thng">Thng object to create, reference is updated with Id and dates</param>
        void CreateThng(Thng thng);

        /// <summary>
        /// Gets all Thngs created under current account
        /// </summary>
        /// <returns>Collection of Thngs</returns>
        List<Thng> GetThngs();

        /// <summary>
        /// Gets a specific Thng by its Id
        /// </summary>
        /// <param name="thngId">The Id of the Thng</param>
        /// <returns>Thng returned from Evrythng</returns>
        Thng GetThng(string thngId);

        /// <summary>
        /// Updates a Thng
        /// </summary>
        /// <param name="thng">Thng object to update</param>
        /// <returns>Thng object returned from Evrythng</returns>
        void UpdateThng(Thng thng);

        /// <summary>
        /// Deletes a Thng
        /// </summary>
        /// <param name="thngId">The Id of the Thng to delete</param>
        void DeleteThng(string thngId);

        /// <summary>
        /// Gets the properties of a Thng
        /// </summary>
        /// <param name="thngId">The Id of the Thng</param>
        /// <returns>Collection of Properties of a Thng</returns>
        List<Property> GetProperties(string thngId);
       
        /// <summary>
        /// Gets a specific property of a Thng
        /// </summary>
        /// <param name="thngId">The Id of the Thng</param>
        /// <param name="propertyKey">The property key of the desired property</param>
        /// <returns>The history of a Thng Property in the form of a list reflecting values at different times.</returns>
        List<Property> GetPropertyHistory(string thngId, string propertyKey);

        /// <summary>
        /// Gets a specific property of a Thng within a specified time interval
        /// </summary>
        /// <param name="thngId">The Id of the Thng</param>
        /// <param name="propertyKey">The property key of the desired property</param>
        /// <param name="beginDateTime">The "Begin" DateTime</param>
        /// <param name="endDateTime">The "End" DateTime</param>
        /// <returns>The history of a Thng Property in the form of a list reflecting values between the given times.</returns>
        List<Property> GetPropertyHistory(string thngId, string propertyKey, DateTime? beginDateTime, DateTime? endDateTime);

        /// <summary>
        /// Creates or updates properties of a Thng
        /// </summary>
        /// <param name="thngId">the Id of the Thng</param>
        /// <param name="properties">One or more properties to create or update.</param>
        /// <returns>The property or properties created or updated.</returns>
        List<Property> CreateUpdateProperties(string thngId, List<Property> properties);


        /// <summary>
        /// Updates a specific property of a Thng
        /// </summary>
        /// <param name="thngId">The Id of the Thng</param>
        /// <param name="property">The Property object with updates</param>
        /// <returns>void - Property passed by reference will be updated</returns>
        void UpdateProperty(string thngId, Property property);

        /// <summary>
        /// Updates a specific property of a Thng with multiple values
        /// </summary>
        /// <param name="thngId">The Id of the Thng</param>
        /// <param name="properties">List of properties to update</param>
        /// <returns>void - Properties passed by reference will be updated</returns>
        void UpdateProperty(string thngId, List<Property> properties);

        /// <summary>
        /// Deletes a specific Property of a Thng
        /// </summary>
        /// <param name="thngId">The Id of the Thng</param>
        /// <param name="propertyKey">The property key of the property to delete</param>
        void DeleteProperty(string thngId, string propertyKey);

        /// <summary>
        /// Deletes all values of a Property prior to the specified timestamp.
        /// </summary>
        /// <param name="thngId">The Id of the Thng</param>
        /// <param name="propertyKey">The property key</param>
        /// <param name="endDateTime">Timestamp before which all property values will be removed</param>
        void DeleteProperty(string thngId, string propertyKey, DateTime? endDateTime);


        /// <summary>
        /// Gets collection of Locations of a Thng
        /// </summary>
        /// <param name="thngId">The Id of the Thng</param>
        /// <returns>A collection of Locations of a Thng</returns>
        List<Location> GetLocations(string thngId);

        /// <summary>
        /// Updates the Locations of a Thng
        /// </summary>
        /// <param name="thngId">The Id of the Thng</param>
        /// <param name="locations">The collection of Locations to update</param>
        /// <returns>The location(s) created or updated</returns>
        List<Location> CreateUpdateLocations(string thngId, List<Location> locations);

        /// <summary>
        /// Deletes all Locations for the specified Thng
        /// </summary>
        /// <param name="thngId">Id of Thng</param>
        void DeleteLocations(string thngId);

        /// <summary>
        /// Deletes Locations of a Thng prior to a point in time specified.
        /// </summary>
        /// <param name="thngId">The Id of the Thng</param>
        /// <param name="endDateTime">The time specified, such that all Locations with a timestamp previous will be deleted</param>
        void DeleteLocations(string thngId, DateTime? endDateTime);
    }
}
