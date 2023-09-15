using ScriptableEvents;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetFloorTextID : MonoBehaviour
{
    [SerializeField] private Floor floor;
    [SerializeField] private TextMeshPro text;
    [SerializeField] private IntEvent elevatorAtFloor;

    private void OnEnable()
    {
        floor.Initialized -= SetText;
        floor.Initialized += SetText;

        if (elevatorAtFloor)
        {
            elevatorAtFloor.OnTriggeredVariant -= OnElevatorAtFloor;
            elevatorAtFloor.OnTriggeredVariant += OnElevatorAtFloor;
        }
    }

    private void OnDisable()
    {
        floor.Initialized -= SetText;
        if (elevatorAtFloor) elevatorAtFloor.OnTriggeredVariant -= OnElevatorAtFloor;
    }

    private void SetText()
    {
        text.text = floor.DisplayIndex.ToString();
        ResetStyle();
    }

    private void OnElevatorAtFloor(int floorLevel)
    {
        if (floorLevel == -1 || floorLevel != floor.Index) ResetStyle();
        
        else
            Highlight();
    }

    private void ResetStyle()
    {
        text.fontStyle = FontStyles.Normal;
        text.color = Color.white;
    }

    private void Highlight()
    {
        text.fontStyle = FontStyles.Bold;
        text.color = Color.yellow;
    }
}
