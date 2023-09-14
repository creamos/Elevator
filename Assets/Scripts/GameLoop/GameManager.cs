using NaughtyAttributes;
using ScriptableEvents;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [field: SerializeField, BoxGroup("Raised Events")]
    public GameEvent GameStart  { get; private set; }
    
    [field: SerializeField, BoxGroup("Raised Events")]
    public GameEvent GameOver  { get; private set; }
    
    [field: SerializeField, BoxGroup("Raised Events")]
    public BoolEvent GamePause { get; private set; }
    
    [field: SerializeField, BoxGroup("Raised Events")]
    public GameEvent OpenMenuRequest { get; private set; }

    [SerializeField, BoxGroup("Listened Events")]
    private GameEvent queueOverflow, startGameRequest, scoreRecapClosed;

    [field: ShowNonSerializedField]
    public bool IsGameRunning { get; private set; }
    
    private void OnEnable()
    {
        if (queueOverflow)
        {
            queueOverflow.OnTriggered -= OnGameOver;
            queueOverflow.OnTriggered += OnGameOver;
        }

        if (startGameRequest)
        {
            startGameRequest.OnTriggered -= OnStartGameRequested;
            startGameRequest.OnTriggered += OnStartGameRequested;
        }

        if (scoreRecapClosed)
        {
            scoreRecapClosed.OnTriggered -= OnScoreRecapClosed;
            scoreRecapClosed.OnTriggered += OnScoreRecapClosed;
        }
    }

    private void OnDisable()
    {
        if (queueOverflow) queueOverflow.OnTriggered -= OnGameOver;
        if (startGameRequest) startGameRequest.OnTriggered -= OnStartGameRequested;
        if (scoreRecapClosed) scoreRecapClosed.OnTriggered -= OnScoreRecapClosed;
    }

    private void OnStartGameRequested()
    {
        if (IsGameRunning) return;

        IsGameRunning = true;
        GameStart.Raise();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            GameOver.Raise();
        }
    }

    private void OnGameOver()
    {
        GameOver.Raise();
    }

    private void OnScoreRecapClosed()
    {
        IsGameRunning = false;
        OpenMenuRequest.Raise();
    }
}