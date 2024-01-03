using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;

public class PlateKitchenObject : KitchenObject
{
    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;
    public class OnIngredientAddedEventArgs : EventArgs
    {
        public KitchenObjectSO kitchenObjectSo;
    }
    [SerializeField] private List<KitchenObjectSO> validKitchenObjectSoList;
    
    private List<KitchenObjectSO> kitchenObjectSoList;

    private void Awake()
    {
        kitchenObjectSoList = new List<KitchenObjectSO>();
    }

    public bool TryAddIngredient(KitchenObjectSO kitchenObjectSo)
    {
        if (!validKitchenObjectSoList.Contains(kitchenObjectSo))
        {
            return false;
        }
        if (kitchenObjectSoList.Contains(kitchenObjectSo))
        {
            return false;
        }
        else
        {
            kitchenObjectSoList.Add(kitchenObjectSo);
            OnIngredientAdded?.Invoke(this,new OnIngredientAddedEventArgs
            {
                kitchenObjectSo = kitchenObjectSo
            });
            return true;
        }
       
    }

    public List<KitchenObjectSO> GetKitchenObjectSOList()
    {
        return kitchenObjectSoList;
    }
    
}
