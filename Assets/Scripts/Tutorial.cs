using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public event Action<TutorialSteps> OnStepChanged;
    
    public static Tutorial Instance { get; private set; }
    public bool IsInit { get; private set; }

    private void Awake()
    {
        Instance = this;
        IsInit = true;
    }

    private void Start()
    {
        ChangeStep(TutorialSteps.StartGame);
    }

    public void ChangeStep(TutorialSteps newStep)
    {
        OnStepChanged?.Invoke(newStep);
    }
}
public enum TutorialSteps
{
    StartGame,
    Grab,
    Exit,
    NeedKey,
}
