using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Graph<T> : IGraph<T>
{
    private List<Point<T>> points;
    private List<Edge<T>> edges;

    private GameObject pointPrefab, edgeprefab;
    private RectTransform container;
    private float xSize, ySize, yMax;
    private Color pointColor, edgeColor;

    public Graph(GameObject point, GameObject edge, RectTransform container, float xSize, float yMax, Color pointColor, Color edgeColor)
    {
        pointPrefab = point;
        edgeprefab = edge;
        this.container = container;
        this.xSize = xSize;
        this.yMax = yMax;
        ySize = container.sizeDelta.y;
        this.pointColor = pointColor;
        this.edgeColor = edgeColor;
        points = new();
        edges = new();
    }

    public void DrawGraph(List<DataGraph<T>> dataList)
    {
        DrawAllPoint(dataList);
    }

    private void DrawAllPoint(List<DataGraph<T>> dataList)
    {
        for (int i = 0; i < dataList.Count; i++)
        {
            float xPos = i * xSize;
            float yPos = ((float)Convert.ToDouble(dataList[i].Data) / yMax) * ySize;
            GameObject newPoint = GameObject.Instantiate(pointPrefab, container);
            newPoint.GetComponent<Image>().color = pointColor;
            var rect = newPoint.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(xPos, yPos);
            rect.sizeDelta = new Vector2(30, 30);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.zero;
            var point = new Point<T>() { Position = rect.anchoredPosition, Data = dataList[i].Data, Timestamp = dataList[i].Timestamp, PointGameobject = newPoint };
            points.Add(point);
        }

        DrawAllEdge();
    }

    private void DrawAllEdge()
    {
        for (int i = 0; i < points.Count - 1; i++)
        {
            DrawEdge(points[i], points[i + 1]);
        }
    }

    private void DrawEdge(Point<T> startPoint, Point<T> endPoint)
    {
        GameObject newEdge = GameObject.Instantiate(edgeprefab, container);
        newEdge.GetComponent<Image>().color = edgeColor;
        var rect = newEdge.GetComponent<RectTransform>();
        var direction = (endPoint.Position - startPoint.Position).normalized;
        var distance = Vector2.Distance(startPoint.Position, endPoint.Position);
        rect.anchoredPosition = new Vector2(0, 0);
        rect.sizeDelta = new Vector2(distance, 7);
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.zero;
        rect.anchoredPosition = startPoint.Position + direction * distance * 0.5f;
        rect.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        edges.Add(new Edge<T>() { StartPoint = startPoint, EndPoint = endPoint, EdgeGameobject = newEdge });
    }

    public void ClearGraph()
    {
        foreach (var point in points)
        {
            GameObject.Destroy(point.PointGameobject);
        }

        foreach (var edge in edges)
        {
            GameObject.Destroy(edge.EdgeGameobject);
        }

        points.Clear();
        edges.Clear();
    }

    public List<Point<T>> GetPoints()
    {
        return points;
    }
}

public class DataGraph<T> : IComparable<DataGraph<T>>
{
    public T Data { get; set; }
    public DateTime Timestamp { get; set; }


    public int CompareTo(DataGraph<T> other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return Timestamp.CompareTo(other.Timestamp);
    }
}

public class Edge<T>
{
    public Point<T> StartPoint { get; set; }
    public Point<T> EndPoint { get; set; }
    public GameObject EdgeGameobject { get; set; }
}

public class Point<T>
{
    public T Data { get; set; }
    public Vector2 Position { get; set; }
    public GameObject PointGameobject { get; set; }
    public DateTime Timestamp { get; set; }

    public bool ContainsDate(DateTime timestamp)
    {
        return Timestamp.Equals(timestamp);
    }
}