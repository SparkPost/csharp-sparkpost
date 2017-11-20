using System.Linq;
using System.Reflection;

namespace SparkPost
{
    public static class LeftRight
    {
        public static void SetValuesToMatch(object left, object right)
        {
            var leftProperties = left.GetType().GetTypeInfo().GetProperties();
            var rightProperties = left.GetType().GetTypeInfo().GetProperties();
            foreach (var rightProperty in rightProperties)
            {
                try
                {
                    var leftProperty = leftProperties.FirstOrDefault(x => x.Name == rightProperty.Name);
                    leftProperty?.SetValue(left, rightProperty.GetValue(right));
                }
                catch
                {
                    // ignore
                }
            }
        }
    }
}