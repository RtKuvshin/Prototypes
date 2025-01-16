using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgress
{
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;

    public event EventHandler<IHasProgress.OnProgressChangeEventArgs> OnProgressChange;
    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }
    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned
    }
    
    [SerializeField] private FryingRecipeSO[] _fryingRecipeSoArray;
    [SerializeField] private BurningRecipeSO[] _burningRecipeSoArray;

    private float fryingTimer;
    private float burningTimer;
    private FryingRecipeSO _fryingRecipeSo;
    private BurningRecipeSO _burningRecipeSo;
    private State currentState;

    private void Start()
    {
        currentState = State.Idle;
    }

    private void Update()
    {
        if (this.HasKitchenObject())
        {
            switch (currentState)
            {
                case State.Idle:
                    break;
                case State.Frying:
                    fryingTimer += Time.deltaTime;

                    OnProgressChange?.Invoke(this, new  IHasProgress.OnProgressChangeEventArgs()
                    {
                        progressNormalized = fryingTimer/_fryingRecipeSo.fryingTimeMax
                    });
                    
                    if (fryingTimer > _fryingRecipeSo.fryingTimeMax)
                    {
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(_fryingRecipeSo.output, this);
                        currentState = State.Fried;
                        
                        burningTimer = 0;
                        _burningRecipeSo = GetBurningRecipeSoWithInput(GetKitchenObject().GetKitchenObjectSO());
                        
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs()
                        {
                            state = currentState
                        });

                    }
                    break;
                case State.Fried:
                    burningTimer += Time.deltaTime;

                    OnProgressChange?.Invoke(this, new  IHasProgress.OnProgressChangeEventArgs()
                    {
                        progressNormalized = burningTimer/_burningRecipeSo.burningTimeMax
                    });
                    
                    if (burningTimer > _burningRecipeSo.burningTimeMax)
                    {
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(_burningRecipeSo.output, this);
                        currentState = State.Burned;
                        
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs()
                        {
                            state = currentState
                        });
                        
                        OnProgressChange?.Invoke(this, new  IHasProgress.OnProgressChangeEventArgs()
                        {
                            progressNormalized = 0
                        });
                    }
                    break;
                case State.Burned:
                    break;
            }
            //Debug.Log(currentState);
        }
    }

    public override void Interact(Player player)
    {
        if (HasKitchenObject() == false)
        {
            if (player.HasKitchenObject())
            {
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    _fryingRecipeSo = GetFryingRecipeSoWithInput(this.GetKitchenObject().GetKitchenObjectSO());
                    currentState = State.Frying;
                    fryingTimer = 0;
                    
                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs()
                    {
                        state = currentState
                    });
                    
                    OnProgressChange?.Invoke(this, new  IHasProgress.OnProgressChangeEventArgs()
                    {
                        progressNormalized = fryingTimer/_fryingRecipeSo.fryingTimeMax
                    });
                }
            }
        }
        else
        {
            if (player.HasKitchenObject())
            {
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        GetKitchenObject().DestroySelf();
                        currentState = State.Idle;
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs()
                        {
                            state = currentState
                        });
                        OnProgressChange?.Invoke(this, new  IHasProgress.OnProgressChangeEventArgs()
                        {
                            progressNormalized = 0
                        });
                    }
                }
            }
            else
            {
                this.GetKitchenObject().SetKitchenObjectParent(player);
                currentState = State.Idle;
                
                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs()
                {
                    state = currentState
                });
                OnProgressChange?.Invoke(this, new  IHasProgress.OnProgressChangeEventArgs()
                {
                    progressNormalized = 0
                });
            }
        }
    }
    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingRecipeSO fryingRecipeSo = GetFryingRecipeSoWithInput(inputKitchenObjectSO);
        return fryingRecipeSo != null;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingRecipeSO fryingRecipeSo = GetFryingRecipeSoWithInput(inputKitchenObjectSO);
        return fryingRecipeSo.output;
    }

    private FryingRecipeSO GetFryingRecipeSoWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (FryingRecipeSO fryingRecipeSo in _fryingRecipeSoArray)
        {
            if (fryingRecipeSo.input == inputKitchenObjectSO)
            {
                return fryingRecipeSo;
            }
        }

        return null;
    }
    private BurningRecipeSO GetBurningRecipeSoWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (BurningRecipeSO burningRecipeSo in _burningRecipeSoArray)
        {
            if (burningRecipeSo.input == inputKitchenObjectSO)
            {
                return burningRecipeSo;
            }
        }

        return null;
    }

    public bool IsFired()
    {
        return currentState == State.Fried;
    }
}
