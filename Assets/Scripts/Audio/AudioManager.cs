using System.Collections;
using System.Collections.Generic;
using System.Threading;
using FMOD.Studio;
using UnityEngine;
using FMODUnity;

namespace PaperBallGame
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance;

        public EventReference bgm;
        
        [Header("volume")]
        [Range(0, 1)] public float bgmVolume = 1 ;
        [Range(0, 1)] public float sfxVolume = 1;

        private Bus bgmBus;
        private Bus sfxBus;
        


        void Awake()
        {
            if (instance != null && instance != this)
            {
                Debug.LogError("there is more than one AudioManager in the scene!");
            }
            instance = this;
            
            bgmBus = RuntimeManager.GetBus("bus:/BGM");
            sfxBus = RuntimeManager.GetBus("bus:/SFX");
        }

        void Start()
        {
            EventInstance ins = CreateInstance(bgm, Camera.main.transform.position);
            ins.start();
        }

        void Update()
        {
            if (InputManager.instance.gameState == GameState.Menu)
            {
                bgmBus.setVolume(bgmVolume);
                sfxBus.setVolume(sfxVolume); 
            }
        }

        public void PlayOneShot(EventReference soundEvent, Vector3 WorldPos)
        {
            RuntimeManager.PlayOneShot(soundEvent, WorldPos);
        }

        public void PlayOneShot(EventReference soundEvent, Vector3 worldPos, float volume)
        {
            if (volume <= 0f) return;

            FMOD.Studio.EventInstance instance = RuntimeManager.CreateInstance(soundEvent);

            instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(worldPos));

            instance.setVolume(volume);

            instance.start();
            instance.release();
        }

        public EventInstance CreateInstance(EventReference soundEvent, Vector3 WorldPos)
        {
            EventInstance instance = RuntimeManager.CreateInstance(soundEvent);
            instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(WorldPos));
            return instance;
        }

        // public void SetInstance(EventInstance instance, Vector3 WorldPos, float volume)
        // {
        //     instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(WorldPos));
        //     instance.setVolume(volume);
        // }


    }
}