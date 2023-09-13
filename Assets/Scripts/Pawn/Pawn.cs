using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PawnMovementInQueueBehaviour))]
public class Pawn : MonoBehaviour
{
    public UnityEvent Spawned, EnteredElevator, FellFromElevator, ExitedElevator;
    public UnityEvent ReadyToEnterElevator;

    [NonSerialized] public PawnMovementInQueueBehaviour MovementInQueueBehaviour;
    
    [field: SerializeField, ReadOnly] public int Destination { get; private set; }
    [field: SerializeField, ReadOnly] public bool IsInElevator { get; private set; }

    [SerializeField] private DestinationBubble destinationBubble;

    private void Awake()
    {
        MovementInQueueBehaviour = GetComponent<PawnMovementInQueueBehaviour>();
        MovementInQueueBehaviour.OnWaitingPosReached.RemoveListener(OnWaitingPosReached);
        MovementInQueueBehaviour.OnWaitingPosReached.AddListener(OnWaitingPosReached);

        MovementInQueueBehaviour.OnReadyToEnterElevator -= OnReadyToEnterElevator;
        MovementInQueueBehaviour.OnReadyToEnterElevator += OnReadyToEnterElevator;
    }

    private void OnDestroy()
    {
        if (MovementInQueueBehaviour)
        {
            MovementInQueueBehaviour.OnWaitingPosReached.RemoveListener(OnWaitingPosReached);
            MovementInQueueBehaviour.OnReadyToEnterElevator -= OnReadyToEnterElevator;
        }
    }

    public void Init(int destination)
    {
        Destination = destination;
        destinationBubble.Init(destination);
        
        Spawned?.Invoke();
    }
    

    private void OnWaitingPosReached()
    {
        
    }

    private void OnReadyToEnterElevator()
    {
        ReadyToEnterElevator?.Invoke();
        ShowDestinationBubble();
    }

    /// <summary>
    /// Called when the pawn is added to the elevator
    /// </summary>
    public void GetInElevator()
    {
        IsInElevator = true;
        MovementInQueueBehaviour.Stop();
        EnteredElevator?.Invoke();
        
        StartCoroutine(JumpInElevator());
    }

    /// <summary>
    /// Called when the pawn in the elevator reaches the desired floor
    /// </summary>
    public void Release()
    {
        IsInElevator = false;
        ExitedElevator?.Invoke();
        
        StartCoroutine(JumpOutElevator());
    }

    private void ShowDestinationBubble()
    {
        destinationBubble.Show();
    }
    

    private IEnumerator JumpInElevator()
    {
        yield return JumpTo(ElevatorInfo.Instance.SeatTarget, 1f, 0.5f);
        transform.parent = ElevatorInfo.Instance.transform;
    }

    private IEnumerator JumpOutElevator()
    {
        transform.parent = null;
        yield return JumpTo(FloorManager.Instance.Floors[Destination].ExitElevatorTarget, 1f, 0.5f);
        yield return JumpTo(FloorManager.Instance.Floors[Destination].ExitFloorTarget, 0f, 0.5f);
        Destroy(gameObject);
    }
    
    private IEnumerator JumpTo(Transform target, float height, float duration)
    {
        Vector3 initialPos = transform.position;

        float startTime = Time.time;
        float progress = 0;

        do
        {
            progress = (Time.time - startTime) / duration;
            progress = Mathf.Clamp01(progress);

            transform.position = Vector3.Lerp(initialPos, target.position, progress);
            if (progress <= 0.5f)
                transform.position += Vector3.up * Mathf.SmoothStep(0, height, progress * 2f);
            else
                transform.position += Vector3.up * Mathf.SmoothStep(height, 0, (progress - 0.5f) * 2f);

            yield return null;
            
        } while (progress < 1);
    }
}