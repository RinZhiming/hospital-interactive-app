using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGraph<T>
{
    void DrawGraph(List<DataGraph<T>> dataList);
    void ClearGraph();
    List<Point<T>> GetPoints();
}