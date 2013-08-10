using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EvrythngAPI
{
    public static class Utilities
    {
        public static long MillisecondsSinceEpoch(DateTime? dateTime)
        {
            if (dateTime.HasValue)
            {
                TimeSpan t = dateTime.Value.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                return (long)t.TotalMilliseconds;
            }
            else
            {
                return 0;
            }
            
        }

        public static DateTime? DateTimeSinceEpoch(long milliseconds)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(milliseconds).ToLocalTime();            
        }

        public static JArray ConvertPropertiesToJArray(List<Property> properties)
        {
            var propertiesArray = new JArray();

            foreach (Property p in properties)
            {
                dynamic jObject = new JObject();
                jObject.key = p.key;
                jObject.value = p.value;
                jObject.timestamp = MillisecondsSinceEpoch(p.timestamp);
                propertiesArray.Add(jObject);
            }

            return propertiesArray;
        }

        public static JObject ConvertPropertyToJObject(Property property)
        {
            // Create a dynamic JObject for more controlled serialization
            dynamic dynamicPropertyObject = new JObject();

            dynamicPropertyObject.key = property.key;
            dynamicPropertyObject.value = property.value;

            return dynamicPropertyObject;
        }

        public static List<Property> ConvertJArrayToProperties(JArray propertiesArray)
        {
            var properties = new List<Property>();

            foreach (dynamic p in propertiesArray)
            {
                var myProperty = new Property();
                myProperty.key = p.key;
                myProperty.value = p.value;
                myProperty.timestamp = Utilities.DateTimeSinceEpoch((long)p.timestamp);
                properties.Add(myProperty);
            }

            return properties;
        }

        public static List<Property> ConvertJArrayToProperties(JArray propertiesArray, string propertyKey)
        {
            var properties = new List<Property>();

            foreach (dynamic p in propertiesArray)
            {
                var myProperty = new Property();
                myProperty.key = propertyKey;
                myProperty.value = p.value;
                myProperty.timestamp = Utilities.DateTimeSinceEpoch((long)p.timestamp);
                properties.Add(myProperty);
            }

            return properties;
        }
    }
}
