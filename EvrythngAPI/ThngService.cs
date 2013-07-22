using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvrythngAPI
{
    public class ThngService : IThngService
    {
        private IThngRepository _thngRepository;
                
        public ThngService()
        {
            _thngRepository = new ThngRepository();

        }
        
        public void CreateThng(Thng thng)
        {
            if (thng == null)
            {
                throw new ArgumentNullException("thng", "Thng cannot be null.");
            }
            if (string.IsNullOrEmpty(thng.name))
            {
                throw new ArgumentException("Thng.name", "Thng must have a name.");
            }

            _thngRepository.CreateThng(thng);
            
        }

        public List<Thng> GetThngs()
        {
            return _thngRepository.GetThngs();
        }

        public Thng GetThng(string thngId)
        {
            if (string.IsNullOrEmpty(thngId))
            {
                throw new ArgumentException("thngId", "Thng Id must have a value.");
            }

            return _thngRepository.GetThng(thngId);
        }

        public void UpdateThng(Thng thng)
        {
            if (thng == null)
            {
                throw new ArgumentNullException("thng", "Thng cannot be null.");
            }
            if (thng.properties.Count > 0)
            {
                _thngRepository.CreateUpdateProperties(thng.Id, thng.properties);
            }            

            _thngRepository.UpdateThng(thng);
        }

        public void DeleteThng(string thngId)
        {
            if (string.IsNullOrEmpty(thngId))
            {
                throw new ArgumentException("thngId", "Thng Id must have a value.");
            }
            
            _thngRepository.DeleteThng(thngId);
        }

        public List<Property> GetProperties(string thngId)
        {
            if (string.IsNullOrEmpty(thngId))
            {
                throw new ArgumentException("thngId", "Thng Id must have a value."); 
            }

            return _thngRepository.GetProperties(thngId);
        }

        public List<Property> GetPropertyHistory(string thngId, string propertyKey)
        {
            if (string.IsNullOrEmpty(thngId))
            {
                throw new ArgumentException("thngId", "Thng Id must have a value.");
            }
            if (string.IsNullOrEmpty(propertyKey))
            {
                throw new ArgumentException("propertyKey", "The Property key must have a value.");
            }

            return _thngRepository.GetPropertyHistory(thngId, propertyKey);
        }

        public List<Property> GetPropertyHistory(string thngId, string propertyKey, DateTime? beginDateTime, DateTime? endDateTime)
        {
            if (string.IsNullOrEmpty(thngId))
            {
                throw new ArgumentException("thngId", "Thng Id must have a value.");
            }
            if (string.IsNullOrEmpty(propertyKey))
            {
                throw new ArgumentException("propertyKey", "The Property key must have a value.");
            } 
            if (beginDateTime == null || beginDateTime.HasValue == false)
            {
                throw new ArgumentException("beginDateTime", "The begin time must have a value.");
            }
            if (endDateTime == null || endDateTime.HasValue == false)
            {
                throw new ArgumentException("endDateTime", "The end time must have a value.");
            }
            if (endDateTime < beginDateTime)
            {
                throw new ArgumentException("time interval", "The end time must be greater than the begin time.");
            }

            return _thngRepository.GetPropertyHistory(thngId, propertyKey, beginDateTime, endDateTime);
        }

        public List<Property> CreateUpdateProperties(string thngId, List<Property> properties)
        {
            if (string.IsNullOrEmpty(thngId))
            {
                throw new ArgumentException("thngId", "Thng Id must have a value.");
            }
            if (properties == null || properties.Count == 0)
            {
                throw new ArgumentException("properties", "The list of properties is null or empty.");
            }

            return _thngRepository.CreateUpdateProperties(thngId, properties);
        }

        public void UpdateProperty(string thngId, Property property)
        {
            if (string.IsNullOrEmpty(thngId))
            {
                throw new ArgumentException("thngId", "Thng Id must have a value.");
            }
            if (property == null)
            {
                throw new ArgumentNullException("property", "The property must have a value.");
            }

            _thngRepository.UpdateProperty(thngId, property);
        }

        public void UpdateProperty(string thngId, List<Property> properties)
        {
            if (string.IsNullOrEmpty(thngId))
            {
                throw new ArgumentException("thngId", "Thng Id must have a value.");
            }
            if (properties == null || properties.Count == 0)
            {
                throw new ArgumentException("properties", "The list of properties is null or empty.");
            }

            _thngRepository.UpdateProperty(thngId, properties);
        }

        public void DeleteProperty(string thngId, string propertyKey)
        {
            if (string.IsNullOrEmpty(thngId))
            {
                throw new ArgumentException("thngId", "Thng Id must have a value.");
            }
            if (string.IsNullOrEmpty(propertyKey))
            {
                throw new ArgumentException("propertyKey", "The Property key must have a value.");
            }

            _thngRepository.DeleteProperty(thngId, propertyKey);
        }

        public void DeleteProperty(string thngId, string propertyKey, DateTime? endDateTime)
        {
            if (string.IsNullOrEmpty(thngId))
            {
                throw new ArgumentException("thngId", "Thng Id must have a value.");
            }
            if (string.IsNullOrEmpty(propertyKey))
            {
                throw new ArgumentException("propertyKey", "The Property key must have a value.");
            }
            if (endDateTime == null || endDateTime.HasValue == false)
            {
                throw new ArgumentException("endDateTime", "The end time must have a value.");
            }

            _thngRepository.DeleteProperty(thngId, propertyKey, endDateTime);

        }

        public List<Location> GetLocations(string thngId)
        {
            if (string.IsNullOrEmpty(thngId))
            {
                throw new ArgumentException("thngId", "Thng Id must have a value.");
            }

            return _thngRepository.GetLocations(thngId);
        }

        public List<Location> CreateUpdateLocations(string thngId, List<Location> locations)
        {
            if (string.IsNullOrEmpty(thngId))
            {
                throw new ArgumentException("thngId", "Thng Id must have a value.");
            }
            if (locations == null || locations.Count == 0)
            {
                throw new ArgumentException("locations", "The list of locations is null or empty.");
            }

            return _thngRepository.CreateUpdateLocations(thngId, locations);
        }

        public void DeleteLocations(string thngId)
        {
            if (string.IsNullOrEmpty(thngId))
            {
                throw new ArgumentException("thngId", "Thng Id must have a value.");
            }

            _thngRepository.DeleteLocations(thngId);            
        }

        public void DeleteLocations(string thngId, DateTime? endDateTime)
        {
            if (string.IsNullOrEmpty(thngId))
            {
                throw new ArgumentException("thngId", "Thng Id must have a value.");
            }
            if (endDateTime == null || endDateTime.HasValue == false)
            {
                throw new ArgumentException("endDateTime", "The end time must have a value.");
            }

            _thngRepository.DeleteLocations(thngId, endDateTime);

        }
    }
}
