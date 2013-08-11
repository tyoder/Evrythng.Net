using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvrythngAPI
{
   
    public class Thng
    {

        public Thng()
        {
            // Initialize List properties so they are never null.
            this.tags = new List<string>();
            this.properties = new List<Property>();

        }

        #region Public Properties

        public string Id { get; set; }
        public string productId { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
        public List<string> tags { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public List<Property> properties { get; set; }
        public Location location { get; set; }

        #endregion Public Properties
    }

    public class Product
    {
        public Product()
        {
            // Initialize List properties so they are never null.
            this.categories = new List<string>();
            this.photos = new List<string>();
            this.identifiers = new List<Identifier>();
            this.tags = new List<string>();
            this.properties = new List<Property>();
        }
        
        #region Public Properties

        public string Id { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
        public string fn { get; set; }
        public string description { get; set; }
        public string brand { get; set; }
        public List<string> categories { get; set; }
        public List<string> photos { get; set; }
        public string url { get; set; }
        public List<Identifier> identifiers { get; set; }
        public List<Property> properties { get; set; }
        public List<string> tags { get; set; }
        
        #endregion Public Properties
    }

    public class Property
    {
        public string key { get; set; }
        public string value { get; set; }
        public DateTime? timestamp { get; set; }
    }

    public class Location
    {
        public DateTime? timestamp { get; set; }
        public Double latitude { get; set; }
        public Double longitude { get; set; }
    }

    public class Identifier
    {
        public string key { get; set; }
        public string value { get; set; }
    }
    


}
