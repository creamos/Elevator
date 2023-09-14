using System;
using ScriptableEvents;
using TMPro;
using UnityEngine;

namespace Score
{
    public class ScoreUI : MonoBehaviour
    {
        [SerializeField] private PawnEvent onPawnDropped;
        [SerializeField] private GameEvent onGameStarted, onGameOver;
        
        [SerializeField] private Canvas scoreUI;
        [SerializeField] private TextMeshProUGUI scoreTMP;
        
        private int score = 0;

        private void OnEnable()
        {
            if (onPawnDropped)
            {
                onPawnDropped.OnTriggeredVariant -= AddScore;
                onPawnDropped.OnTriggeredVariant += AddScore;
                
            }

            if (onGameStarted)
            {
                onGameStarted.OnTriggered -= OnGameStarted;
                onGameStarted.OnTriggered += OnGameStarted;
            }

            if (onGameOver)
            {
                onGameOver.OnTriggered -= OnGameOver;
                onGameOver.OnTriggered += OnGameOver;
            }
        }

        private void OnDestroy()
        {
            if (onPawnDropped) onPawnDropped.OnTriggeredVariant -= AddScore;
            if (onGameStarted) onGameStarted.OnTriggered -= OnGameStarted;
            if (onGameOver) onGameOver.OnTriggered -= OnGameOver;
        }

        private void Start()
        {
            Hide();
        }

        private void OnGameStarted()
        {
            ResetScore();
            Show();
        }

        private void OnGameOver()
        {
            Hide();
        }

        private void AddScore(Pawn pawn) => AddScore(pawn.ScoreValue);

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

        private void Show()
        {
            scoreUI.gameObject.SetActive(true);
        }

        private void Hide()
        {
            scoreUI.gameObject.SetActive(false);
        }
    }
}