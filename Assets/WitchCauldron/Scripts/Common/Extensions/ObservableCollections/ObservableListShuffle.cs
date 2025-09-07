using System;
using System.Linq;
using ObservableCollections;

namespace WitchCauldron.Scripts.Common.Extensions.ObservableCollections
{
    public static class ObservableListShuffle
    {
        private static readonly Random Random = new Random();

        public static void Shuffle<T>(this ObservableList<T> collection)
        {
            var list = collection.ToList();

            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = Random.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
            

            collection.Clear();
            foreach (var item in list)
            {
                collection.Add(item);
            }
        }
    }
}