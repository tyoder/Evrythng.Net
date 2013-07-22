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
            // Initialize tags so List is never null.
            this.tags = new List<string>();
            this.properties = new List<Property>();

        }
        
        #region Public Properties

        public string Id { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
        public List<string> tags { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public List<Property> properties { get; set; }
        public Location location { get; set; }

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
}
