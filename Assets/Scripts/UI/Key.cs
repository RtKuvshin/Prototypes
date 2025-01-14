using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Key : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI keyText;
    
    public void SetText(string value)
    {
        keyText.text = value; 
    }

}
