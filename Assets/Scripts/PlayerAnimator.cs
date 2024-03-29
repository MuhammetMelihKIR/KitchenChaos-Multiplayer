using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerAnimator : NetworkBehaviour
{
    private const string IS_WALKING = "IsWalking";
    private Animator animator;

     [SerializeField] Player player;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    
    }

    private void Update()
    {
        if(!IsOwner)
        {
            return;
        }
         animator.SetBool(IS_WALKING, player.IsWalking());
    }
}
