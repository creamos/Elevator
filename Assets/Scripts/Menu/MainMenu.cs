using NaughtyAttributes;
using ScriptableEvents;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [field: SerializeField, BoxGroup("Raised Events")]
    public GameEvent StartGameRequest { get; private set; }

    [SerializeField, BoxGroup("Listened Events")]
    private GameEvent openMenuRequest, gameStarted;

    [SerializeField] private JoyconHandler joyconHandler;
    [SerializeField] private KeyCode startGameKey;

    [SerializeField] private Canvas canvas;

    private bool inMenu;

    private void OnEnable()
    {
        if (openMenuRequest)
        {
            openMenuRequest.OnTriggered -= Show;
            openMenuRequest.OnTriggered += Show;
        }

        if (gameStarted)
        {
            gameStarted.OnTriggered -= Hide;
            gameStarted.OnTriggered += Hide;
        }
    }

    private void OnDestroy()
    {
        if (openMenuRequest) openMenuRequest.OnTriggered -= Show;
        if (gameStarted) gameStarted.OnTriggered -= Hide;
    }

    private void Start()
    {
        Show();
    }

    private void Update()
    {
        if (Input.GetKeyDown(startGameKey))
            OnStartKeyPressed();
    }

    private void OnStartKeyPressed()
    {
        if (inMenu) 
            StartGameRequest.Raise();
    }

    private void Show()
    {
        inMenu = true;
        canvas.gameObject.SetActive(true);
    }

    private void Hide()
    {
        inMenu = false;
        canvas.gameObject.SetActive(false);
    }
}
