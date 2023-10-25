using System.Collections;
using NaughtyAttributes;
using ScriptableEvents;
using UnityEngine;

public class ElevatorInfo : MonoBehaviour
{
    public static ElevatorInfo Instance { get; private set; }
    
    [SerializeField] private FloorManager floorManager;

    [SerializeField] private IntEvent elevatorAtFloor;
    
    [field: SerializeField] public Transform GroundHeightTarget { get; private set; }
    [field: SerializeField] public Transform SeatTarget { get; private set; }
    [field: SerializeField] public Transform PivotPoint { get; private set; }
    
    [field: SerializeField, ReadOnly] public bool OnPickupCooldown { get; private set; }
    [field: SerializeField, ReadOnly] public bool IsAtFloorLevel { get; private set; }
    [field: SerializeField, ReadOnly] public bool IsReadyToPickup { get; private set; }
    [SerializeField] private float pickupAlignmentDuration;
    private float alignmentElapsedTime;
    [field: SerializeField, ReadOnly] public int FloorLevel { get; private set; }

    [SerializeField, Min(0)] private float floorDistanceThreshold = 0.5f;

    private Coroutine PickupCooldownRoutine;

    private void OnEnable()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Update()
    {
        UpdateFloorLevel();
    }

    private void UpdateFloorLevel()
    {
        bool wasAtFloorLevel = IsAtFloorLevel;
        
        IsAtFloorLevel = false;
        FloorLevel = -1;


        foreach (Floor floor in floorManager.Floors)
        {
            float distanceToFloor = Mathf.Abs(floor.GroundHeightTarget.position.y - GroundHeightTarget.position.y);
            if (distanceToFloor < floorDistanceThreshold)
            {
                alignmentElapsedTime += Time.deltaTime;
                alignmentElapsedTime = Mathf.Clamp(alignmentElapsedTime,  0, pickupAlignmentDuration);

                IsAtFloorLevel = true;
                FloorLevel = floor.Index;

                elevatorAtFloor.Raise(FloorLevel);
                
                break;
            }
        }

        if (!IsAtFloorLevel)
        {
            alignmentElapsedTime -= Time.deltaTime * 2f;
            alignmentElapsedTime = Mathf.Clamp(alignmentElapsedTime,  0, pickupAlignmentDuration);
            
            
            if(wasAtFloorLevel) elevatorAtFloor.Raise(-1);
        }
        
        IsReadyToPickup = alignmentElapsedTime >= pickupAlignmentDuration;
    }

    public void SetPickupCooldown(float cooldownDuration)
    {
        OnPickupCooldown = true;
        PickupCooldownRoutine = StartCoroutine(PickupCooldownCrt(cooldownDuration));

        IEnumerator PickupCooldownCrt(float duration)
        {
            yield return new WaitForSeconds(duration);
            OnPickupCooldown = false;
            PickupCooldownRoutine = null;
        }
    }
    
}