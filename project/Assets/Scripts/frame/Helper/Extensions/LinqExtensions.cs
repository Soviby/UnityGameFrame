using System.Collections.Generic;

 
    public static class LinqExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                return true;
            }

            return !enumerable.GetEnumerator().MoveNext();
        }

        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }
    }
