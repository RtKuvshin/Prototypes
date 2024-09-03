using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;


public class MessageController : MonoBehaviour
{
    [SerializeField] private RectTransform messagePanel;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private float animationDuration;

    private Vector2 initialPosition;
    private Vector2 targetPosition;
    private const float OFFSET = 15;
    private const float WAIT_TIME = 4f;

    private void Awake()
    {
        Tutorial.Instance.OnStepChanged += OnStepChanged;
        InitializePanelPosition();
    }

    private void OnDestroy()
    {
        Tutorial.Instance.OnStepChanged -= OnStepChanged;
    }

    private void OnStepChanged(TutorialSteps obj)
    {
        string text = "";

        switch (obj)
        {
            case TutorialSteps.StartGame:
                text = "First of all you need \nto find the key 0_0";
                break;
            case TutorialSteps.Grab:
                text = "Press \"E\" to pick up the Key";
                break;
            case TutorialSteps.Exit:
                text = "Find the exit to escape \n The Spooky Mansion";
                break;
            case TutorialSteps.NeedKey:
                text = "You missed the key. \n Go and find it!";
                break;
        }
        
        StartCoroutine(ShowMessageCoroutine(text));
    }

    private void InitializePanelPosition()
    {
        initialPosition = messagePanel.anchoredPosition;
        
        float panelHeight = messagePanel.rect.height;
        targetPosition = new Vector2(initialPosition.x, -(panelHeight / 2 + OFFSET));
    }

    private void AnimatePanel(Vector2 position, Ease ease)
    {
        messagePanel.DOAnchorPos(position, animationDuration).SetEase(ease);
    }

    private IEnumerator ShowMessageCoroutine( string text)
    {
        SetText(text);
        AnimatePanel(targetPosition, Ease.OutCubic);

        yield return new WaitForSeconds(WAIT_TIME);
        AnimatePanel(initialPosition, Ease.InCubic);
    }

    private void SetText(string text)
    {
        messageText.text = text;
    }
}
