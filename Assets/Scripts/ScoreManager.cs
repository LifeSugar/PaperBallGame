using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace PaperBallGame
{
    public class ScoreManager : MonoBehaviour
    {
        public int currentScore;
        public int bestScore;

        public int ballsRemained;

        public int totalScores;


        public TextMeshProUGUI bestScoreT;
        public TextMeshProUGUI currentScoreT;

        public TextMeshProUGUI totalScoresT;


        public static ScoreManager instance;

        void Awake()
        {
          instance = this;  
        }


        void Update()
        {

        }

        public void GetScore(int scorePerBall)
        {
            currentScore += 1;
            totalScores += scorePerBall;
            RefreshScoreUI();
            
        }


        public void RefreshScoreUI()
        {
            if (currentScore >= bestScore)
            {
                bestScore = currentScore;
            }

            bestScoreT.text = bestScore.ToString();
            currentScoreT.text = currentScore.ToString();
            totalScoresT.text = totalScores.ToString();

        }


    }

}