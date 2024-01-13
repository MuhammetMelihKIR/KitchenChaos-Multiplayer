using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class KitchenGameMultiplayer : NetworkBehaviour
{
    public static KitchenGameMultiplayer Instance { get; private set; }

    [SerializeField] private KitchenObjectListSO kitchenObjectListSO;
    private void Awake()
    {
        Instance = this;
    }
    
  public void SpawnKitchenObject(KitchenObjectSO kitchenObjectSo, IKitchenObjectParent kitchenObjectParent)
  {
     SpawnKitchenObjectServerRpc(GetKitchenObjectSOIndex(kitchenObjectSo), kitchenObjectParent.GetNetworkObject());
  }

  [ServerRpc(RequireOwnership = false)]
   private void SpawnKitchenObjectServerRpc(int kitchenObjectSoIndex, NetworkObjectReference kitchenObjectParentNetworkObjectReference)
   { 
       KitchenObjectSO kitchenObjectSo = GetKitchenObjectSOFromIndex(kitchenObjectSoIndex);
       Transform kitchenObjectTransform = Instantiate(kitchenObjectSo.prefab);
   
       NetworkObject kitchenObjectNetworkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
       kitchenObjectNetworkObject.Spawn(true);
       KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
       
       kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
       IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

       kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
   }
   private int GetKitchenObjectSOIndex(KitchenObjectSO kitchenObjectSo)
   {
       return kitchenObjectListSO.kitchenObjectSOList.IndexOf(kitchenObjectSo);
   }
   
   private KitchenObjectSO GetKitchenObjectSOFromIndex(int kitchenObjectSoindex)
   {
       return kitchenObjectListSO.kitchenObjectSOList[kitchenObjectSoindex];
   }
 
}
