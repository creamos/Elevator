using System;
using System.Linq;
using System.Collections.Generic;
using NaughtyAttributes;
using ScriptableEvents;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    [SerializeField, BoxGroup("Listened Events")]
    private GameEvent gameStarted, gameOver;
    
    [field: ShowNonSerializedField] public bool IsRunning { get; private set; }
    private Coroutine spawnRoutine;
    private float startTime, lastSpawnTime, nextSpawnTime;

    [SerializeField, Expandable] private DifficultyProgressionSettings difficultyProgressSettings;
    
    [BoxGroup("NOT USED ANYMORE")][SerializeField, CurveRange(0,0, 3, 5)]
    private AnimationCurve spawnRateOverTime;
    [BoxGroup("NOT USED ANYMORE")][SerializeField, CurveRange(0,0, 3, 15)] 
    private AnimationCurve spawnDelayOverTime;

    private int[] floorIDs;


    [SerializeField, Header("DEBUG")] private bool infiniteSpawn;

    private void OnEnable()
    {
        if (gameStarted)
        {
            gameStarted.OnTriggered -= OnGameStarted;
            gameStarted.OnTriggered += OnGameStarted;
        }

        if (gameOver)
        {
            gameOver.OnTriggered -= OnGameOver;
            gameOver.OnTriggered += OnGameOver;
        }
    }

    private void OnDisable()
    {
        if (gameStarted) gameStarted.OnTriggered -= OnGameStarted;
        if (gameOver) gameOver.OnTriggered -= OnGameOver;
    }

    private void OnGameStarted()
    {
        StartSpawnLoop();
    }

    private void OnGameOver()
    {
        if (infiniteSpawn) return;
        
        StopSpawnLoop();
    }

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
            
            int spawnCount = Mathf.RoundToInt(difficultyProgressSettings.SpawnRateOverTime.Evaluate(inGameTime/60.0f));

            // in case multiple pawns are spawned, avoid putting them on the same one
            List<int> availableFloors = new List<int>(floorIDs);

            bool successfulSpawn = true;
            for (int i = 0; i < spawnCount; ++i)
            {
                successfulSpawn = TrySpawnPawn(ref availableFloors);
                if (!successfulSpawn) break;
            }
            
            if (!successfulSpawn) return;
            
            lastSpawnTime = nextSpawnTime;
            float delay = difficultyProgressSettings.SpawnDelayOverTime.Evaluate(inGameTime / 60.0f);
            nextSpawnTime = lastSpawnTime + delay;
            
            Debug.Log($"Spawning {spawnCount} pawn, next spawn in: {delay} sec...");
        }
    }

    private bool TrySpawnPawn(ref List<int> availableFloors)
    {
        int chosenFloor = Random.Range(0, availableFloors.Count);
        int floorID = availableFloors[chosenFloor];
        availableFloors.RemoveAt(chosenFloor);
        
        return FloorManager.Instance.Floors[floorID].TrySpawnPawn();
    }
}