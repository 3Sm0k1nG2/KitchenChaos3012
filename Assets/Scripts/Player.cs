using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour, IKitchenObjectParent {
    public static Player Instance { get; private set; }

    [SerializeField] private KitchenChaosGameManager gameManager;

    public event EventHandler OnPickedSomething;
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs {
        public BaseCounter selectedCounter;
    }

    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;

    private bool isWalking;
    private float playerRadius = .7f;
    private float playerHeight = 2f;

    private Vector3 lastInteractDir;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;

    private void Awake() {
        if(Instance != null) {
            Debug.LogError("There is more than one existing Player.");
        }
        Instance = this;
    }

    private void Start() {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
        gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e) {
        if (!gameManager.IsGamePlayingActive()) return;

        selectedCounter?.Interact(this);
    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e) {
        if (!gameManager.IsGamePlayingActive()) return;

        selectedCounter?.InteractAlternate(this);
    }

    private void Update() {
        HandleMovement();
        HandleInteraction();
    }

    private void HandleInteraction() {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if (moveDir != Vector3.zero) {
            lastInteractDir = moveDir;
        }

        float interactDistance = 2f;
        BaseCounter counter = null;
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, countersLayerMask)) {
            raycastHit.transform.TryGetComponent(out counter);
        };

        SetSelectedCounter(counter);
    }

    private void HandleMovement() {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;
        if (!CanMove(moveDir)) {
            Vector3 moveDirX = new Vector3(moveDir.x, 0f, 0f).normalized;
            if (CanMove(moveDirX)) {
                moveDir = moveDirX;
            } else {
                Vector3 moveDirZ = new Vector3(0f, 0f, moveDir.z).normalized;
                if (CanMove(moveDirZ)) {
                    moveDir = moveDirZ;
                }

            }
        }

        if (CanMove(moveDir)) {
            Move(moveDir);
        }

        isWalking = moveDir != Vector3.zero;

        float rotateSpeed = 20f;
        transform.forward += Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);

        bool CanMove(Vector3 moveDirection) {
            float moveDistance = moveSpeed * Time.deltaTime;
            return moveDirection != Vector3.zero && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirection, moveDistance);
        }

        void Move(Vector3 moveDirection) {
            float moveDistance = moveSpeed * Time.deltaTime;
            transform.position += moveDirection * moveDistance;
        }
    }

    public bool IsWalking() {
        return isWalking;
    }

    private void SetSelectedCounter(BaseCounter selectedCounter) {
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs {
            selectedCounter = this.selectedCounter
        });
    }

    public Transform GetKitchenObjectFollowTransform() {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject) {
        this.kitchenObject = kitchenObject;

        if (kitchenObject == null) return;

        OnPickedSomething?.Invoke(this, EventArgs.Empty);
    }

    public KitchenObject GetKitchenObject() { return kitchenObject; }
    public void ClearKitchenObject() {
        this.kitchenObject = null;
    }

    public bool HasKitchenObject() {
        return kitchenObject != null;
    }
}
