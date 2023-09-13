using System;
using NaughtyAttributes;
using ScriptableEvents;
using UnityEngine;

public class GameOverMenu : MonoBehaviour
{
    [field: SerializeField, BoxGroup("Raised Events")]
    public GameEvent ScoreRecapClosed { get; private set; }

    [SerializeField, BoxGroup("Listened Events")]
    private GameEvent gameOver;

    [SerializeField] private JoyconHandler joyconHandler;
    [SerializeField] private KeyCode exitKey;

    [SerializeField] private Canvas canvas;

    private bool inMenu;
    
    private void OnEnable()
    {
        if (gameOver)
        {
            gameOver.OnTriggered -= Show;
            gameOver.OnTriggered += Show;
        }
    }

    private void OnDestroy()
    {
        if (gameOver) gameOver.OnTriggered -= Show;
    }

    private void Awake()
    {
        Hide();
    }

    private void Update()
    {
        if (Input.GetKeyDown(exitKey))
            OnExitKeyPressed();
    }
    
    private void OnExitKeyPressed()
    {
        if (inMenu) 
            Hide();
    }

    private void Show()
    {
        inMenu = true;
        canvas.gameObject.SetActive(true);
    }

    private void Hide()
    {
        inMenu = false;
        ScoreRecapClosed.Raise();
        canvas.gameObject.SetActive(false);
    }
}
