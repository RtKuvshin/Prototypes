using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonWithText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buttonText;
    private Button button;

    public Button Button => button;

    public void Initialize()
    {
        button = GetComponent<Button>();
    }

    public void SetText(string value)
    {
        buttonText.text = value; 
    }

}
