using System.Collections.Generic;

namespace DefaultNamespace
{
    public class ElevatorContent
    {
        public int MaxPawns;

        public List<Pawn> Pawns;

        public void AddPawn(Pawn pawn)
        {
            if (!Pawns.Contains(pawn))
            {
                Pawns.Add(pawn);
                
                // Do additional things when a pawn is successfully added to the elevator
            }
            else
            {
                // The pawn already is in the elevator (potential logical error).
            }
        }

        public void RemovePawn(Pawn pawn)
        {
            if (Pawns.Contains(pawn))
            {
                Pawns.Remove(pawn);

                // Do additional things when a pawn is successfully removed from the elevator
            }
            else
            {
                // The pawn wasn't in the elevator (potential logical error).
            }
        }
    }
}