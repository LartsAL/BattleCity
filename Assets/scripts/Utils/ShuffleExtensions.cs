using System.Collections.Generic;

namespace Utils
{
    public static class ShuffleExtensions // Fisher-Yates shuffle
    {
        private static readonly System.Random Random = new();
           
        public static void Shuffle<T>(this T[] array)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                int r = Random.Next(i + 1);
                (array[i], array[r]) = (array[r], array[i]);
            }
        }
        
        public static void Shuffle<T>(this IList<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int r = Random.Next(i + 1);
                (list[i], list[r]) = (list[r], list[i]);
            }
        }
    }
}