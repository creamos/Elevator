using System;
using System.Collections.Generic;
using NaughtyAttributes;
using ScriptableEvents;
using UnityEngine;
using Random = UnityEngine.Random;

public class Floor : MonoBehaviour
{
    [field: SerializeField, BoxGroup("Raised Events")]
    public GameEvent QueueOverflow;

    public event Action Initialized;
    
    public int Index;
    public int DisplayIndex => GetDisplayIndex(Index);
    public static int GetDisplayIndex(int index) => index + 1;
    
    public List<Pawn> WaitingPawns;
    public Transform GroundHeightTarget, ExitElevatorTarget, ExitFloorTarget;

    [SerializeField] private Transform waitingPos, spawnPos;
    [SerializeField] private float offset;

    private int pawnCount;
    private int floorCount;

    public Vector3 GetWaitingPos(int positionInQueue) => offset * positionInQueue * Vector3.left + waitingPos.position;

    public void Init(int index, int floorCount, int maxPawns)
    {
        Index = index;
        this.floorCount = floorCount;
        
        pawnCount = 0;
        
        WaitingPawns = new List<Pawn>(maxPawns);
        for (int i = 0; i < maxPawns; ++i)
            WaitingPawns.Add(null);
        
        Initialized?.Invoke();
    }

    public Pawn TryPickup()
    {
        if (WaitingPawns[0] == null || !WaitingPawns[0].MovementInQueueBehaviour.WaitingPosReached)
            return null;
        
        var pawn = WaitingPawns[0];
        WaitingPawns[0] = null;
        pawnCount--;
        
        ShiftQueueContent();
        
        MovePawns();

        return pawn;
    }

    private void ShiftQueueContent()
    {
        for (int i = 1; i < WaitingPawns.Count; ++i)
        {
            if (WaitingPawns[i] == null) break;

            WaitingPawns[i - 1] = WaitingPawns[i];
            WaitingPawns[i] = null;
        }
    }

    public bool TrySpawnPawn(Pawn spawnedPrefab)
    {
        if (pawnCount == WaitingPawns.Count)
        {
            Overflow();
            return false;
        }

        var pawn = Instantiate(spawnedPrefab, spawnPos.position, Quaternion.identity);

        int destination = Random.Range(0, floorCount-1);
        if (destination >= Index) destination++;
        pawn.Init(destination, Index);
            
        WaitingPawns[pawnCount] = pawn;
        pawn.MovementInQueueBehaviour.SetWaitingSlot(GetWaitingPos(pawnCount), pawnCount);
            
        pawnCount += 1;

        return true;
    }

    private void MovePawns()
    {
        for (int pawnIndex = 0; pawnIndex < pawnCount; pawnIndex++)
        {
            var pawn = WaitingPawns[pawnIndex];
            pawn.MovementInQueueBehaviour.SetWaitingSlot(GetWaitingPos(pawnIndex), pawnIndex);
            
            
        }
    }
    
    private void Overflow()
    {
        Debug.Log($"Queue of floor {Index} is overflowing!");
        QueueOverflow.Raise();
    }

    public void ResetFloor()
    {
        for (int i = 0; i < WaitingPawns.Count; i++)
            WaitingPawns[i] = null;
        pawnCount = 0;
    }
}