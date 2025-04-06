using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
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
        public RectTransform windPanel;

        public ParticleSystem windEffect;
        private float particleSpeed = 1;
        public EventReference ambient;
        private EventInstance ambientInstance;

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
            
            

            
            
        }

        public void InitializeFan()
        {
            orignalBallCounts = PaperBallManager.instance.paperBallCount;
            stageStep = Mathf.Abs(orignalBallCounts / (states.Count -1));
            stage = 1;
            currentStage = 0;
            ambientInstance = AudioManager.instance.CreateInstance(ambient, this.transform.position);
            ambientInstance.start();
            if (autoFanControl)
            {
                rotationspeed = states[0].speed;
                this.transform.eulerAngles = new Vector3(0, states[0].angle, 0);
            }
            else
            {
                // rotationspeed = UIManager.instance.userFanState.speed;
                // this.transform.eulerAngles = new Vector3(0, UIManager.instance.userFanState.angle, 0);
                SetFan(UIManager.instance.userFanState);
            }
            FanController.instance.windEffect.gameObject.SetActive(true);
            FanController.instance.CalculateWind();
        }

        void Update()
        {
            flabellum.Rotate(Vector3.forward * rotationspeed * Time.deltaTime, Space.Self);
            HandleAutoControl();
            
        }

        public void SetFanSpeed(float speed)
        {
            this.rotationspeed = speed;
            particleSpeed = speed / 540;
            this.windPower = speed / 300f;
        }

        public void SetFanDir(float angle)
        {
            this.transform.eulerAngles = new Vector3(0f, angle, 0f);
        }

        public void CalculateWind()//这里只取x轴的风
        {
            SetFanSpeed(rotationspeed);
            windEffect.playbackSpeed = particleSpeed;
            float angle = (this.transform.eulerAngles.y > 180)? this.transform.eulerAngles.y - 360f : this.transform.eulerAngles.y;
            ambientInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(flabellum.position));
            
            float dirOffset = - angle / 60 * 1.2f;
            windDir = -Vector3.right * windPower * dirOffset;
            windPowerText.text = Mathf.Round((windPower * dirOffset) * 100f) / 100f + "m/s";
            if (windPower * dirOffset < 1)
            {
                windPanel.localScale = new Vector3(1, 1, 1);
                windPanel.anchoredPosition = new Vector2(0f, windPanel.anchoredPosition.y);
            }
            else
            {
                windPanel.localScale = new Vector3(-1, 1, 1);
                windPanel.anchoredPosition = new Vector2(-0.03f, windPanel.anchoredPosition.y);
            }

            

        }

        public void HandleAutoControl()
        {
            if (PaperBallManager.instance.paperBallsInHand.Count == orignalBallCounts - stageStep * stage
            && currentStage != stage)
            {
                if (autoFanControl)
                    SetFan(states[currentStage]);
                currentStage = stage;
                stage++;

            }
        }

        public void SetFan(FanState state)
        {
            if (state.speed == 0)
            {
                windEffect.gameObject.SetActive(false);
            }
            else
            {
                windEffect.gameObject.SetActive(true);
            }
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
            seq.Join( 
                DOTween.To(() => this.rotationspeed / 600f,
                    x => ambientInstance.setVolume(x),
                    state.speed/600, 1.5f
                    
                    )
                
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