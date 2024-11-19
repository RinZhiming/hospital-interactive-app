using UnityEngine;

namespace Utility
{
    public static class ArrayExtension
    {
        /// <summary>
        /// Method help iterate and set active in array
        /// </summary>
        /// <param name="data">GameObject Array Data</param>
        /// <param name="value">Boolean For Set Active</param>
        /// <returns></returns>
        public static bool SetActive(this GameObject[] data, bool value)
        {
            if (data == null) return false;
            if (data.Length == 0) return false;

            foreach (var obj in data)
            {
                if (!obj)
                    return false;
                obj.SetActive(value);
            }
            
            return true;
        }
    }
}