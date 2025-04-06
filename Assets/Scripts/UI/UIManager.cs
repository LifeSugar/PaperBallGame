using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Rendering;

namespace PaperBallGame
{
    public class UIManager : MonoBehaviour
    {
        public Button startButton;
        public Button RetryButton;
        public Button PauseButton;
        
        [Header("Pause Panel")]
        public GameObject pausePanel;
        public Slider musicSlider;
        public Slider sfxSlider;

        public Toggle autoFan;
        public Slider fanSpeedSlider;
        public Slider fanDirSlider;
        public FanState userFanState;
        public Button closePanelButton;
        
        
        private Tween scaleTween;
        public GameObject scorePanel;
        
        public static UIManager instance;

        void Awake()
        {
            instance = this;
        }
        

        void Start()
        {
            scorePanel.SetActive(false);
            StartPulse(startButton.gameObject);
            startButton.onClick.AddListener(() => OnStartClicked(startButton.gameObject));
            RetryButton.onClick.AddListener(() => OnRetryClicked(RetryButton.gameObject));
            PauseButton.onClick.AddListener(() => OnPauseClicked());
            closePanelButton.onClick.AddListener(() => OnClosePanelClicked());
            
            AudioManager.instance.bgmVolume = musicSlider.value;
            AudioManager.instance.sfxVolume = sfxSlider.value;
            musicSlider.onValueChanged.AddListener((float x) =>
            {
                AudioManager.instance.bgmVolume = x;
            });
            sfxSlider.onValueChanged.AddListener((float x) =>
            {
                AudioManager.instance.sfxVolume = x;
            });

            autoFan.isOn = FanController.instance.autoFanControl;
            autoFan.onValueChanged.AddListener((bool b) =>
            {
                FanController.instance.autoFanControl = b;
            });
            fanSpeedSlider.value = 0;
            fanDirSlider.value = 0.5f;
            userFanState = SetUserFanState();
            fanSpeedSlider.onValueChanged.AddListener((b) =>
            {
                fanSpeedSlider.value = b;
                userFanState  = SetUserFanState();
            } );
            
            fanDirSlider.onValueChanged.AddListener((b) =>
            {
                userFanState  = SetUserFanState();
            });
        }

        public void GetRedayRestart()
        {
            RetryButton.gameObject.SetActive(true);
            StartPulse(RetryButton.gameObject);
        }

        void StartPulse(GameObject button)
        {
            scaleTween?.Kill();
            
            scaleTween = button.transform.DOScale(1.1f, 0.9f).SetEase(Ease.OutSine).SetLoops(-1, LoopType.Yoyo);
            
        }

        void StopPulse(GameObject button)
        {
            scaleTween?.Kill();
            button.transform.DOScale(1, 0.9f).SetEase(Ease.OutSine);
        }
        
        void OnStartClicked(GameObject button)
        {
            PaperBallManager.instance.SetBallsTranform();
            StopPulse(button);
            startButton.gameObject.SetActive(false);
            scorePanel.SetActive(true);
            FanController.instance.InitializeFan();
        }

        void OnRetryClicked(GameObject button)
        {
            ScoreManager.instance.currentScore = 0;
            ScoreManager.instance.totalScores = 0;
            ScoreManager.instance.RefreshScoreUI();
            PaperBallManager.instance.SetBallsTranform();
            StopPulse(button);
            RetryButton.gameObject.SetActive(false);
            FanController.instance.InitializeFan();
        }

        private GameState previousGameState;
        void OnPauseClicked()
        {
            previousGameState = InputManager.instance.gameState;
            pausePanel.SetActive(true);
            InputManager.instance.gameState = GameState.Menu;
        }

        void OnClosePanelClicked()
        {
            pausePanel.SetActive(false);
            if (previousGameState == GameState.BallCrumpling)
            {
                previousGameState = GameState.BallTrhow;
            }
            InputManager.instance.gameState = previousGameState;
            if (!FanController.instance.autoFanControl)
            {
                FanController.instance.SetFan(userFanState);
            }
            else
            {
                FanController.instance.SetFan(FanController.instance.states[FanController.instance.currentStage]);
            }
        }

        public FanState SetUserFanState()
        {
            FanState newState;
            newState.speed = fanSpeedSlider.value * 600f;
            newState.angle = 60 - fanDirSlider.value * 120;
            return newState;
        }
    }

}