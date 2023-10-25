using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Settings/Difficulty Progression Settings")]
public class DifficultyProgressionSettings : ScriptableObject
{
    [CurveRange(0,0, 3, 5)] public AnimationCurve SpawnRateOverTime;
    [CurveRange(0,0, 3, 15)] public AnimationCurve SpawnDelayOverTime;

}
