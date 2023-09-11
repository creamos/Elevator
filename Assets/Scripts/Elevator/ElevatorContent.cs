using UnityEngine;

public class ElevatorContent : MonoBehaviour
{
    public int MaxPawns;
    public Pawn Passenger;

    public void AddPawn(Pawn pawn)
    {
        if (Passenger != pawn)
        {
            Passenger = pawn;

            // Do additional things when a pawn is successfully added to the elevator
        }
        else
        {
            // The pawn already is in the elevator (potential logical error).
        }
    }

    public void RemovePawn(Pawn pawn)
    {
        if (Passenger == pawn)
        {
            Passenger = null;

            // Do additional things when a pawn is successfully removed from the elevator
        }
        else
        {
            // The pawn wasn't in the elevator (potential logical error).
        }
    }
}