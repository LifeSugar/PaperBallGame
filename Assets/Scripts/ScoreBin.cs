using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace PaperBallGame
{
    public class TrashBin : MonoBehaviour
    {
        public int scorePerBall = 100;

        void OnTriggerEnter (Collider other)
        {
            if (other.GetComponent<PaperBall>() != null)
            {
                ScoreManager.instance.GetScore(scorePerBall);
            }
        }

    }
}
