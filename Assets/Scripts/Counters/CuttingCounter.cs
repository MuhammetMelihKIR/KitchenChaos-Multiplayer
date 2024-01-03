using System;
using UnityEngine;
using UnityEngine.Serialization;

public class CuttingCounter : BaseCounter, IHasProgress
{
   public static event EventHandler OnAnyCut;

   new public static void ResetStaticData()
   {
      OnAnyCut=null;
   }
   public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
   public event EventHandler OnCut;
   
   [SerializeField] private CuttingRecipeSO[] cuttingRecipeSoArray;

   private int cuttingProgress;
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
               cuttingProgress = 0;
               CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
               OnProgressChanged?.Invoke(this,new IHasProgress.OnProgressChangedEventArgs
               {
                  progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax
               });
            }
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
               } 
            }
         }
         else
         {
            //player
            GetKitchenObject().SetKitchenObjectParent(player);
         }
      }
      
   }

   public override void InteractAlternate(Player player)
   {
      if (HasKitchenObject()&& HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))
      {
         // There is a kitchen object and it can be cut
         cuttingProgress++;
         
         OnCut?.Invoke(this,EventArgs.Empty);
         OnAnyCut?.Invoke(this,EventArgs.Empty);
         
         CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

         OnProgressChanged?.Invoke(this,new IHasProgress.OnProgressChangedEventArgs
         {
            progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax
         });
         
         if (cuttingProgress >= cuttingRecipeSO.cuttingProgressMax)
         {
            KitchenObjectSO outputKitchenObjectSo = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());
            GetKitchenObject().DestroySelf();
            KitchenObject.SpawnKitchenObject(outputKitchenObjectSo, this);
            
         }
         
      }
   }
   
   private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSo)
   {
      CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSo);
      return cuttingRecipeSO != null;
   }

   private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSo)
   {
      CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSo);
      if (cuttingRecipeSO != null)
      {
         return cuttingRecipeSO.output;
      }
      else
      {
         return null;
      }
   }

   private CuttingRecipeSO GetCuttingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSo)
   {
      foreach (CuttingRecipeSO cuttingRecipeSo in cuttingRecipeSoArray)
      {
         if (cuttingRecipeSo.input == inputKitchenObjectSo)
         {
            return cuttingRecipeSo;
         }
      }
      return null;
   }
}

