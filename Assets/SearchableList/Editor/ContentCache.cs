using System;
using System.Collections.Generic;

namespace FromList
{
    public static class ContentCache
    {
        private static Dictionary<Type, ListContent> _cachedEnums; 


        static ContentCache()
        {
            _cachedEnums = new Dictionary<Type, ListContent>();
        }

        /// <summary>
        /// Based of an enum type this tries to get it's cached list.
        public static ListContent GetListFromEnum(Type enumType)
        {
            if(!enumType.IsEnum)
            {
                return null;
            }

            if(_cachedEnums.ContainsKey(enumType))
            {
                return _cachedEnums[enumType];
            }
            ListContent newList = new ListContent(enumType);
            _cachedEnums[enumType] = newList;
            return newList;
        }
    }
}
