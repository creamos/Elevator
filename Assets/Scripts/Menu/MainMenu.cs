using System.Collections;
using System.Threading.Tasks;
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
    private bool inputsEnabled;

    private Coroutine inputEnablingRoutine;

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
        if (inMenu && inputsEnabled) 
            StartGameRequest.Raise();
    }

    private void Show()
    {
        inMenu = true;
        canvas.gameObject.SetActive(true);

        EnableInputs(1f);
    }

    private void Hide()
    {
        inMenu = false;
        canvas.gameObject.SetActive(false);

        DisableInputs();
    }

    private void EnableInputs(float delay)
    {
        inputEnablingRoutine = StartCoroutine(Routine(delay));
        
        IEnumerator Routine(float t)
        {
            yield return new WaitForSeconds(t);
            inputsEnabled = true;
            inputEnablingRoutine = null;
        }
    }

    private void DisableInputs()
    {
        inputsEnabled = false;
        
        if (inputEnablingRoutine == null) return;
        StopCoroutine(inputEnablingRoutine);
        inputEnablingRoutine = null;
    }
}
