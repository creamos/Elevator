using UnityEngine;

public class ElevatorInfo : MonoBehaviour
{
    [SerializeField] private FloorManager floorManager;
    [SerializeField] private Transform groundHeightTarget;
    
    [field: SerializeField] public bool IsAtFloorLevel { get; private set; }
    [field: SerializeField] public int FloorLevel { get; private set; }

    [SerializeField, Min(0)] private float floorDistanceThreshold;

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
            float distanceToFloor = Mathf.Abs(floor.GroundHeightTarget.position.y - groundHeightTarget.position.y);
            if (distanceToFloor < floorDistanceThreshold)
            {
                IsAtFloorLevel = true;
                FloorLevel = floor.Index;
                break;
            }
        }
    }
}