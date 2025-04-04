using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;

namespace PaperBallGame
{
    public class PaperBall : MonoBehaviour
    {
        public Rigidbody rb;
        public float crumplingFactor;

        private Material VATMat;

        [SerializeField]
        private Collider col;

        public bool onMouse = false; // 鼠标是否在纸团上
        public GameObject outlineObj; // 轮廓物体
        [SerializeField]
        private Vector3 originanPos;

        private Collider selectionCol;

        public bool thrown;
        public bool inAir;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            col = GetComponent<Collider>();
            col.enabled = false; // 禁用碰撞器
            selectionCol = GetComponentInChildren<Collider>(false);
        }

        

        void Update()
        {
            if (InputManager.instance.gameState == GameState.BallSelect && !thrown)
            {
                HandleSelectionState();
            }
        
        }

        void FixedUpdate()
        {
            if (inAir)
            {
                rb.AddForce(FanController.instance.windDir * FanController.instance.windPower, ForceMode.Force);
                Ray ray = new Ray(this.transform.position, new Vector3(0, -1, 0).normalized);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 0.1f))
                {
                    if (hit.collider != null)
                    {
                        inAir = false;
                    }

                }
               
            }
            
        }

        void ODrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(this.transform.position, new Vector3(0, 1, 0).normalized * 0.05f);
        }

        private Tween currentTween;

        private void HandleSelectionState()
        {
            if (onMouse)
            {
                outlineObj.SetActive(true); // 显示轮廓
                if (currentTween != null)
                {
                    currentTween.Kill();
                }

                // currentTween = this.transform.DOLocalMove(originanPos - 0.1f * transform.InverseTransformDirection(transform.up), 0.35f);
                currentTween = this.transform.DOMove(originanPos - 0.1f * (transform.up), 0.35f);
                
            }
            else
            {
                outlineObj.SetActive(false); // 隐藏轮廓
                if (currentTween != null)
                {
                    currentTween.Kill();
                }
                currentTween = this.transform.DOMove(originanPos, 0.15f);
            }
        }

        public void SetOriginanPos(Vector3 pos)
        {
            originanPos = pos;
        }

        public void GetReady(Vector3 pos)
        {
            currentTween.Kill();
            this.outlineObj.SetActive(false);
            selectionCol.enabled = false;
            
            DG.Tweening.Sequence seq = DOTween.Sequence();

            Renderer renderer = this.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                renderer.material = new Material (renderer.material);
            }

            seq.Append(
                this.transform.DOMove(pos, 0.25f).SetEase(Ease.OutQuad)
            );

            seq.Join(
                DOTween.To(() => this.transform.eulerAngles.y,
                x => {
                    this.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x, this.transform.eulerAngles.y, x);
                }, 180.0f, 0.25f)
            );

            seq.AppendInterval(0.3f);

            seq.Append(
                DOTween.To(() => crumplingFactor,
                x =>{
                    renderer.material.SetFloat( "_frame", x);
                    crumplingFactor = x;
                }, 99f, 1.0f)

            ).OnComplete(() => {
                col.enabled = true;
                InputManager.instance.activePaperBall = this;
                InputManager.instance.gameState = GameState.BallTrhow;

            });
        }

        


    
    }
}