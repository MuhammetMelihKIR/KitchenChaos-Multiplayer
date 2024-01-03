using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour,IKitchenObjectParent
{
   public static Player Instance { get; private set; }

   public event EventHandler OnPickedSomething;
   public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
   public class OnSelectedCounterChangedEventArgs : EventArgs
   {
      public BaseCounter selectedCounter;
   }
   
   [SerializeField] private float moveSpeed = 7f;
   [SerializeField] private GameInput gameInput;
   [SerializeField] private LayerMask countersLayerMask;
   [SerializeField] private Transform kiitchenObjectHoldPoint;
   
   private bool _isWalking;
   private Vector3 _lastInteractDir;
   private BaseCounter _selectedCounter;
   private KitchenObject kitchenObject;

   private void Awake()
   {
      if (Instance != null)
      {
         Debug.LogError("There is more than one Player Instance in the Scene");
      }
      Instance = this;
   }

   private void Start()
   {
      gameInput.OnInteractAction += GameInputOnOnInteractAction; 
      gameInput.OnInteractAlternateAction += GameInputOnOnInteractAlternateAction;
   }

   private void GameInputOnOnInteractAlternateAction(object sender, EventArgs e)
   {
      if (!KitchenGameManager.Instance.IsGamePlaying()) return;
      
      if (_selectedCounter != null) {
         _selectedCounter.InteractAlternate(this);
      }
   }

   private void GameInputOnOnInteractAction(object sender, EventArgs e)
   {
      if (!KitchenGameManager.Instance.IsGamePlaying()) return;
      
      if (_selectedCounter != null) {
         _selectedCounter.Interact(this);
      }
   }

   private void Update()
   {
     HandleMovement();
     HandleInteractions();
   }
   
   public bool IsWalking()
   {
      return _isWalking;
   }

   private void HandleInteractions()
   {
      Vector2 inputVector = gameInput.GetMovementVectorNormalized();
      Vector3 moveDir = new Vector3(inputVector.x,0f,inputVector.y);

      if (moveDir != Vector3.zero) {
         _lastInteractDir= moveDir;
      }
      
      float interactDistance = 2f;
      if (Physics.Raycast(transform.position, _lastInteractDir,out RaycastHit raycastHit ,interactDistance,countersLayerMask))
      {
         if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter)) {
            if (baseCounter != _selectedCounter) {
               SetSelectedCounter(baseCounter);
            }   
         }         else {
            SetSelectedCounter(null);
         }
      }else {
         SetSelectedCounter(null);
      }
   }
   private void HandleMovement()
   {
      Vector2 inputVector = gameInput.GetMovementVectorNormalized();
      Vector3 moveDir = new Vector3(inputVector.x,0f,inputVector.y);

      float moveDistance = moveSpeed* Time.deltaTime;
      float playerRadius = 0.7f;
      float playerHeight = 2f;
      bool canMove = !Physics.CapsuleCast(transform.position,transform.position +Vector3.up* playerHeight, playerRadius, moveDir,moveDistance);

      if (!canMove) {
         // cannot move towards the moveDir
         // Attempt only x movement
         
         Vector3 moveDirX = new Vector3(moveDir.x,0f,0f).normalized;
         canMove = (moveDir.x < -0.5f || moveDir.x > 0.5f) && !Physics.CapsuleCast(transform.position,transform.position +Vector3.up* playerHeight, playerRadius, moveDirX,moveDistance);

         if (canMove) {
            // can move only x
            moveDir = moveDirX;
         }
         else {
            // cannot move only x
            
            // Attempt only z movement
            Vector3 moveDirZ = new Vector3(0F,0f,moveDir.z);
            canMove = (moveDir.z < -0.5f || moveDir.z > 0.5f) && !Physics.CapsuleCast(transform.position,transform.position +Vector3.up* playerHeight, playerRadius, moveDirZ,moveDistance);
            
            if (canMove) {
               // can move only z
               moveDir= moveDirZ;
            }
            else
            {
               // cannot move 
               
            }
         }
      }
      if (canMove) {
         transform.position += moveDir * moveDistance;
      }
      
      _isWalking = moveDir != Vector3.zero;

      float rotateSpeed = 10f;
      transform.forward = Vector3.Slerp(transform.forward,moveDir,Time.deltaTime * rotateSpeed);

   }
   
   private void SetSelectedCounter(BaseCounter selectedCounter)
   {
      this._selectedCounter = selectedCounter;
      OnSelectedCounterChanged ?.Invoke(this, new OnSelectedCounterChangedEventArgs
      {
         selectedCounter = _selectedCounter
      });
   }

   public Transform GetKitchenObjectFollowTransform()
   {
      return kiitchenObjectHoldPoint;
   }
   public void SetKitchenObject(KitchenObject kitchenObject)
   {
      this.kitchenObject = kitchenObject;
      if (kitchenObject!=null)
      {
         OnPickedSomething?.Invoke(this,EventArgs.Empty);
      }
   } public KitchenObject GetKitchenObject()
   {
      return kitchenObject;
   } public void ClearKitchenObject()
   {
      kitchenObject = null;
   } public bool HasKitchenObject()
   {
      return kitchenObject != null;
   }
 
}
