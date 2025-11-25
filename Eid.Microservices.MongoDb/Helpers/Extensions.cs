using System;

namespace Eid.Microservices.MongoDb.Helpers
{
    public static class Extensions
    {
        /// <summary>
        /// Get the default value of a given type. Null for reference type and default(T) for value types.
        /// </summary>
        /// <param name="type">Type being evaluated.</param>
        /// <returns>Boxed value of null or default(T).</returns>
        public static object GetDefaultValue(this Type type)
        {
            if (type.IsValueType)
                return Activator.CreateInstance(type);

            return null;
        }
    }
}
