using System;
using System.Collections;
using System.Collections.Generic;
using ScriptableEvents;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuAnimation : MonoBehaviour
{
    [SerializeField] private GameEvent startGameEvent;
    [SerializeField] private RectTransform logo;
    [SerializeField] private Image logoBackground;
    
    [Header("Options")]
    [SerializeField] private float animationDuration = 1f; 
    [SerializeField] private AnimationCurve animationYCurve;
    [SerializeField] private float animationYOffset = 100f;
    [SerializeField] private AnimationCurve alphaCurve;


    private float animationPlayTime = 0;

    private void Start()
    {
        startGameEvent.OnTriggered += PlayAnimation;
    }

    public void PlayAnimation()
    {
        animationPlayTime = 0;
        StartCoroutine(AnimateLogo());
    }

    private IEnumerator AnimateLogo()
    {
        float initalAlpha = logoBackground.color.a;
        float initialY = logo.transform.position.y;
        while (animationPlayTime < animationDuration)
        {
            yield return null;
            float animationYPos = animationYCurve.Evaluate(animationPlayTime / animationDuration) * animationYOffset;
            logo.transform.position = new Vector3(logo.transform.position.x, initialY + animationYPos, logo.transform.position.z);
            
            
            float alpha = alphaCurve.Evaluate(animationPlayTime / animationDuration) * initalAlpha;
            logoBackground.color = new Color(logoBackground.color.r, logoBackground.color.g, logoBackground.color.b, initalAlpha-alpha);
            
            animationPlayTime += Time.deltaTime;
        }
        
        gameObject.SetActive(false);
    }
}
