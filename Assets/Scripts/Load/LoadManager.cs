using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LoadManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup loadingCanvas;
    [SerializeField] private Image loading;
    private static LoadManager instance;
    private readonly object lockObj = new();
    public static bool IsPlaying { get; set; }
    private Coroutine load;
    
    private void Awake()
    {
        if (instance == null)
        {
            lock (lockObj)
            {
                if (instance == null) instance = this;
            }
        }
        
        loadingCanvas.SetActive(false);
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        if (load != null)
        {
            StopCoroutine(load);
            load = null;
        }

        instance = null;
    }

    public static void Loading(bool isLoad)
    {
        instance.loadingCanvas.SetActive(isLoad);
        instance.loading.fillClockwise = true;
        instance.loading.fillAmount = 0;
        
        if (isLoad)
        {
            if (instance.load != null)
            {
                instance.StopCoroutine(instance.load);
                instance.load = null;
            }
            
            instance.load = instance.StartCoroutine(instance.LoadDelay());
        }
        else
        {
            if (instance.load != null)
            {
                instance.StopCoroutine(instance.load);
                instance.load = null;
            }
        }
        
        IsPlaying = isLoad;
    }

    private IEnumerator LoadDelay()
    {
        var defaultLoadTime = 10;
        
        while (defaultLoadTime > 0)
        {
            var start = DOVirtual.Float(0, 1, 1, value =>
            {
                loading.fillAmount = value;
            }).OnComplete(() =>
            {
                loading.fillClockwise = false;
            }).SetEase(Ease.InOutSine);
            
            yield return start.WaitForCompletion();
            yield return new WaitForSeconds(0.15f);
            
            var end = DOVirtual.Float(1, 0, 1, value =>
            {
                loading.fillAmount = value;
            }).OnComplete(() =>
            {
                loading.fillClockwise = true;
            }).SetEase(Ease.InOutSine);
            
            yield return end.WaitForCompletion();
            yield return new WaitForSeconds(0.15f);
            defaultLoadTime -= 1;
        }

        IsPlaying = false;
        load = null;
    }
}
