using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounter : BaseCounter
{
    public event Action OnPlateSpawned;
    public event Action OnPlateRemoved;
    
    [SerializeField] private KitchenObjectSO _plateKitchenObjectSo;

    private float spawnPlateTimer;
    private float spawnPlaeTimerMax = 4f;
    private int platesSpawnedAmount;
    private int platesSpawnedAmountMax = 4;
    

    private void Update()
    {
        spawnPlateTimer += Time.deltaTime;
        if (spawnPlateTimer > spawnPlaeTimerMax)
        {
            spawnPlateTimer = 0;
            if (KitchenGameManager.Instance.IsGamePlaying() && platesSpawnedAmount < platesSpawnedAmountMax)
            {
                platesSpawnedAmount += 1;
                OnPlateSpawned?.Invoke();
                
            }
        }
    }

    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            if (platesSpawnedAmount > 0)
            {
                platesSpawnedAmount -= 1;
                KitchenObject.SpawnKitchenObject(_plateKitchenObjectSo, player);
                OnPlateRemoved?.Invoke();
            }
        }
    }
}
