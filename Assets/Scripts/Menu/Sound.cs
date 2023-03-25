using System;
using UnityEngine;

namespace Menu
{
    [Serializable]
    public class Sound
    {
        public AudioClip clip;
        public MusicType musicType;

        [Range(0f, 1f)]
        public float volume;
        [Range(0f, 1f)]
        public float pitch;

        [HideInInspector]
        public AudioSource audioSource;
    }
}