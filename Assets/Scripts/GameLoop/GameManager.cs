using NaughtyAttributes;
using ScriptableEvents;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private SpawnManager spawnManager;

    [field: SerializeField, BoxGroup("Raised Events")]
    public GameEvent GameStart  { get; private set; }
    
    [field: SerializeField, BoxGroup("Raised Events")]
    public GameEvent GameOver  { get; private set; }
    
    [field: SerializeField, BoxGroup("Raised Events")]
    public BoolEvent GamePause { get; private set; }

    [SerializeField, BoxGroup("Listened Events")]
    private GameEvent queueOverflow;

    private void OnEnable()
    {
        if (queueOverflow)
        {
            queueOverflow.OnTriggered -= OnGameOver;
            queueOverflow.OnTriggered += OnGameOver;
        }
    }

    private void OnDisable()
    {
        if (queueOverflow) queueOverflow.OnTriggered -= OnGameOver;
    }

    private void Start()
    {
        GameStart.Raise();
        spawnManager.StartSpawnLoop();
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
}