using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
public enum GraphName
{
    None,
    TemperatureGraph,
    PressureGraph,
    DiastolicGraph,
    SystolicGraph,
    HeartRateGraph,
    PRBpmGraph,
    SpO2Graph,
}

[RequireComponent(typeof(HealthDataManager))]
public partial class HealthDataGraphManager : MonoBehaviour
{
    private GraphManager<float> graphManager;
    private List<DataGraph<float>> diastolicData = new(), prbpmData = new(), systolicData = new(), spO2Data = new(), temperatureData = new();
    private DataWeekCalculate diastolicWeekData, prbpmWeekData, systolicWeekData, spO2WeekData, temperatureWeekData;
    private int weekIndex;
    private GraphName currentGraph;

    private void Awake()
    {
        nextButton.onClick.AddListener(Nextbutton);
        backButton.onClick.AddListener(BackButton);
        
        graphManager = new();
        HealthDataManager.OnAddGraphData += AddGraph;
        InitializeGraph();
    }

    private void Start()
    {
        ClearGraph();
        SetHealthInformationDataUi("-", "-", "-", "-", "-");
    }

    private void Update()
    {
        if (HealthDataManager.RangeDataList.Count == 0)
        {
            nextButton.interactable = false;
            backButton.interactable = false;
        }
    }

    public void OnDestroy()
    {
        ClearGraph();
        HealthDataManager.OnAddGraphData -= AddGraph;
    }

    private void InitializeGraph()
    {
        graphManager.CreateGraph(GraphName.TemperatureGraph.ToString(), pointPrefab, edgePrefab, temperatureGraphContainer, 172, 240, Color.blue, Color.blue);
        graphManager.CreateGraph(GraphName.DiastolicGraph.ToString(), pointPrefab, edgePrefab, pressureGraphContainer, 172, 240, Color.blue, Color.blue);
        graphManager.CreateGraph(GraphName.SystolicGraph.ToString(), pointPrefab, edgePrefab, pressureGraphContainer, 172, 240, Color.green, Color.green);
        graphManager.CreateGraph(GraphName.PRBpmGraph.ToString(), pointPrefab, edgePrefab, heartRateGraphContainer, 155, 240, Color.blue, Color.blue);
        graphManager.CreateGraph(GraphName.SpO2Graph.ToString(), pointPrefab, edgePrefab, heartRateGraphContainer, 155, 240, Color.green, Color.green);
    }

    private void AddGraph()
    {
        weekIndex = HealthDataManager.RangeDataList.Count - 1;
        
        CheckButton();
        SetInformationData();
    }

    private void AddGraphData()
    {
        ClearGraph();
        ClearGraphData();

        diastolicData = DataWeekSort("Diastolic");
        prbpmData = DataWeekSort("PRbpm");
        systolicData = DataWeekSort("Systolic");
        spO2Data = DataWeekSort("SpO2");
        temperatureData = DataWeekSort("Temp");
        
        diastolicData.Sort();
        prbpmData.Sort();
        systolicData.Sort();
        spO2Data.Sort();
        temperatureData.Sort();
        
        DrawGraph();
    }

    private List<DataGraph<float>> DataWeekSort(string data)
    {
        var handlePrbp = new List<DataGraph<float>>();
        
        foreach (var rawDate in HealthDataManager.RangeDataList.ToList()[weekIndex].Key.rangeDate)
        {
            handlePrbp.Add(new DataGraph<float>()
            {
                Data = 0,
                Timestamp = rawDate
            });
        }
        
        foreach (var rawData in HealthDataManager.RangeDataList.ToList()[weekIndex].Value)
        {
            foreach (var dataGraph in handlePrbp)
            {
                var jobject = JObject.Parse(rawData.healthValue);
                if (dataGraph.Timestamp == rawData.date)
                {
                    dataGraph.Data = float.Parse(jobject[data].ToString());
                }
            }
        }

        return handlePrbp;
    }
    
    private void DrawGraph()
    {
        switch (currentGraph)
        {
            case GraphName.TemperatureGraph:
                graphManager.DrawGraph(graphManager.GetGraph(GraphName.TemperatureGraph.ToString()), temperatureData);
                temperatureWeekData = DataWeekCalculator(temperatureData);
                graphNameText.text = "Temperature Statistic";
                TemperatureCalculator();
                break;
            case GraphName.PressureGraph:
                graphManager.DrawGraph(graphManager.GetGraph(GraphName.DiastolicGraph.ToString()), diastolicData);
                graphManager.DrawGraph(graphManager.GetGraph(GraphName.SystolicGraph.ToString()), systolicData);
                diastolicWeekData = DataWeekCalculator(diastolicData);
                systolicWeekData = DataWeekCalculator(systolicData);
                graphNameText.text = "Pressure Statistic";
                PressureCalculator();
                break;
            case GraphName.HeartRateGraph:
                graphManager.DrawGraph(graphManager.GetGraph(GraphName.PRBpmGraph.ToString()), prbpmData);
                graphManager.DrawGraph(graphManager.GetGraph(GraphName.SpO2Graph.ToString()), spO2Data);
                prbpmWeekData = DataWeekCalculator(prbpmData);
                spO2WeekData = DataWeekCalculator(spO2Data);
                graphNameText.text = "Heart Rate Statistic";
                HeartRateCalculator();
                break;
            case GraphName.None:
                ClearGraph();
                break;
        }

    }

