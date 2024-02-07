using PMM.Core.Provider.Enum;


namespace PMM.Core.Utils
{
    public struct RequireClass<T> where T : class { }
    public struct RequireStruct<T> where T : struct { }

    internal static class ExtensionMethod
    {
        public static void AddOptionalParameter<T>(this Dictionary<string, T> parameters, string key, T? value)
        {
            if (value != null)
            {
                parameters.Add(key, value);
            }
        }

        public static TSource? SingleOrNull<TSource>(this IEnumerable<TSource> source, RequireClass<TSource> _ = default)
    where TSource : class
        {
            return Enumerable.SingleOrDefault(source.Select(x => (TSource?)x));
        }

        public static TSource? SingleOrNull<TSource>(this IEnumerable<TSource> source, RequireStruct<TSource> _ = default)
            where TSource : struct
        {
            return Enumerable.SingleOrDefault(source.Select(x => (TSource?)x));
        }

        public static TSource? SingleOrNull<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, RequireClass<TSource> _ = default)
            where TSource : class
        {
            return Enumerable.SingleOrDefault(source.Where(predicate));
        }

        public static TSource? SingleOrNull<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, RequireStruct<TSource> _ = default)
            where TSource : struct
        {
            return Enumerable.SingleOrDefault(source.Where(predicate));
        }
    }
}
