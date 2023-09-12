using System;
using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(ElevatorInfo))]
public class ElevatorContent : MonoBehaviour
{
    private FloorManager floors;
    private ElevatorInfo elevatorInfo;

    public int MaxPawns;
    [ReadOnly] public Pawn Passenger;

    private void Awake()
    {
        elevatorInfo = GetComponent<ElevatorInfo>();
    }

    private void Start()
    {
        floors = FloorManager.Instance;
    }

    private void Update()
    {
        if (elevatorInfo.IsAtFloorLevel && !elevatorInfo.OnPickupCooldown)
        {
            Floor floor = floors.Floors[elevatorInfo.FloorLevel];
            if (Passenger == null)
            {
                var passenger = floor.TryPickup();
                if (passenger)
                {
                    AddPassenger(passenger);
                }
            }

            else if (Passenger)
            {
                if (Passenger.Destination == floor.Index)
                {
                    ReleasePassenger();
                    elevatorInfo.SetPickupCooldown(0.25f);
                }
            }
        }
    }

    public void AddPassenger(Pawn pawn)
    {
        if (Passenger != pawn)
        {
            Passenger = pawn;
            pawn.GetInElevator();

            // Do additional things when a pawn is successfully added to the elevator
        }
        else
        {
            // The pawn already is in the elevator (potential logical error).
        }
    }

    public void ReleasePassenger()
    {
        Passenger.Release();
        Passenger = null;
        // Do additional things when a pawn is successfully removed from the elevator
        
    }
}