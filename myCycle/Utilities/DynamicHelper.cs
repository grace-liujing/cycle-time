using System.Collections.Generic;

namespace myCycle.Utilities
{
    public static class DynamicHelper
    {
        public static List<dynamic> ToDList(this object source)
        {
            dynamic sourceList = source;
            var result = new List<dynamic>();
            foreach (var elem in sourceList)
            {
                result.Add(elem);
            }
            return result;
        }
    }
}