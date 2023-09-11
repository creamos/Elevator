using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomColor : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<SpriteRenderer>().color = Color.HSVToRGB(Random.Range(0, 1f), 0.75f, 1);
    }
}
