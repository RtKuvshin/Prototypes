using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
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

    private NetworkVariable<float> fryingTimer = new NetworkVariable<float>(0);
    private NetworkVariable<float> burningTimer = new NetworkVariable<float>(0);
    private FryingRecipeSO _fryingRecipeSo;
    private BurningRecipeSO _burningRecipeSo;
    private NetworkVariable<State> currentState = new NetworkVariable<State>(State.Idle);

    public override void OnNetworkSpawn()
    {
        fryingTimer.OnValueChanged+= FryingTimerOnValueChanged;
        burningTimer.OnValueChanged+= BurningTimerOnValueChanged;
        currentState.OnValueChanged += CurrentStateOnValueChanged;
    }

    private void CurrentStateOnValueChanged(State previousvalue, State newvalue)
    {
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs()
        {
            state = currentState.Value
        });
        if (currentState.Value is State.Burned or State.Idle)
        {
            OnProgressChange?.Invoke(this, new  IHasProgress.OnProgressChangeEventArgs()
            {
                progressNormalized = 0
            });
        }
    }

    private void BurningTimerOnValueChanged(float previousvalue, float newvalue)
    {
        float burningTimerMax = _burningRecipeSo != null ? _burningRecipeSo.burningTimeMax : 1f;
        OnProgressChange?.Invoke(this, new  IHasProgress.OnProgressChangeEventArgs()
        {
            progressNormalized = burningTimer.Value/ burningTimerMax
        });

    }

    private void FryingTimerOnValueChanged(float previousvalue, float newvalue)
    {
        float fryingTimerMax = _fryingRecipeSo != null ? _fryingRecipeSo.fryingTimeMax : 1f;
        OnProgressChange?.Invoke(this, new  IHasProgress.OnProgressChangeEventArgs()
        {
            progressNormalized = fryingTimer.Value/fryingTimerMax
        });
    }

    private void Update()
    {
        if (!IsServer)
        {
            return;
        }
        if (this.HasKitchenObject())
        {
            switch (currentState.Value)
            {
                case State.Idle:
                    break;
                case State.Frying:
                    fryingTimer.Value += Time.deltaTime;

                    if (fryingTimer.Value > _fryingRecipeSo.fryingTimeMax)
                    {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                        KitchenObject.SpawnKitchenObject(_fryingRecipeSo.output, this);
                        currentState.Value = State.Fried;
                        
                        burningTimer.Value = 0;
                        SetBurningRecipeSoClientRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSoIndex(GetKitchenObject().GetKitchenObjectSO()));
                    }
                    break;
                case State.Fried:
                    burningTimer.Value += Time.deltaTime;

                    if (burningTimer.Value > _burningRecipeSo.burningTimeMax)
                    {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                        KitchenObject.SpawnKitchenObject(_burningRecipeSo.output, this);
                        currentState.Value = State.Burned;
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
                    KitchenObject kitchenObject = player.GetKitchenObject();
                        kitchenObject.SetKitchenObjectParent(this);
                    InteractLogicPlaceObjectOnCounterServerRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSoIndex(kitchenObject.GetKitchenObjectSO()));
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
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                        SetStateIdleServerRpc();
                    }
                }
            }
            else
            {
                this.GetKitchenObject().SetKitchenObjectParent(player);
                SetStateIdleServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetStateIdleServerRpc()
    {
        currentState.Value = State.Idle;
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlaceObjectOnCounterServerRpc(int kitchenObjectSOIndex)
    {
        fryingTimer.Value = 0;
        currentState.Value = State.Frying;
        SetFryingRecipeSoClientRpc(kitchenObjectSOIndex);
    }
    [ClientRpc]
    private void SetFryingRecipeSoClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSo =
            KitchenGameMultiplayer.Instance.GetKitchenObjectSoFromIndex(kitchenObjectSOIndex);
        _fryingRecipeSo = GetFryingRecipeSoWithInput(kitchenObjectSo);
    }
    [ClientRpc]
    private void SetBurningRecipeSoClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSo =
            KitchenGameMultiplayer.Instance.GetKitchenObjectSoFromIndex(kitchenObjectSOIndex);
        _burningRecipeSo = GetBurningRecipeSoWithInput(kitchenObjectSo);
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
        return currentState.Value == State.Fried;
    }
}
