using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class SpawnManager : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Key key;

    private void Start()
    {
       //KeySpawn();
       SpawnTest();
    }

    private void KeySpawn()
    {
        int randomValue = Random.Range(0, spawnPoints.Length);
        Instantiate(key, spawnPoints[randomValue].position, Quaternion.identity);
    }

    private void SpawnTest()
    {
        foreach (var spawnPoint in spawnPoints)
        {
            Instantiate(key, spawnPoint.position, Quaternion.identity);
        }
    }
}
