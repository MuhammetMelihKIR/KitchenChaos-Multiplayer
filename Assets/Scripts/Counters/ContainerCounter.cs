using System;
using UnityEngine;
using UnityEngine.Serialization;

public class ContainerCounter : BaseCounter
{
    public event EventHandler OnPlayerGrabbedObject;
    [SerializeField] private KitchenObjectSO kitchenObjectSo;
    
    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            KitchenObject.SpawnKitchenObject(kitchenObjectSo, player);
            OnPlayerGrabbedObject?.Invoke(this,EventArgs.Empty);
        }
       
    }
}
