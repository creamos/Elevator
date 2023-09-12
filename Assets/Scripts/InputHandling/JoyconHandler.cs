using UnityEngine;

[RequireComponent(typeof(JoyconManager))]
[SelectionBase]
public class JoyconHandler : MonoBehaviour
{
    private JoyconManager joyconManager;
    private Joycon joycon;

    public float Acceleration;

    private void Awake()
    {
        joyconManager = GetComponent<JoyconManager>();
    }

    private void Update()
    {
        HandleJoycon();
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
}
