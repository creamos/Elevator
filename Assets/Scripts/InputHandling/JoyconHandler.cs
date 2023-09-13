using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(JoyconManager))]
[SelectionBase]
public class JoyconHandler : MonoBehaviour
{
    [SerializeField] private float sensitivity = 1f;
    [SerializeField] private float sharpness = 1f;
    
    private JoyconManager joyconManager;
    private Joycon joycon;

    public Vector3 JoyconRotation = Vector3.zero;
    private Vector3 targetJoyconRotation = Vector3.zero;
    

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
                JoyconRotation = Vector3.zero;
                return;
            }
        }

        targetJoyconRotation = joycon.GetGyro() * sensitivity;
        JoyconRotation = Vector3.Slerp(JoyconRotation, targetJoyconRotation, Time.deltaTime * sharpness);
        Debug.Log((targetJoyconRotation - JoyconRotation).magnitude);
        
        if (Input.GetKeyDown(KeyCode.Space))
            joycon.Recenter();
    }
}
