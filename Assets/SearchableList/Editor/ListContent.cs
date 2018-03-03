using System;
using System.Reflection;

namespace FromList
{
    public class ListContent
    {
        private bool _isCachable;
        private string[] _content; 

        /// <summary>
        /// Returns back if our list is cacheable accross mulitple instances. 
        /// </summary>
        public bool isCachable
        {
            get { return _isCachable; }
        }

        /// <summary>
        /// Retuns back all the content in this list 
        /// </summary>
        public string[] content
        {
            get { return _content; }
        }

        /// <summary>
        /// Fills this list will all the names of an enum 
        /// </summary>
        public ListContent(Type enumType)
        {
            _isCachable = true;
            _content = Enum.GetNames(enumType); 
        }

        public ListContent(MethodInfo methodContent)
        {
            _isCachable = methodContent.IsStatic; 
        }

        public ListContent(PropertyInfo propertyContent)
        {
            if(propertyContent.CanRead)
            {
                _isCachable = propertyContent.GetAccessors()[0].IsStatic;
            }
        }

        public ListContent(FieldInfo fieldContent)
        {
            _isCachable = fieldContent.IsStatic;
        }

        public ListContent(string filePath)
        {
            _isCachable = true;
        }
    }
}
