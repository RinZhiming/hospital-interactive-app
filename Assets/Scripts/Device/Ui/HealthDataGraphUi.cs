using UnityEngine;
using UnityEngine.UI;

public partial class HealthDataGraphManager
{
    [SerializeField]
    private GameObject
    pointPrefab,
    edgePrefab;

    [SerializeField]
    private RectTransform
    temperatureGraphContainer,
    pressureGraphContainer,
    heartRateGraphContainer;

    [SerializeField]
    private Button
    nextButton,
    backButton;

    [SerializeField] private CanvasGroup temperatureCanvasGroup, pressureCanvasGroup, heartRateCanvasGroup;

    [SerializeField]
    private Text
    dateText,
    graphNameText,
    temperatureMinText,
    temperatireMaxText,
    temperatireAvgText,
    pressureMinText,
    pressureMaxText,
    pressureAvgText,
    prbpmMinText,
    prbpmMaxText,
    prbpmAvgText,
    spO2MinText,
    spO2MaxText,
    spO2AvgText,
    temperatureText,
    highpressureText,
    lowpressureText,
    highHeartRateText,
    lowHeartRateText;
}