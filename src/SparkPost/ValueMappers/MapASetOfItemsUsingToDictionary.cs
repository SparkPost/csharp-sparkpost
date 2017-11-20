using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using SparkPost.Utilities;

namespace SparkPost.ValueMappers
{
    public class MapASetOfItemsUsingToDictionary : IValueMapper
    {
        private readonly IDictionary<Type, MethodInfo> converters;
        private readonly IDataMapper dataMapper;

        public MapASetOfItemsUsingToDictionary(IDataMapper dataMapper)
        {
            this.dataMapper = dataMapper;
            converters = dataMapper.ToDictionaryMethods();
        }

        public bool CanMap(Type propertyType, object value)
        {
            return value != null && propertyType.Name.EndsWith("List`1") &&
                   propertyType.GetTypeInfo().GetGenericArguments().Count() == 1 &&
                   converters.ContainsKey(propertyType.GetTypeInfo().GetGenericArguments().First());
        }

        public object Map(Type propertyType, object value)
        {
            var converter = converters[propertyType.GetTypeInfo().GetGenericArguments().First()];

            var list = (value as IEnumerable<object>).ToList();

            if (list.Any())
                value = list.Select(x => converter.Invoke(dataMapper, new[] {x})).ToList();
            else
                value = null;

            return value;
        }
    }
}