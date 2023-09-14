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

    [SerializeField, BoxGroup("PawnSettings")]
    private Pawn[] defaultPawns;
    [SerializeField, BoxGroup("PawnSettings")]
    private Pawn[] bonusPawns;
    [SerializeField, BoxGroup("PawnSettings")]
    [Range(0,1)] private float bonusPawnSpawnChance;

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

            bool spawnBonusPawn = Random.Range(0f, 1f) <= bonusPawnSpawnChance;
            int bonusPawnFloor = Random.Range(0, spawnCount); 
            
            bool successfulSpawn = true;
            for (int i = 0; i < spawnCount; ++i)
            {
                Pawn spawnedPrefab = (spawnBonusPawn && bonusPawnFloor == i)
                    ? bonusPawns[Random.Range(0, bonusPawns.Length)]
                    : defaultPawns[Random.Range(0, defaultPawns.Length)];
                successfulSpawn = TrySpawnPawn(ref availableFloors, spawnedPrefab);
                if (!successfulSpawn) break;
            }
            
            if (!successfulSpawn) return;
            
            lastSpawnTime = nextSpawnTime;
            float delay = difficultyProgressSettings.SpawnDelayOverTime.Evaluate(inGameTime / 60.0f);
            nextSpawnTime = lastSpawnTime + delay;
            
            Debug.Log($"Spawning {spawnCount} pawn, next spawn in: {delay} sec...");
        }
    }

    private bool TrySpawnPawn(ref List<int> availableFloors, Pawn spawnedPrefab)
    {
        int chosenFloor = Random.Range(0, availableFloors.Count);
        int floorID = availableFloors[chosenFloor];
        availableFloors.RemoveAt(chosenFloor);
        
        return FloorManager.Instance.Floors[floorID].TrySpawnPawn(spawnedPrefab);
    }
}