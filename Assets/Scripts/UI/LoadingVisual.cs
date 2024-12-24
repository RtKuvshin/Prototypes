using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingVisual : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI loadingtext;
    [SerializeField] private float interval = 0.5f;

    private string baseText = "LOADING";

    private void Start()
    {
        StartCoroutine(AnimateLoadingDots());
    }

    private void OnDestroy()
    {
        StopCoroutine(AnimateLoadingDots());
    }

    private IEnumerator AnimateLoadingDots()
    {
        int dotCount = 0;
        int dotCountMax = 3;

        while (true)
        {
            dotCount = (dotCount % dotCountMax) + 1;
            loadingtext.text = baseText + new string('.', dotCount);
            yield return new WaitForSeconds(interval);
        }
    }
}