    private void TemperatureCalculator()
    {
        temperatureMinText.text = MathF.Round(temperatureWeekData.Min).ToString();
        temperatireMaxText.text = MathF.Round(temperatureWeekData.Max).ToString();
        temperatireAvgText.text = MathF.Round(temperatureWeekData.Average).ToString();
    }

    private void PressureCalculator()
    {
        pressureMinText.text = MathF.Round(systolicWeekData.Min).ToString() + "/" + MathF.Round(diastolicWeekData.Min).ToString();
        pressureMaxText.text = MathF.Round(systolicWeekData.Max).ToString() + "/" + MathF.Round(diastolicWeekData.Max).ToString();
        pressureAvgText.text = MathF.Round(systolicWeekData.Average).ToString() + "/" + MathF.Round(diastolicWeekData.Average).ToString();
    }

    private void HeartRateCalculator()
    {
        prbpmMinText.text = MathF.Round(prbpmWeekData.Min).ToString();
        prbpmMaxText.text = MathF.Round(prbpmWeekData.Max).ToString();
        prbpmAvgText.text = MathF.Round(prbpmWeekData.Average).ToString();
        spO2MinText.text = MathF.Round(spO2WeekData.Min).ToString();
        spO2MaxText.text = MathF.Round(spO2WeekData.Max).ToString();
        spO2AvgText.text = MathF.Round(spO2WeekData.Average).ToString();
    }

    private void Nextbutton()
    {
        weekIndex++;
        CheckButton();
    }

    private void BackButton()
    {
        weekIndex--;
        CheckButton();
    }

    private void CheckButton()
    {
        dateText.text = HealthDataManager.RangeDataList.ToList()[weekIndex].Key.startDate.ToString("dd MMMM yyyy") 
                        + " - " + HealthDataManager.RangeDataList.ToList()[weekIndex].Key.endDate.ToString("dd MMMM yyyy");
        
        
         var check = HealthDataManager.RangeDataList.Count;
        
         nextButton.interactable = true;
        
         backButton.interactable = weekIndex != 0;
        
         if (weekIndex >= check - 1)
         {
             nextButton.interactable = false;
         }
         
         AddGraphData();
    }

    private DataWeekCalculate DataWeekCalculator(List<DataGraph<float>> data)
    {
        return new DataWeekCalculate()
        {
            Min = data.Where(x => x.Data != 0).DefaultIfEmpty().Min(d => d?.Data ?? 0),
            Max = data.Where(x => x.Data != 0).DefaultIfEmpty().Max(d => d?.Data ?? 0),
            Average = data.Where(x => x.Data != 0).DefaultIfEmpty().Average(d => d?.Data ?? 0)
        };
    }

    public void ClearGraph()
    {
        graphManager.GetGraph(GraphName.TemperatureGraph.ToString()).ClearGraph();
        graphManager.GetGraph(GraphName.DiastolicGraph.ToString()).ClearGraph();
        graphManager.GetGraph(GraphName.SystolicGraph.ToString()).ClearGraph();
        graphManager.GetGraph(GraphName.PRBpmGraph.ToString()).ClearGraph();
        graphManager.GetGraph(GraphName.SpO2Graph.ToString()).ClearGraph();
    }

    private void ClearGraphData()
    {
        diastolicData.Clear();
        prbpmData.Clear();
        systolicData.Clear();
        spO2Data.Clear();
        temperatureData.Clear();
    }

    public void SetInformationData()
    {
        var dataList = HealthDataManager.RangeDataList.ToList()[^1].Value;
        var lastestData = dataList[^1].healthValue;
        var jobject = JObject.Parse(lastestData);
        var temperature = jobject["Temp"].ToString();
        var highpressure = jobject["Systolic"].ToString();
        var lowpressure = jobject["Diastolic"].ToString();
        var highHeartRate = jobject["PRbpm"].ToString();
        var lowHeartRate = jobject["SpO2"].ToString();
        SetHealthInformationDataUi(temperature, highpressure, lowpressure, highHeartRate, lowHeartRate);
    }

    public void SetGraph(GraphName graphName)
    {
        currentGraph = graphName;
        HealthDataManager.OnAddGraphData?.Invoke();
    }

    private void SetHealthInformationDataUi(string temperature, string highPressure, string lowPressure, string highHeartRate, string lowHeartRate)
    {
        temperatureText.text = temperature;
        highpressureText.text = highPressure;
        lowpressureText.text = lowPressure;
        highHeartRateText.text = highHeartRate;
        lowHeartRateText.text = lowHeartRate;
    }
}

public class DataWeekCalculate
{
    public float Min { get; set; }
    public float Max { get; set; }
    public float Average { get; set; }
}