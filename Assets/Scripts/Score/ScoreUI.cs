using TMPro;
using UnityEngine;

namespace Score
{
    public class ScoreUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreTMP;
        
        private int score = 0;
        
        public void AddScore(int amount)
        {
            score += amount;
            scoreTMP.text = score.ToString();
        }
        
        public void ResetScore()
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