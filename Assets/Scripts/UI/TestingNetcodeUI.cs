using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TestingNetcodeUI : MonoBehaviour
{
   [SerializeField] private Button startHostbutton, startClientButton;

   private void Awake()
   {
      startHostbutton.onClick.AddListener(() =>
      {
         NetworkManager.Singleton.StartHost();
         Hide();
      });
      startClientButton.onClick.AddListener(() =>
      {
         NetworkManager.Singleton.StartClient();
         Hide();
      });
      
}

   private void Hide()
   {
      gameObject.SetActive(false);
   }
}
