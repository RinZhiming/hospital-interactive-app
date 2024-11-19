using System;
using System.Collections.Generic;
using UnityEngine;

namespace InspectorEditor
{
    [Serializable]
    public class SerializableQueue<T> : ISerializationCallbackReceiver
    {
        [SerializeField] private List<T> list = new();
        private Queue<T> queue = new();

        public void Enqueue(T item) => queue.Enqueue(item);
        public T Dequeue() => queue.Dequeue();
        public T Peek() => queue.Peek();
        public void Clear() => queue.Clear();
        public int Count => queue.Count;

        public void OnBeforeSerialize()
        {
            list.Clear();
            foreach (var item in queue)
            {
                list.Add(item);
            }
        }

        public void OnAfterDeserialize()
        {
            queue.Clear();
            foreach (var item in list)
            {
                queue.Enqueue(item);
            }
        }
    }
}