using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetFloorTextID : MonoBehaviour
{
    [SerializeField] private Floor floor;
    [SerializeField] private TextMeshPro text;

    private void OnEnable()
    {
        floor.Initialized -= SetText;
        floor.Initialized += SetText;
    }

    private void OnDisable()
    {
        floor.Initialized -= SetText;
    }

    private void SetText()
    {
        text.text = floor.DisplayIndex.ToString();
    }
}
