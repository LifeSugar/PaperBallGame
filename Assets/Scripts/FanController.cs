using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

namespace PaperBallGame
{
    public class FanController : MonoBehaviour
    {
        public Transform flabellum;

        public float rotationspeed = 1;

        public static FanController instance;
        public Vector3 windDir;
        public float windPower;
        public TextMeshProUGUI windPowerText;

        public bool autoFanControl;
        public int stage = 1;
        public int currentStage = 0;
        public List<FanState> states = new List<FanState>();

        void Awake()
        {
            if (instance != null)
            {
                Debug.LogError("there are more than one fancontraller in scene!");
            }
            instance = this;
        }

        public int stageStep;
        public int orignalBallCounts;

        void Start()
        {
            orignalBallCounts = PaperBallManager.instance.paperBallCount;
            stageStep = Mathf.Abs(orignalBallCounts / (states.Count -1));
            rotationspeed = states[0].speed;
            this.transform.eulerAngles = new Vector3(0, states[0].angle, 0);
            
            CalculateWind();
        }

        void Update()
        {
            flabellum.Rotate(Vector3.forward * rotationspeed * Time.deltaTime, Space.Self);
            if (autoFanControl)
            {
                HandleAutoControl();
            }
        }

        public void SetFanSpeed(float speed)
        {
            this.rotationspeed = speed;
            this.windPower = speed / 300f;
        }

        public void SetFanDir(float angle)
        {
            this.transform.eulerAngles = new Vector3(0f, angle, 0f);
        }

        public void CalculateWind()//这里只取x轴的风
        {
            SetFanSpeed(rotationspeed);
            float angle = (this.transform.eulerAngles.y > 180)? this.transform.eulerAngles.y - 360f : this.transform.eulerAngles.y;
            
            float dirOffset = - angle / 60 * 1.2f;
            windDir = -Vector3.right * windPower * dirOffset;
            windPowerText.text =Mathf.Round((windPower * dirOffset) * 100f) / 100f + "m/s";

            

        }

        public void HandleAutoControl()
        {
            if (PaperBallManager.instance.paperBallsInHand.Count == orignalBallCounts - stageStep * stage
            && currentStage != stage)
            {
                SetFan(states[currentStage]);
                currentStage = stage;
                stage++;

            }
        }

        private void SetFan(FanState state)
        {
            DG.Tweening.Sequence seq = DOTween.Sequence();

            seq.Join(
                this.transform.DORotate(new Vector3(0, state.angle, 0), 1.5f)
            );
            seq.Join(
                DOTween.To(() => this.rotationspeed,
                x =>
                {
                    SetFanSpeed(x);
                }, state.speed, 1.5f)
            );

            seq.OnUpdate(() =>
            {
                CalculateWind();
            });
            
        }
    }

    [Serializable]
    public struct FanState
    {
        public float speed;
        public float angle;

    }


}