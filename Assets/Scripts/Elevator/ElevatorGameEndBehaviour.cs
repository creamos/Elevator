using NaughtyAttributes;
using ScriptableEvents;
using UnityEngine;

public class ElevatorGameEndBehaviour : MonoBehaviour
{
    [field: SerializeField, BoxGroup("Raised Events")]
    public GameEvent ResetElevatorRequest { get; private set; }
    
    [SerializeField, BoxGroup("Listened Events")]
    private GameEvent GameOver;

    [SerializeField]
    private Elevator elevator;
    
    private void OnEnable()
    {
        if (GameOver)
        {
            GameOver.OnTriggered -= OnGameOver;
            GameOver.OnTriggered += OnGameOver;
        }
    }

    private void OnDestroy()
    {
        if (GameOver) GameOver.OnTriggered -= OnGameOver;
    }

    private void OnGameOver()
    {
        elevator.EnablePlayerInputs(false);
        ResetElevatorRequest?.Raise();
    }
}
