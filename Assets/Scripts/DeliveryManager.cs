using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class DeliveryManager : MonoBehaviour
{
    public static DeliveryManager Instance { get; private set; }
    
    [SerializeField] private RecipeListSO _recipeListSo;

    private List<RecipeSO> waitingRecipeSOList = new List<RecipeSO>();
    private float spawnRecipeTimer = 4f;
    private int waitingRecipeMax = 4;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    { 
        StartCoroutine(SpawnRecipe());
    }
    
    private IEnumerator SpawnRecipe()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnRecipeTimer);
            if (waitingRecipeSOList.Count < waitingRecipeMax)
            {
                RecipeSO waitingRecipeSO = _recipeListSo.recipeSOList[Random.Range(0, _recipeListSo.recipeSOList.Count)];
                waitingRecipeSOList.Add(waitingRecipeSO);
                Debug.Log(waitingRecipeSO.recipeName);
            }
        }
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
    {
        for (int i = 0; i < waitingRecipeSOList.Count; i++)
        {
            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

            if (waitingRecipeSO.KitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count)
            {
                bool plateContentsMatchesRecipe = true;
                foreach (var recipeKitchenObjectSo in waitingRecipeSO.KitchenObjectSOList)
                {
                    bool ingredientFound = false;
                    foreach (var plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())
                    {
                        if (plateKitchenObjectSO == recipeKitchenObjectSo)
                        {
                            ingredientFound = true;
                            break;
                        }
                    }

                    if (!ingredientFound)
                    {
                        plateContentsMatchesRecipe = false;
                    }
                }

                if (plateContentsMatchesRecipe)
                {
                    waitingRecipeSOList.RemoveAt(i);
                    Debug.Log("player delivered the correct recipe!");
                    return;
                }
                
            }
        }
        Debug.Log("player didn't deliver the correct recipe!");
    }
}
