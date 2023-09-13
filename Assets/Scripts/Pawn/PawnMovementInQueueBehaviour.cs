using System;
using NaughtyAttributes;
using ScriptableEvents;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class PawnMovementInQueueBehaviour : MonoBehaviour
{
    public UnityEvent OnWaitingPosReached;
    public event Action OnReadyToEnterElevator;
    
    [SerializeField] private float walkSpeed = 1;
    [field: SerializeField, ReadOnly] public bool WaitingPosReached { get; private set; }

    [field: ShowNonSerializedField, ReadOnly]
    private bool isReachingWaitingPos;
    
    private Vector3 waitingSlot;
    private int waitingSlotID;


    public void SetWaitingSlot(Vector3 newWaitingSlot, int slotID)
    {
        waitingSlot = newWaitingSlot;
        waitingSlotID = slotID;
        WaitingPosReached = false;
        isReachingWaitingPos = true;
    }

    private void Update()
    {
        if (isReachingWaitingPos)
        {
            const float delta = 0.003f;
            if ((transform.position - waitingSlot).sqrMagnitude <= delta)
            {
                isReachingWaitingPos = false;
                WaitingPosReached = true;
                OnWaitingPosReached?.Invoke();
                if (waitingSlotID == 0) OnReadyToEnterElevator?.Invoke();
            }
            else
            {
                MoveTowardsWaitingPos();
            }
        }
    }

    private void MoveTowardsWaitingPos()
    {
        float distanceLeft = (waitingSlot - transform.position).magnitude;
        Vector3 dir = (waitingSlot - transform.position) / distanceLeft;
        transform.position += dir * Mathf.Min(walkSpeed * Time.deltaTime, distanceLeft);
    }

    public void Stop()
    {
        enabled = false;
    }
}