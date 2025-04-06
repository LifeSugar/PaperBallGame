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

        [Header("Audio")]
        [SerializeField] private EventReference getScoreSFX;
        [Header("VFX")]
        [SerializeField] private ParticleSystem getScoreVFX;
        void OnTriggerEnter (Collider other)
        {
            if (other.GetComponent<PaperBall>() != null)
            {
                
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
        }

    }
}
