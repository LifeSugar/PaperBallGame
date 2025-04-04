using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


namespace PaperBallGame
{
    public class InputManager : MonoBehaviour
    {


        public static InputManager instance;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }


        public float adjustCoefficient = 0.01f;
        [Range(0, 1.0f)] public float ThrowDirectionY = 0;

        private Vector2 startMousePos;
        private float startTime;
        private bool dragging = false;


        public Camera mainCamera;

        public GameState gameState = GameState.Menu; // 游戏状态

        [Header("激活的纸团")]
        public PaperBall activePaperBall; // 当前活动的纸团


        void Start()
        {
            mainCamera = Camera.main;
        }

        void Update()
        {
            if (gameState == GameState.BallTrhow)
            {
                ThrowPaperBall();
                if (activePaperBall)
                {
                    if (activePaperBall.rb.velocity.magnitude <= 0.2 && activePaperBall.thrown)
                    {
                        PaperBallManager.instance.ReCalculateBallPos();
                        PaperBallManager.instance.SetBallsTranform();
                        activePaperBall.inAir = false;
                        activePaperBall = null;
                    }
                }
            }
            else if (gameState == GameState.BallSelect)
            {
                SelectBall();
            }

        }

        void ThrowPaperBall()
        {
            if (Input.GetMouseButtonDown(0))
            {
                startMousePos = Input.mousePosition;
                startTime = Time.time;
                dragging = true;
            }

            if (Input.GetMouseButtonUp(0) && dragging)
            {
                dragging = false;
                Vector2 endMousePos = Input.mousePosition;
                float duration = Time.time - startTime;

                if (duration > 1f)
                {
                    Debug.Log("再试一次");
                    return;
                }

                if (endMousePos.y <= startMousePos.y)
                {
                    Debug.Log("再试一次");
                    return;
                }

                activePaperBall.thrown = true;

                Vector2 screenVelocity = (endMousePos - startMousePos) / duration;

                // 通过摄像机方向，将屏幕速度转换到世界空间中位于xz平面的速度

                Vector3 cameraRight = mainCamera.transform.right;
                // 摄像机的前方向投影到水平面（xz平面）
                Vector3 projectedForward = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up).normalized;

                Vector3 worldVelocity = (cameraRight * screenVelocity.x + projectedForward * screenVelocity.y) * adjustCoefficient;

                // 将计算得到的初速度赋予纸团的刚体
                activePaperBall.rb.isKinematic = false;
                activePaperBall.rb.velocity = worldVelocity + new Vector3(0, ThrowDirectionY * worldVelocity.z, 0);

                float spinCoefficient = 10f;
                Vector3 spinAxis = -Vector3.Cross(worldVelocity, Vector3.up).normalized;
                float spinSpeed = worldVelocity.magnitude * spinCoefficient;
                activePaperBall.rb.angularVelocity = spinAxis * spinSpeed;
                activePaperBall.inAir = true;

            }
        }

        void SelectBall()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            LayerMask mask = 1 << 9;
            if (Physics.Raycast(ray, out hitInfo, 100f, mask))
            {
                PaperBall paperBall = hitInfo.collider.GetComponentInParent<PaperBall>();
                PaperBallManager.instance.paperBallsInHand.ForEach(x => x.onMouse = false);
                var foundBall = PaperBallManager.instance.paperBallsInHand.Find(x => x == paperBall);
                if (foundBall != null)
                {
                    foundBall.onMouse = true;
                }

                if (Input.GetMouseButtonDown(0) && paperBall != null)
                {
                    gameState = GameState.BallCrumpling;

                    paperBall.transform.SetParent(null, true);

                    PaperBallManager.instance.paperBallsInHand.ForEach(x => x.onMouse = false);
                    PaperBallManager.instance.paperBallCount -= 1;
                    PaperBallManager.instance.paperBallsInHand.Remove(paperBall);
                    PaperBallManager.instance.ReSetPaperBall();

                    paperBall.GetReady(this.transform.position);
                }
            }
        }





        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(this.transform.position, new Vector3(0, ThrowDirectionY, 1).normalized * 5f);

        }
    }

    public enum GameState
    {
        Menu,
        BallSelect,
        BallCrumpling,
        BallTrhow,

    }

}