using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ClearCounter : BaseCounter
{
   [SerializeField] private KitchenObjectSO kitchenObjectSo;
   
   public override void Interact(Player player)
   {
      if (!HasKitchenObject())
      {
         //then is no kitchen object
         if (player.HasKitchenObject())
         {
            //player is carrying something
            player.GetKitchenObject().SetKitchenObjectParent(this);
         }
      }
      else
      {
         //player has carriyng anything
         if (player.HasKitchenObject())
         {
            //player
            if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
            { //player is holding a plate
               if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
               {
                  GetKitchenObject().DestroySelf();
               } 
            }
            else
            {
               if (GetKitchenObject().TryGetPlate(out plateKitchenObject ))
               {
                  if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO()))
                  {
                     player.GetKitchenObject().DestroySelf();
                  }
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
   
}
