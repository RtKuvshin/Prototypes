using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class DeliveryManager : MonoBehaviour
{
    public static DeliveryManager Instance { get; private set; }
    
    public event Action OnRecipeSpawned; 
    public event Action OnRecipeCompleted;

    public event Action OnRecipeSuccess; 
    public event Action OnRecipeFailed; 

    [SerializeField] private RecipeListSO _recipeListSo;

    private List<RecipeSO> waitingRecipeSOList = new List<RecipeSO>();
    private float spawnRecipeTimer = 4f;
    private int waitingRecipeMax = 4;
    private int successfullRecipesAmount;

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
            if (KitchenGameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count < waitingRecipeMax)
            {
                RecipeSO waitingRecipeSO = _recipeListSo.recipeSOList[Random.Range(0, _recipeListSo.recipeSOList.Count)];
                waitingRecipeSOList.Add(waitingRecipeSO);
                
                OnRecipeSpawned?.Invoke();
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
                    successfullRecipesAmount++;
                    waitingRecipeSOList.RemoveAt(i);
                    
                    OnRecipeCompleted?.Invoke();
                    OnRecipeSuccess?.Invoke();
                    return;
                }
                
            }
        }
        OnRecipeFailed?.Invoke();
    }

    public List<RecipeSO> GetWaitingRecipeSOList()
    {
        return waitingRecipeSOList;
    }

    public int GetSuccessfullRecipesAmount()
    {
        return successfullRecipesAmount;
    }
}
