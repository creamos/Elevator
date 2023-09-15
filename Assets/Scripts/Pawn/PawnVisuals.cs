using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PawnVisuals : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private Pawn pawn;

    [SerializeField] private List<GameObject> pawnVisuals;
    [SerializeField] private Transform visualsParent;
        
    private bool exitElevator = false; 
    
    private void Start()
    {
        pawn.ExitedElevator.AddListener(OnExitedElevator);
        
        int randomIndex = Random.Range(0, pawnVisuals.Count);

        for (int i = visualsParent.childCount - 1; i >= 0; i--)
        {
            Destroy(visualsParent.GetChild(i).gameObject);
        }
        
        Instantiate(pawnVisuals[randomIndex], visualsParent);
        animator = visualsParent.GetComponentInChildren<Animator>();
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
