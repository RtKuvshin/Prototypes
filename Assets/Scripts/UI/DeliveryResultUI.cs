using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryResultUI : MonoBehaviour
{
    private const string POPUP = "Popup";
    [SerializeField] private Image iconImage;
    [SerializeField] private Image bgImage;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private DeliveryResultParameters _deliveryResultParameters;
    
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        DeliveryManager.Instance.OnRecipeSuccess += DeliveryManagerOnRecipeSuccess;
        DeliveryManager.Instance.OnRecipeFailed += DeliveryManagerOnRecipeFailed;
        gameObject.SetActive(false);
    }

    private void DeliveryManagerOnRecipeFailed()
    {
        gameObject.SetActive(true);
        _animator.SetTrigger(POPUP);
        bgImage.color = _deliveryResultParameters.failedColor;
        iconImage.sprite = _deliveryResultParameters.failedSprite;
        messageText.text = "DELIVERY\nFAILED";
    }

    private void DeliveryManagerOnRecipeSuccess()
    {
        gameObject.SetActive(true);
        _animator.SetTrigger(POPUP);
        bgImage.color = _deliveryResultParameters.successColor;
        iconImage.sprite = _deliveryResultParameters.successSprite;
        messageText.text = "DELIVERY\nSUCCESS";
    }
}
[Serializable]
public class DeliveryResultParameters    
{
    public Color successColor;
    public Color failedColor;
    public Sprite successSprite;
    public Sprite failedSprite;
}
