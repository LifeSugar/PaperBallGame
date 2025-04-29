using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using TMPro;

namespace PaperBallGame
{
    public class TrashBin : MonoBehaviour
    {
        public int scorePerBall = 100;
        public Vector3 startPosition;
        public Vector3 targetUIPosition;
        public TextMeshProUGUI scoreText;
        
        public TrashType trashType;

        [Header("Audio")]
        [SerializeField] private EventReference getScoreSFX;
        [SerializeField] private EventReference loseScoreSFX;
        [Header("VFX")]
        [SerializeField] private ParticleSystem getScoreVFX;
        
        private Collider previousCollider;
        void OnTriggerEnter (Collider other)
        {
            if (other != previousCollider)
            {
                if (other.GetComponent<Trash>().trashType == trashType)
                {
                    previousCollider = other;
                    AudioManager.instance.PlayOneShot(getScoreSFX, this.transform.position);
                    getScoreVFX.Play();
                    scoreText.text ="+" + scorePerBall;
                    scoreText.rectTransform.localPosition = startPosition;
                    scoreText.gameObject.SetActive(true);
                    scoreText.rectTransform.DOLocalMove(targetUIPosition, 1.2f).SetEase(Ease.OutQuad)
                        .OnComplete(() =>
                        {
                            ScoreManager.instance.GetScore(scorePerBall);
                            scoreText.gameObject.SetActive(false);
                        });
                }
                else
                {
                    previousCollider = other;
                    AudioManager.instance.PlayOneShot(loseScoreSFX, this.transform.position);
                    // getScoreVFX.Play();
                    scoreText.text ="-" + scorePerBall;
                    scoreText.rectTransform.localPosition = startPosition;
                    scoreText.gameObject.SetActive(true);
                    scoreText.rectTransform.DOLocalMove(targetUIPosition, 1.2f).SetEase(Ease.OutQuad)
                        .OnComplete(() =>
                        {
                            ScoreManager.instance.GetScore(-scorePerBall);
                            scoreText.gameObject.SetActive(false);
                        });
                }
            }
        }

        public void RefreshPaperBall()
        {
            previousCollider = null;
        }

    }
}
