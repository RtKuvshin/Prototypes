using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private CuttingCounter _cuttingCounter;
    [SerializeField] private Image fillImage;

    private void Start()
    {
        
        _cuttingCounter.OnProgressChange += CuttingCounterOnProgressChange;
    
        fillImage.fillAmount = 0;
        Hide();
    }

    private void CuttingCounterOnProgressChange(object sender, CuttingCounter.OnProgressChangeEventArgs e)
    {
        fillImage.fillAmount = e.progressNormalized;
        if (e.progressNormalized <= 0 || e.progressNormalized == 1)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
}
