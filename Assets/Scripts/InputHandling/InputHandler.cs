using UnityEngine;

[RequireComponent(typeof(JoyconManager))]
[SelectionBase]
public class InputHandler : MonoBehaviour
{
    [SerializeField] private Potentiometer potentiometer;
    private JoyconManager joyconManager;
    private Joycon joycon;

    public float Acceleration;

    private void Awake()
    {
        joyconManager = GetComponent<JoyconManager>();
    }

    private void Update ()
    {
        HandleJoycon();
        HandleAcceleration();
    }

    private void HandleJoycon()
    {
        if (joycon == null)
        {
            if (joyconManager.j is { Count: > 0 } && joyconManager.j[0] != null)
            {
                joycon = joyconManager.j[0];
            }
            else
            {
                Acceleration = 0;
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
            joycon.Recenter();
    }

    private void HandleAcceleration()
    {
        Acceleration = potentiometer.Value;
    }
}
