using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class PawnVisuals : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Pawn pawn;

    private bool exitElevator = false; 
    
    private void Start()
    {
        pawn.ExitedElevator.AddListener(OnExitedElevator);
        
    }

    void Update()
    {
        if(!exitElevator)
            animator.SetBool("isWalking", pawn.MovementInQueueBehaviour.IsReachingWaitingPos);
        else 
            animator.SetBool("isWalking", true);

    }
    
    private void OnExitedElevator()
    {
        Debug.Log("On Exit Elevator");
        exitElevator = true;
        animator.gameObject.transform.localScale = new Vector3(-100, 100, 100);
    }
    
}
