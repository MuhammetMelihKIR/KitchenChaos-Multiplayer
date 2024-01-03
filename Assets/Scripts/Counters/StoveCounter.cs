
using System;
using System.Collections;
using UnityEngine;

public class StoveCounter : BaseCounter,IHasProgress
{
   public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
   public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
   
   public class OnStateChangedEventArgs : EventArgs
   {
      public State state;
   }
   public enum State
   {
      Idle,Frying,Fried,Burned,
   }
   
   [SerializeField] private FryingRecipeSO[] fryingRecipeSoArray;
   [SerializeField] private BurningRecipeSO[] burningRecipeSoArray;
   private State state;
   private float fryingTimer;
   private FryingRecipeSO fryingRecipeSo;
   private BurningRecipeSO burningRecipeSo;
   private float burningTimer;

   private void Start()
   {
      state = State.Idle;
   }

   private void Update()
   {
      if (HasKitchenObject())
      {
         switch (state)
         {
            case State.Idle:
               break;
            case State.Frying:
               fryingTimer += Time.deltaTime;
               
               OnProgressChanged?.Invoke(this,new IHasProgress.OnProgressChangedEventArgs
               {
                  progressNormalized = fryingTimer / fryingRecipeSo.fryingTimerMax 
               });
               
               if (fryingTimer > fryingRecipeSo.fryingTimerMax)
               {
                  GetKitchenObject().DestroySelf();
                  KitchenObject.SpawnKitchenObject(fryingRecipeSo.output, this);
                  state = State.Fried;
                  burningTimer = 0f;
                  burningRecipeSo = GetBurningRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
                  
                  OnStateChanged?.Invoke(this,new OnStateChangedEventArgs
                  {
                     state = state
                  });
               }
               break;   
            case State.Fried:
               burningTimer += Time.deltaTime;
               
               OnProgressChanged?.Invoke(this,new IHasProgress.OnProgressChangedEventArgs
               {
                  progressNormalized = burningTimer / burningRecipeSo.burningTimerMax 
               });
               
               if (burningTimer > burningRecipeSo.burningTimerMax)
               {
                  GetKitchenObject().DestroySelf();
                  KitchenObject.SpawnKitchenObject(burningRecipeSo.output, this);
                  state = State.Burned;
                  
                  OnStateChanged?.Invoke(this,new OnStateChangedEventArgs
                  {
                     state = state
                  });
                  
                  OnProgressChanged?.Invoke(this,new IHasProgress.OnProgressChangedEventArgs
                  {
                     progressNormalized = 0f
                  });
                  
               }
               break;
            case State.Burned:
               break;
         
         }
      }
   }


   public override void Interact(Player player)
   {
      if (!HasKitchenObject())
      {
         //then is no kitchen object
         if (player.HasKitchenObject())
         {
            //player is carrying something
            if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())) 
            {
               //player is carrying something that can be cut
               player.GetKitchenObject().SetKitchenObjectParent(this);
               
               fryingRecipeSo = GetFryingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());  
               
               state = State.Frying;
               fryingTimer = 0f;
               OnStateChanged?.Invoke(this,new OnStateChangedEventArgs
               {
                  state = state
               });
               OnProgressChanged?.Invoke(this,new IHasProgress.OnProgressChangedEventArgs
               {
                  progressNormalized = fryingTimer / fryingRecipeSo.fryingTimerMax 
               });
            }
         }
         else
         {
            //player is not carrying anything
         }
      }
      else
      {
         //player has carriyng anything
         if (player.HasKitchenObject())
         {
            if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
            { //player is holding a plate
               if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
               {
                  GetKitchenObject().DestroySelf();
                  state = State.Idle;
                  OnStateChanged?.Invoke(this,new OnStateChangedEventArgs
                  {
                     state = state
                  });
                  OnProgressChanged?.Invoke(this,new IHasProgress.OnProgressChangedEventArgs
                  {
                     progressNormalized = 0f
                  });
               } 
            }
         }
         else
         {
            //player
            GetKitchenObject().SetKitchenObjectParent(player);
            
            state = State.Idle;
            OnStateChanged?.Invoke(this,new OnStateChangedEventArgs
            {
               state = state
            });
            OnProgressChanged?.Invoke(this,new IHasProgress.OnProgressChangedEventArgs
            {
               progressNormalized = 0f
            });
         }
      }
      

   }
   private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSo)
   {
      FryingRecipeSO fryingRecipeSo = GetFryingRecipeSOWithInput(inputKitchenObjectSo);
      return fryingRecipeSo != null;
   }

   private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSo)
   {
      FryingRecipeSO fryingRecipeSo = GetFryingRecipeSOWithInput(inputKitchenObjectSo);
      if (fryingRecipeSo != null)
      {
         return fryingRecipeSo.output;
      }
      else
      {
         return null;
      }
   }

   private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSo)
   {
      foreach (FryingRecipeSO fryingRecipeSo in fryingRecipeSoArray)
      {
         if (fryingRecipeSo.input == inputKitchenObjectSo)
         {
            return fryingRecipeSo;
         }
      }
      return null;
   }
   private BurningRecipeSO GetBurningRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSo)
   {
      foreach ( BurningRecipeSO burningRecipeSO in burningRecipeSoArray)
      {
         if (burningRecipeSO.input == inputKitchenObjectSo)
         {
            return burningRecipeSO;
         }
      }
      return null;
   }

   public bool IsFried()
   {
      return state == State.Fried;
   }
}
