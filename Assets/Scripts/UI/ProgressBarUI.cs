using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private GameObject hasProgressGameObject;
    [SerializeField] private Image fillImage;

    private IHasProgress _hasProgress;

    private void Start()
    {
        _hasProgress = hasProgressGameObject.GetComponent<IHasProgress>();
        if (_hasProgress == null)
        {
            Debug.LogError("GameObject " + hasProgressGameObject + "doesn't implement Interface IHasProgress");
        }
        _hasProgress.OnProgressChange += HasProgressOnProgressChange;
    
        fillImage.fillAmount = 0;
        Hide();
    }

    private void HasProgressOnProgressChange(object sender, IHasProgress.OnProgressChangeEventArgs e)
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
