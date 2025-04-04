using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Xml.Serialization;

namespace PaperBallGame
{
    public class PaperBallManager : MonoBehaviour
    {
        public List<PaperBall> paperBallsInHand; // 纸团列表
        public int paperBallCount = 5;
        public float paperBallAngle = 30f;
        public List<float> paperBallAngles;
        public Vector3 FirstPos = new Vector3(-0.3f, 0.3f, 0.6f);
        public Vector3 LastPos = new Vector3(0.3f, 0.3f, 0.6f);

        public bool inSelection = false; // 是否在选择纸团

        public List<Vector3> paperPositions;

        public static PaperBallManager instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Debug.LogError("Multiple instances of PaperBallManager found!");
            }
        }

        void Start()
        {

            InitializePaperBalls();
            SetBallsTranform();
        }

        private void InitializePaperBalls()
        {
            float angleStep = paperBallAngle / (paperBallCount - 1);
            paperBallAngles = new List<float>();

            var distanceStep = (LastPos - FirstPos) / (paperBallCount - 1);
            paperPositions = new List<Vector3>();


            paperBallsInHand = new List<PaperBall>();
            for (int i = 0; i < paperBallCount; i++)
            {
                float angle = -paperBallAngle / 2 + i * angleStep;
                paperBallAngles.Add(angle);
                Vector3 postion = FirstPos + i * distanceStep;
                paperPositions.Add(postion);
                var newPaperBall = Instantiate(Resources.Load<PaperBall>("prefab/papers"), this.transform);
                newPaperBall.transform.parent = this.transform;
                newPaperBall.transform.localPosition = FirstPos - newPaperBall.transform.forward * i * 0.01f; // 设置纸团的位置
                paperBallsInHand.Add(newPaperBall);
            }
        }

        public void ReCalculateBallPos()
        {
            if (paperBallCount > 1)
            {
                float angleStep = paperBallAngle / (paperBallCount - 1);
                paperBallAngles = new List<float>();

                var distanceStep = (LastPos - FirstPos) / (paperBallCount - 1);
                paperPositions = new List<Vector3>();

                for (int i = 0; i < paperBallCount; i++)
                {
                    float angle = -paperBallAngle / 2 + i * angleStep;
                    paperBallAngles.Add(angle);
                    Vector3 postion = FirstPos + i * distanceStep;
                    paperPositions.Add(postion);
                }
            }
            else 
            {
                paperBallAngles =new List<float> {0f};

                paperPositions = new List<Vector3> {(FirstPos + LastPos) / 2f};
            }
        }

        public void SetBallsTranform()
        {
            this.transform.DOLocalMove(new Vector3(0, 0, 0), 0.5f).SetEase(Ease.OutQuad);
            for (int i = 0; i < paperBallsInHand.Count; i++)
            {
                var paperBall = paperBallsInHand[i];
                paperBall.transform.DOLocalMove(paperPositions[i] - InputManager.instance.mainCamera.transform.forward * i * 0.02f, 0.5f).SetEase(Ease.OutQuad)
                    .OnComplete(() =>
                    {
                        paperBall.SetOriginanPos(paperBall.transform.position);
                        InputManager.instance.gameState = GameState.BallSelect;
                    });
                paperBall.transform.DOLocalRotate(new Vector3(0, 0, paperBallAngles[i]), 0.5f).SetEase(Ease.OutQuad);
            }
        }

        public void ReSetPaperBall()
        {
            this.transform.DOMove(new Vector3(0.02f, -0.07f, -0.1f), 0.5f).SetEase(Ease.OutQuad);
            for (int i = 0; i < paperBallsInHand.Count; i++)
            {
                var paperBall = paperBallsInHand[i];
                paperBall.transform.DOLocalMove(FirstPos - InputManager.instance.mainCamera.transform.forward * i * 0.02f, 0.5f).SetEase(Ease.OutQuad);
                paperBall.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.5f).SetEase(Ease.OutQuad);
            }
            // InputManager.instance.gameState = GameState.BallTrhow;
        }

    }
}