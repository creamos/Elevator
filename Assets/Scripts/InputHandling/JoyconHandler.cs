using NaughtyAttributes;
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

    private Vector3 gyroRawInput;
    public Quaternion JoyconRotation;
    [ShowNonSerializedField] private Vector3 targetJoyconRotation = Vector3.zero;
    [SerializeField] private bool invertY, invertZ;


    private void Awake()
    {
        joyconManager = GetComponent<JoyconManager>();
    }

    private void Update()
    {
        HandleJoycon();
    }

    private bool HandleJoyconConnection()
    {
        if (joycon == null)
        {
            if (joyconManager.j is { Count: > 0 } && joyconManager.j[0] != null)
            {
                joycon = joyconManager.j[0];
            }
            else
            {
                JoyconRotation = Quaternion.identity;
                return false;
            }
        }

        return true;
    }

    private Vector3 GetGyro()
    {
        Vector3 gyro = joycon.GetGyro();
        gyro = new Vector3(gyro[1] * (invertY?-1:1), 0, gyro[2] * (invertZ?-1:1));
        return gyro;
    }

    private void HandleJoycon()
    {
        if (!HandleJoyconConnection()) return;

        targetJoyconRotation = GetGyro() * sensitivity;

        
        gyroRawInput = Vector3.Slerp(gyroRawInput, targetJoyconRotation, Time.deltaTime * sharpness);
        //Debug.Log((targetJoyconRotation - gyroRawInput).magnitude);
        
        JoyconRotation *= Quaternion.Euler(gyroRawInput);
        JoyconRotation = Quaternion.Lerp(JoyconRotation, Quaternion.identity, 1 - Mathf.Pow(0.25f, Time.deltaTime));
        JoyconRotation = Quaternion.Euler(Vector3.Scale(JoyconRotation.eulerAngles, new Vector3(1,0,1)));
        
        if (Input.GetKeyDown(KeyCode.Space))
            joycon.Recenter();
    }
}