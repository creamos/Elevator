using NaughtyAttributes;
using ScriptableEvents;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(ElevatorInfo))]
public class ElevatorContent : MonoBehaviour
{
    [SerializeField, BoxGroup("Listened Events")]
    private GameEvent gameOver;
    
    [SerializeField, BoxGroup("Listened Events")]
    private GameEvent overSwing;
    
    [SerializeField, BoxGroup("Raised Events")]
    private GameEvent onPawnFall;
    
    [SerializeField, BoxGroup("Raised Events")]
    private PawnEvent onPawnDrop;
    
    [SerializeField, BoxGroup("Raised Events")]
    private PawnEvent onPawnPickup;

    [SerializeField] private float postPickupCooldownDuration = 0.5f;
    
    private FloorManager floors;
    private ElevatorInfo elevatorInfo;

    [ReadOnly] public Pawn Passenger;


    private void OnEnable()
    {
        if (gameOver)
        {
            gameOver.OnTriggered -= OnGameOver;
            gameOver.OnTriggered += OnGameOver;
        }

        if (overSwing)
        {
            overSwing.OnTriggered -= OnOverSwing;
            overSwing.OnTriggered += OnOverSwing;
        }
    }

    private void OnDisable()
    {
        
        if (gameOver)
            gameOver.OnTriggered -= OnGameOver;
        if (overSwing)
            overSwing.OnTriggered -= OnOverSwing;
    }

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
        if (elevatorInfo.IsAtFloorLevel && !elevatorInfo.OnPickupCooldown && elevatorInfo.IsReadyToPickup)
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
                    elevatorInfo.SetPickupCooldown(postPickupCooldownDuration);
                }
            }
        }
    }

    private void AddPassenger(Pawn pawn)
    {
        if (Passenger != pawn)
        {
            Passenger = pawn;
            pawn.GetInElevator();

            onPawnPickup.Raise(pawn);
            // Do additional things when a pawn is successfully added to the elevator
        }
        else
        {
            // The pawn already is in the elevator (potential logical error).
        }
    }

    private void ReleasePassenger()
    {
        Passenger.Release();
        var releasedPassenger = Passenger;
        Passenger = null;
        
        onPawnDrop.Raise(releasedPassenger);
        // Do additional things when a pawn is successfully removed from the elevator
        
    }

    private void DropPassenger()
    {
        if (Passenger == null) return;
        
        Passenger.FallFromElevator();
        Passenger = null;

        onPawnFall.Raise();
    }

    private void OnOverSwing()
    {
        DropPassenger();
    }

    private void OnGameOver()
    {
        DropPassenger();
    }
}