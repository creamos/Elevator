using System;

[Serializable]
public class Pawn
{
    public int Destination;
    public bool IsInElevator;

    public Pawn(int destination)
    {
        Destination = destination;
    }
}