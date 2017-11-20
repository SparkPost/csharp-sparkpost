using System;
using System.Collections.Generic;
using System.Reflection;

namespace SparkPost.ValueMappers
{
    public class AnonymousValueMapper : IValueMapper
    {
        private readonly IDataMapper dataMapper;

        public AnonymousValueMapper(IDataMapper dataMapper)
        {
            this.dataMapper = dataMapper;
        }

        public bool CanMap(Type propertyType, object value)
        {
            return ThisIsAnAnonymousType(value);
        }

        public object Map(Type propertyType, object value)
        {
            var newValue = new Dictionary<string, object>();
            foreach (var property in value.GetType().GetTypeInfo().GetProperties())
                newValue[property.Name] = property.GetValue(value);
            return dataMapper.GetTheValue(newValue.GetType(), newValue);
        }

        private static bool ThisIsAnAnonymousType(object value)
        {
            return value != null &&
                   (value.GetType().Name.Contains("AnonymousType") || value.GetType().Name.Contains("AnonType"));
        }
    }
}