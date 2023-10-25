using System.Collections;
using NaughtyAttributes;
using ScriptableEvents;
using UnityEngine;

[RequireComponent(typeof(JoyconHandler), typeof(ElevatorContent))]
public class FallOnWiggle : MonoBehaviour
{
    [field: SerializeField, BoxGroup("Raised Events")]
    public GameEvent OverSwinging { get; private set; }
    
    private JoyconHandler joyconHandler;
    private ElevatorContent elevatorContent;

    [SerializeField, Range(0,180f)] private float swingLimit = 45f;
    private bool isOnCooldown;
    [SerializeField] private float cooldown = 1f;
    
    private void Awake()
    {
        joyconHandler = GetComponent<JoyconHandler>();
        elevatorContent = GetComponent<ElevatorContent>();
    }

    private void Update()
    {
        if (IsOverSwinging && !isOnCooldown && elevatorContent.Passenger)
        {
            OverSwinging.Raise();

            StartCooldown();
            isOnCooldown = true;
        }
    }

    [ShowNativeProperty]
    private float swingAngle
    {
        get
        {
            if (!joyconHandler) return 1;
            return Quaternion.Angle(joyconHandler.JoyconRotation, Quaternion.identity);
        }
    }

    private bool IsOverSwinging => swingAngle >= swingLimit;

    private void StartCooldown()
    {
        isOnCooldown = true;
        StartCoroutine(CooldownRoutine(cooldown));

        IEnumerator CooldownRoutine(float t)
        {
            yield return new WaitForSeconds(t);
            isOnCooldown = false;
        }
    }
}