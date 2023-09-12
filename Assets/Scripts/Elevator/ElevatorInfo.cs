using System;
using System.Collections;
using UnityEngine;

public class ElevatorInfo : MonoBehaviour
{
    public static ElevatorInfo Instance;
    
    [SerializeField] private FloorManager floorManager;
    
    [field: SerializeField] public Transform GroundHeightTarget { get; private set; }
    [field: SerializeField] public Transform SeatTarget { get; private set; }
    
    [field: SerializeField] public bool OnPickupCooldown { get; private set; }
    [field: SerializeField] public bool IsAtFloorLevel { get; private set; }
    [field: SerializeField] public int FloorLevel { get; private set; }

    [SerializeField, Min(0)] private float floorDistanceThreshold;

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
        IsAtFloorLevel = false;
        FloorLevel = -1;

        foreach (Floor floor in floorManager.Floors)
        {
            float distanceToFloor = Mathf.Abs(floor.GroundHeightTarget.position.y - GroundHeightTarget.position.y);
            if (distanceToFloor < floorDistanceThreshold)
            {
                IsAtFloorLevel = true;
                FloorLevel = floor.Index;
                break;
            }
        }
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