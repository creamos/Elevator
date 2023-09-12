using UnityEngine;

[RequireComponent(typeof(JoyconManager))]
[SelectionBase]
public class JoyconHandler : MonoBehaviour
{
    [SerializeField] private float sensitivity = 1f;
    
    private JoyconManager joyconManager;
    private Joycon joycon;

    public Vector3 Acceleration = Vector3.zero;

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
                Acceleration = Vector3.zero;
                return;
            }
        }

        Acceleration = joycon.GetAccel() * sensitivity;
        
        if (Input.GetKeyDown(KeyCode.Space))
            joycon.Recenter();
    }
}
