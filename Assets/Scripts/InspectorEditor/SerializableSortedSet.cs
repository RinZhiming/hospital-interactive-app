using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableSortedSet<T> : ISerializationCallbackReceiver
{
    [SerializeField] private List<T> list = new();
    private SortedSet<T> sortedSet = new();

    public void OnBeforeSerialize()
    {
        list.Clear();
        list.AddRange(sortedSet);
    }

    public void OnAfterDeserialize()
    {
        sortedSet.Clear();
        foreach (var item in list)
        {
            sortedSet.Add(item);
        }
    }
    
    public bool Add(T item) => sortedSet.Add(item);
    public bool Remove(T item) => sortedSet.Remove(item);
    public bool Contains(T item) => sortedSet.Contains(item);
    public void Clear() => sortedSet.Clear();
    public int Count => sortedSet.Count;
    public IEnumerator<T> GetEnumerator() => sortedSet.GetEnumerator();
}