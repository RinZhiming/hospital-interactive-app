using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HealthInformationManager : MonoBehaviour
{
    [FormerlySerializedAs("deviceDataGraphManager")] [SerializeField] private HealthDataGraphManager healthDataGraphManager;
    [SerializeField] private CanvasGroup mainHealthCanvasGroup, graphStatisticsCanvasGroup, temperatureCanvasGroup, pressureCanvasGroup, heartRateCanvasGroup;
    [SerializeField] private Button homeButton, backButton, temperatureButton, pressureButton, heartRateButton, healthInformationButton;
    public static bool IsActive { get; private set; }

    private void Awake()
    {
        homeButton.onClick.AddListener(HomeButton);
        backButton.onClick.AddListener(BackButton);
        temperatureButton.onClick.AddListener(TemperatureButton);
        pressureButton.onClick.AddListener(PressureButton);
        heartRateButton.onClick.AddListener(HeartRateButton);
        healthInformationButton.onClick.AddListener(HealthInformationButton);
    }

    private void Start()
    {
        graphStatisticsCanvasGroup.alpha = 0;
        graphStatisticsCanvasGroup.blocksRaycasts = false;
        temperatureCanvasGroup.alpha = 0;
        temperatureCanvasGroup.blocksRaycasts = false;
        pressureCanvasGroup.alpha = 0;
        pressureCanvasGroup.blocksRaycasts = false;
        heartRateCanvasGroup.alpha = 0;
        heartRateCanvasGroup.blocksRaycasts = false;
        mainHealthCanvasGroup.alpha = 0;
        mainHealthCanvasGroup.blocksRaycasts = false;
        IsActive = false;
    }

    private void HomeButton()
    {
        if (HealthDataManager.RangeDataList.Count > 0) healthDataGraphManager.SetInformationData();
        SetCanva(graphStatisticsCanvasGroup, false);
        SetCanva(temperatureCanvasGroup, false);
        SetCanva(pressureCanvasGroup, false);
        SetCanva(heartRateCanvasGroup, false);
        SetCanva(mainHealthCanvasGroup, false);
        healthInformationButton.gameObject.SetActive(true);
        IsActive = false;
    }

    private void BackButton()
    {
        if (HealthDataManager.RangeDataList.Count > 0) healthDataGraphManager.ClearGraph();
        SetCanva(graphStatisticsCanvasGroup, false);
        SetCanva(temperatureCanvasGroup, false);
        SetCanva(pressureCanvasGroup, false);
        SetCanva(heartRateCanvasGroup, false);
        SetCanva(mainHealthCanvasGroup, true);
    }

    private void TemperatureButton()
    {
        if (HealthDataManager.RangeDataList.Count > 0) healthDataGraphManager.SetGraph(GraphName.TemperatureGraph);
        SetCanva(graphStatisticsCanvasGroup, true);
        SetCanva(temperatureCanvasGroup, true);
        SetCanva(pressureCanvasGroup, false);
        SetCanva(heartRateCanvasGroup, false);
        SetCanva(mainHealthCanvasGroup, false);
    }

    private void PressureButton()
    {
        if (HealthDataManager.RangeDataList.Count > 0) healthDataGraphManager.SetGraph(GraphName.PressureGraph);
        SetCanva(graphStatisticsCanvasGroup, true);
        SetCanva(temperatureCanvasGroup, false);
        SetCanva(pressureCanvasGroup, true);
        SetCanva(heartRateCanvasGroup, false);
        SetCanva(mainHealthCanvasGroup, false);
    }

    private void HeartRateButton()
    {
        if (HealthDataManager.RangeDataList.Count > 0) healthDataGraphManager.SetGraph(GraphName.HeartRateGraph);
        SetCanva(graphStatisticsCanvasGroup, true);
        SetCanva(temperatureCanvasGroup, false);
        SetCanva(pressureCanvasGroup, false);
        SetCanva(heartRateCanvasGroup, true);
        SetCanva(mainHealthCanvasGroup, false);
    }

    private void HealthInformationButton()
    {
        SetCanva(graphStatisticsCanvasGroup, false);
        SetCanva(temperatureCanvasGroup, false);
        SetCanva(pressureCanvasGroup, false);
        SetCanva(heartRateCanvasGroup, false);
        SetCanva(mainHealthCanvasGroup, true);
        healthInformationButton.gameObject.SetActive(false);
        IsActive = true;
    }

    private void SetCanva(CanvasGroup canvasGroup, bool value)
    {
        canvasGroup.alpha = value ? 1 : 0;
        canvasGroup.blocksRaycasts = value;
    }
}
