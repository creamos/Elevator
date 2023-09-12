using System.Linq;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    [field: ShowNonSerializedField] public bool IsRunning { get; private set; }
    private Coroutine spawnRoutine;
    private float startTime, lastSpawnTime, nextSpawnTime;

    [SerializeField, CurveRange(0,0, 3, 5)] private AnimationCurve spawnRateOverTime;
    [SerializeField, CurveRange(0,0, 3, 15)] private AnimationCurve spawnDelayOverTime;

    private int[] floorIDs;

    private void Start()
    {
        floorIDs = FloorManager.Instance.Floors.Select((floor, index) => index).ToArray();
    }

    public void StartSpawnLoop()
    {
        IsRunning = true;
        startTime = Time.time;
        lastSpawnTime = startTime;
        nextSpawnTime = startTime + 4f;
    }

    public void StopSpawnLoop()
    {
        IsRunning = false;
    }

    private void Update()
    {
        if (!IsRunning) return;

        if (Time.time >= nextSpawnTime)
        {
            float inGameTime = Time.time - startTime;
            
            int spawnCount = Mathf.RoundToInt(spawnRateOverTime.Evaluate(inGameTime/60.0f));

            // in case multiple pawns are spawned, avoid putting them on the same one
            List<int> availableFloors = new List<int>(floorIDs);   
            
            for (int i = 0; i < spawnCount; ++i)
                SpawnPawn(ref availableFloors);
            
            lastSpawnTime = nextSpawnTime;
            float delay = spawnDelayOverTime.Evaluate(inGameTime / 60.0f);
            nextSpawnTime = lastSpawnTime + delay;
            
            Debug.Log($"Spawning {spawnCount} pawn, next spawn in: {delay} sec...");
        }
    }

    private void SpawnPawn(ref List<int> availableFloors)
    {
        int chosenFloor = Random.Range(0, availableFloors.Count);
        int floorID = availableFloors[chosenFloor];
        availableFloors.RemoveAt(chosenFloor);
        
        FloorManager.Instance.Floors[floorID].SpawnPawn();
    }
}