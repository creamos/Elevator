using System;
using System.Collections;
using NaughtyAttributes;
using ScriptableEvents;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PawnMovementInQueueBehaviour))]
public class Pawn : MonoBehaviour
{
    [SerializeField, BoxGroup("Listened Events")]
    private GameEvent gameOver;
    
    public UnityEvent Spawned, EnteredElevator, FellFromElevator, ExitedElevator;
    public UnityEvent ReadyToEnterElevator;

    [NonSerialized] public PawnMovementInQueueBehaviour MovementInQueueBehaviour;
    
    [field: SerializeField, ReadOnly] public int Destination { get; private set; }
    [field: SerializeField, ReadOnly] public int CurrentFloor { get; private set; }
    [field: SerializeField, ReadOnly] public bool IsInElevator { get; private set; }
    [field: SerializeField, ReadOnly] public bool IsWaitingElevator { get; private set; }

    [SerializeField] private DestinationBubble destinationBubble;

    public int ScoreValue = 1;

    
    private void OnEnable()
    {
        if (gameOver)
        {
            gameOver.OnTriggered -= OnGameOver;
            gameOver.OnTriggered += OnGameOver;
        }
    }

    private void OnDisable()
    {
        if (gameOver) gameOver.OnTriggered -= OnGameOver;
    }

    private void OnGameOver()
    {
        if (gameOver) gameOver.OnTriggered -= OnGameOver;

        MovementInQueueBehaviour.Stop();

        if (IsWaitingElevator) LeaveFloor();
    }
    
    
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

    public void Init(int destination, int currentFloor)
    {
        Destination = destination;
        CurrentFloor = currentFloor;
        destinationBubble.Init(destination);
        IsWaitingElevator = true;
        
        Spawned?.Invoke();
    }

    private void Update()
    {
        if (transform.position.y < -20f && !IsInElevator) Destroy(gameObject);
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
        IsWaitingElevator = false;
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
        CurrentFloor = Destination;
        ExitedElevator?.Invoke();
        
        StartCoroutine(JumpOutElevator());
    }

    private void LeaveFloor()
    {
        destinationBubble.Hide();
        StartCoroutine(LeaveFloorRoutine());
        
        IEnumerator LeaveFloorRoutine()
        {
            yield return JumpTo(FloorManager.Instance.Floors[CurrentFloor].ExitFloorTarget, 0f, 0.5f);
            Destroy(gameObject);
        }
    }

    [Button]
    public void FallFromElevator()
    {
        IsInElevator = false;
        var cachedTransform = transform;
        
        cachedTransform.parent = null;

        cachedTransform.position += Vector3.back * 2;
        
        var col = gameObject.AddComponent<SphereCollider>();
        var rb = gameObject.AddComponent<Rigidbody>();

        col.isTrigger = false;
        col.center = new Vector3(0, .5f, 0);
        rb.isKinematic = false;
        rb.useGravity = true;

        StartCoroutine(DelayedAddForce());
        
        IEnumerator DelayedAddForce()
        {
            yield return null;
            rb.AddForce(Vector3.back * 10, ForceMode.Impulse);
        }
    }

    private void ShowDestinationBubble()
    {
        destinationBubble.Show();
    }

    public void SetAngry()
    {
        destinationBubble.SetAngry();
    }
    

    private IEnumerator JumpInElevator()
    {
        yield return JumpTo(ElevatorInfo.Instance.SeatTarget, 1f, 0.5f);
        transform.parent = ElevatorInfo.Instance.SeatTarget;

        while (Quaternion.Angle(transform.localRotation, Quaternion.identity) > 0.1f)
        {
            transform.localRotation = Quaternion.Lerp(
                transform.localRotation, 
                Quaternion.identity, 
                1 - Mathf.Pow(0.25f, Time.deltaTime));

            yield return null;
        }
    }

    private IEnumerator JumpOutElevator()
    {
        transform.parent = null;
        yield return JumpTo(FloorManager.Instance.Floors[Destination].ExitElevatorTarget, 1f, 0.5f);
        LeaveFloor();
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