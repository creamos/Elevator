using System;
using ScriptableEvents;
using TMPro;
using UnityEngine;

namespace Score
{
    public class ScoreUI : MonoBehaviour
    {
        [SerializeField] private GameEvent onPawnDropped;
        [SerializeField] private GameEvent onGameStarted;
        
        [SerializeField] private TextMeshProUGUI scoreTMP;
        
        private int score = 0;

        private void Start()
        {
            onPawnDropped.OnTriggered += () => AddScore(1);
            onGameStarted.OnTriggered += ResetScore;
        }

        private void AddScore(int amount)
        {
            score += amount;
            scoreTMP.text = score.ToString();
        }
        
        private void ResetScore()
        {
            score = 0;
            scoreTMP.text = score.ToString();
        }

        public int GetScore()
        {
            return score;
        }
    }
}