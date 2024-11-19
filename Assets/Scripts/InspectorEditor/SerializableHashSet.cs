using System;
using System.Collections.Generic;
using UnityEngine;

namespace InspectorEditor
{
    [Serializable]
    public class SerializableHashSet<T> : ISerializationCallbackReceiver
    {
        [SerializeField] private List<T> list = new();
        private HashSet<T> hast = new();

        public bool Add(T item) => hast.Add(item);
        public bool Remove(T item) => hast.Remove(item);
        public bool Contains(T item) => hast.Contains(item);
        public void Clear() => hast.Clear();
        public int Count => hast.Count;
        
        public IEnumerator<T> GetEnumerator()
        {
            return hast.GetEnumerator();
        }
        
        public void OnBeforeSerialize()
        {
            list.Clear();
            foreach (var item in hast)
            {
                list.Add(item);
            }
        }

        public void OnAfterDeserialize()
        {
            hast.Clear();
            foreach (var item in list)
            {
                hast.Add(item);
            }
        }
    }
}