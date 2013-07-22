using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
