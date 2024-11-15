using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PlatesCounterVisual : MonoBehaviour
{
    [SerializeField] private PlatesCounter _platesCounter;
    [SerializeField] private Transform topPoint;
    [SerializeField] private Transform prefabPlate;
    private List<GameObject> platesVisualList = new List<GameObject>();

    private void Start()
    {
        _platesCounter.OnPlateSpawned += PlatesCounterOnPlateSpawned;
        _platesCounter.OnPlateRemoved += PlatesCounterOnPlateRemoved;
    }

    private void PlatesCounterOnPlateRemoved()
    {
        GameObject plateGameObject = platesVisualList[platesVisualList.Count -1];
        platesVisualList.Remove(plateGameObject);
        Destroy(plateGameObject);
    }

    private void PlatesCounterOnPlateSpawned()
    {
        Transform platesVisualTransform = Instantiate(prefabPlate, topPoint);

        float plateOffsetY = 0.1f;
        platesVisualTransform.localPosition = new Vector3(0, plateOffsetY * platesVisualList.Count, 0);
        platesVisualList.Add(platesVisualTransform.gameObject);
    }
}
