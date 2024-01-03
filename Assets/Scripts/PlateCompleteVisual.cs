using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateCompleteVisual : MonoBehaviour
{
   [Serializable]
   public struct KitchenObjectSO_Gameobject
   {
      public KitchenObjectSO kitchenObjectSo;
      public GameObject gameObject;
   }
   
   [SerializeField] private PlateKitchenObject plateKitchenObject;
   [SerializeField] private List<KitchenObjectSO_Gameobject> kitchenObjectSOGameobjectList;

   private void Start()
   {
      plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnOnIngredientAdded;
      foreach (KitchenObjectSO_Gameobject kitchenObjectSoGameobject in kitchenObjectSOGameobjectList)
      {
         kitchenObjectSoGameobject.gameObject.SetActive(false);
      }
   }

   private void PlateKitchenObject_OnOnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
   {
      foreach (KitchenObjectSO_Gameobject kitchenObjectSoGameobject in kitchenObjectSOGameobjectList)
      {
         if (kitchenObjectSoGameobject.kitchenObjectSo == e.kitchenObjectSo)
         {
            kitchenObjectSoGameobject.gameObject.SetActive(true);
         }
      }
   }
}
