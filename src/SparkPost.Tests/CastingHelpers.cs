namespace SparkPost.Tests
{
    public static class CastingHelpers
    {
        public static T CastAs<T>(this object @object) where T : class
        {
            return @object as T;
        }

        public static T CastTo<T>(this object @object)
        {
            return (T)@object;
        }
    }
}
