using System;
using System.Collections;
using ScriptableEvents;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DestinationBubble : MonoBehaviour
{
    [SerializeField] private GameEvent onPawnAngry;
    
    [SerializeField] private TextMeshProUGUI contentText;
    [SerializeField] private Image bubbleImage;

    [SerializeField] private AnimationCurve showScalingCurve;
    [SerializeField] private float showScalingDuration;

    [SerializeField] private Sprite[] normalBubbleSprites;
    [SerializeField] private Sprite[] angryBubbleSprites;

    [SerializeField] private AnimationCurve angerIdleRotation;
    [SerializeField] private float angerIdleSpeed, angerIdleAmplitude;
    
    private bool isAngry, isBubbleVisible, angryBubbleIdling;
    private float angerStartTime;

    public void Init(int destination)
    {
        Hide();
        contentText.text = Floor.GetDisplayIndex(destination).ToString();
        bubbleImage.sprite = normalBubbleSprites[Random.Range(0, normalBubbleSprites.Length)];
    }

    private void Update()
    {
        if (angryBubbleIdling && isBubbleVisible)
        {
            Vector3 rotation = transform.localRotation.eulerAngles;
            rotation.z = angerIdleAmplitude * angerIdleRotation.Evaluate((Time.time - angerStartTime) * angerIdleSpeed);
            transform.localRotation = Quaternion.Euler(rotation);
        }
    }

    public void SetAngry()
    {
        isAngry = true;
        if (isBubbleVisible) SetAngryBubble();
    }

    public void Hide()
    {
        transform.gameObject.SetActive(false);
        transform.localScale = Vector3.zero;
        isBubbleVisible = false;
    }

    public void Show()
    {
        isBubbleVisible = true;
        
        if (isAngry) SetAngryBubble();
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


    private void SetAngryBubble()
    {
        onPawnAngry.Raise();
        StartCoroutine(SetBubbleAngryCrt());
    }

    private IEnumerator SetBubbleAngryCrt()
    {
        Vector3 scale = transform.localPosition;
        
        float startTime = Time.time;
        float progress = 0;

        while (progress < 1)
        {
            progress = (Time.time - startTime) / 0.5f;
            progress = Mathf.Clamp01(progress);

            transform.localScale = Vector3.one * showScalingCurve.Evaluate(1-progress);

            yield return null;
        }

        bubbleImage.sprite = angryBubbleSprites[Random.Range(0, angryBubbleSprites.Length)];
        
        startTime = Time.time;
        progress = 0;
        
        while (progress < 1)
        {
            progress = (Time.time - startTime) / 0.5f;
            progress = Mathf.Clamp01(progress);

            transform.localScale = Vector3.one * showScalingCurve.Evaluate(progress);
            yield return null;
        }

        angryBubbleIdling = true;
        angerStartTime = Time.time;
    }
}