using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphManager<T>
{
    private Dictionary<string, IGraph<T>> graphList = new Dictionary<string, IGraph<T>>();

    public GraphManager()
    {
    }

    public void CreateGraph(string name, GameObject point, GameObject edge, RectTransform container, float xSize, float yMax, Color pointColor, Color edgeColor)
    {
        var newgraph = new Graph<T>(point, edge, container, xSize, yMax, pointColor, edgeColor);
        AddGraph(name, newgraph);
    }

    public void AddGraph(string name, IGraph<T> graph)
    {
        if (!graphList.ContainsKey(name)) graphList.TryAdd(name, graph);
    }

    public void RemoveGraph(string name)
    {
        if (graphList.ContainsKey(name)) graphList.Remove(name);
    }

    public Graph<T> GetGraph(string name)
    {
        return graphList.TryGetValue(name, out IGraph<T> graph) ? (Graph<T>)graph : null;
    }

    public void DrawGraph(IGraph<T> graph, List<DataGraph<T>> dataList)
    {
        graph.DrawGraph(dataList);
    }

    public int CountGraph()
    {
        return graphList.Count;
    }
}