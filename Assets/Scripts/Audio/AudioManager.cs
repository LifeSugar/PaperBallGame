using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace PaperBallGame
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance;


        void Awake()
        {
            if (instance != null && instance != this)
            {
                Debug.LogError("there is more than one AudioManager in the scene!");
                
            }     
        }
    }
}