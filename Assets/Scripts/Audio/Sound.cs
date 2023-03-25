using System;
using UnityEngine;

namespace Menu
{
    [Serializable]
    public class Sound
    {
        public AudioClip clip;
        public SoundType soundType;

        [Range(0f, 1f)]
        public float volume;
        public float pitchMin;
        public float pitchMax;

        [HideInInspector]
        public AudioSource audioSource;
    }
}