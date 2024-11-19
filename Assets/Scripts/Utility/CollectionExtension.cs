using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Utility
{
    public static class CollectionExtension
    {
        public static void Debug(this ICollection collection, bool showCount = true)
        {
            var debug = string.Empty;
            debug += "Name : " + collection.ToString().Split('[')[^1].Replace(']',' ') + "\n";
            debug += "__________________________\n";
            
            var enumerable = collection.Cast<object>().ToList();
            debug = enumerable.Aggregate(debug, (current, item) => current + (item + "\n"));

            debug += showCount ? "Total Count : " + collection.Count + "\n" : string.Empty;
            debug += "__________________________";
            UnityEngine.Debug.Log(debug);
        }
        
        public static void Debug(this IEnumerable collection, bool showCount = true)
        {
            var debug = string.Empty;
            debug += "Name : " + collection.ToString().Split('[')[^1].Replace(']',' ') + "\n";
            debug += "__________________________\n";

            var enumerable = collection.Cast<object>().ToList();
            debug = enumerable.Aggregate(debug, (current, item) => current + (item + "\n"));

            debug += showCount ? "Total Count : " + enumerable.Count + "\n" : string.Empty;
            debug += "__________________________";
            UnityEngine.Debug.Log(debug);
        }
    }
}