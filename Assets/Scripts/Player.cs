using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = System.Random;

public class Player : NetworkBehaviour, IKitchenObjectParent
{
    public static event Action OnAnyPlayerSpawned;
    
    public static void ResetStaticData()
    {
        OnAnyPlayerSpawned = null;
    }
    public static Player LocalInstance { get; private set; }
    
    public event Action OnPickedSomething; 
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;

    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }
    
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Transform kitchenObjectHoldPoint;
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private LayerMask collisionsLayerMask;
    [SerializeField] private List<Vector3> spawnPositionList;
    [SerializeField] private PlayerVisual _playerVisual;
    
    private bool isWalking;
    private Vector3 lastInteractDirection;
    private BaseCounter _selectedCounter;
    private KitchenObject _kitchenObject;

    private void Start()
    {
        GameInput.Instance.OnInteractAction += GameInputOnInteractAction;
        GameInput.Instance.OnInteractAlternateAction += GameInputOnInteractAlternateAction;

        PlayerData playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromClientId(OwnerClientId);
        _playerVisual.SetPlayerColor(KitchenGameMultiplayer.Instance.GetPlayerColor(playerData.colorId));
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;
        }

        transform.position = spawnPositionList[KitchenGameMultiplayer.Instance.GetPlayerDataIndexFromClientId(OwnerClientId)];
        
        OnAnyPlayerSpawned!.Invoke();
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManagerOnClientDisconnectCallback;
        }
        
    }

    private void NetworkManagerOnClientDisconnectCallback(ulong clientID)
    {
        if (clientID == OwnerClientId && HasKitchenObject())
        {
            KitchenObject.DestroyKitchenObject(GetKitchenObject());
        }
    }

    private void GameInputOnInteractAlternateAction()
    {
        if (KitchenGameManager.Instance.IsGamePlaying())
        {
            if (_selectedCounter != null)
            { 
                _selectedCounter.InteractAlternate(this);
            }
        }
    }

    private void GameInputOnInteractAction()
    {
        if (KitchenGameManager.Instance.IsGamePlaying())
        {
            if (_selectedCounter != null)
            { 
                _selectedCounter.Interact(this);
            }
        }
        
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        
        HandleMovement();
        HandleInteractions();
    }

    private void HandleInteractions()
    {
        
        Vector2 inputVector = GameInput.Instance.GetMovementVector();
        Vector3 moveDirection = new Vector3(inputVector.x, 0 , inputVector.y);

        if (moveDirection != Vector3.zero) lastInteractDirection = moveDirection;
        
        float interactDistance = 2f;
        if (Physics.Raycast(transform.position, lastInteractDirection, out RaycastHit hit, interactDistance, countersLayerMask))
        {
            if (hit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                if (baseCounter != _selectedCounter)
                {
                    SetSelectedCounter(baseCounter);
                }
            }
            else
            {
                SetSelectedCounter(null);
            }
        }
        else
        {
            SetSelectedCounter(null);
        }
    }
    private void HandleMovement()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementVector();
        Vector3 moveDirection = new Vector3(inputVector.x, 0 , inputVector.y);

        float moveDistance = Time.deltaTime * moveSpeed;
        float playerWidth = 0.7f;
        float playerHeight = 2f;
        bool canMove = !Physics.BoxCast(transform.position,  Vector3.one * playerWidth,  moveDirection, Quaternion.identity, moveDistance, collisionsLayerMask);

        if (!canMove)
        {
            Vector3 moveDirectionX = new Vector3(moveDirection.x, 0, 0).normalized;
            canMove = (moveDirection.x < -0.5f || moveDirection.x > 0.5f)   && !Physics.BoxCast(transform.position,  Vector3.one * playerWidth,  moveDirectionX, Quaternion.identity, moveDistance, collisionsLayerMask);

            if (canMove)
            {
                moveDirection = moveDirectionX;
            }
            else
            {
                Vector3 moveDirectionZ = new Vector3(0, 0, moveDirection.z).normalized;
                canMove = (moveDirection.z < -0.5f || moveDirection.z > 0.5f) && !Physics.BoxCast(transform.position,  Vector3.one * playerWidth,  moveDirectionZ, Quaternion.identity, moveDistance, collisionsLayerMask);
                if (canMove)
                {
                    moveDirection = moveDirectionZ;
                }
            }
        }
        
        if (canMove)
        {
            transform.position += moveDirection * moveDistance;
        }

        isWalking = moveDirection != Vector3.zero;
        //Debug.Log(inputVector);
        transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * rotationSpeed);
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        _selectedCounter = selectedCounter;
        OnSelectedCounterChanged?.Invoke(this,new OnSelectedCounterChangedEventArgs
        {
            selectedCounter = _selectedCounter
        } );
    }
    
    public Transform GetKitchenObjectFollowTransform()
    {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        _kitchenObject = kitchenObject;
        if (_kitchenObject != null)
        {
            OnPickedSomething?.Invoke();
        }
    }

    public KitchenObject GetKitchenObject()
    {
        return _kitchenObject;
    }

    public void ClearKitchenObject()
    {
        _kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return _kitchenObject != null;
    }

    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }
}
