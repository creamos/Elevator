using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DestinationBubble : MonoBehaviour
{
    [SerializeField] private Text contentText;

    [SerializeField] private AnimationCurve showScalingCurve;
    [SerializeField] private float showScalingDuration;

    public void Init(int destination)
    {
        Hide();
        contentText.text = Floor.GetDisplayIndex(destination).ToString();
    }

    public void Hide()
    {
        transform.gameObject.SetActive(false);
        transform.localScale = Vector3.zero;
    }

    public void Show()
    {
        transform.localScale = Vector3.zero;
        transform.gameObject.SetActive(true);

        StartCoroutine(ShowCrt());

        IEnumerator ShowCrt()
        {
            float startTime = Time.time;

            float progress = 0;

            while (progress < 1)
            {
                progress = (Time.time - startTime) / showScalingDuration;
                progress = Mathf.Clamp01(progress);

                transform.localScale = Vector3.one * showScalingCurve.Evaluate(progress);

                yield return null;
            }
        }
    }
}