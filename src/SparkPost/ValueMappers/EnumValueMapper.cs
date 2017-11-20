using System;
using System.Reflection;

namespace SparkPost.ValueMappers
{
    public class EnumValueMapper : IValueMapper
    {
        public bool CanMap(Type propertyType, object value)
        {
            return propertyType.GetTypeInfo().IsEnum;
        }

        public object Map(Type propertyType, object value)
        {
            return value.ToString().ToLowerInvariant();
        }
    }
}