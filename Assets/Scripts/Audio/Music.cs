using System;
using UnityEngine;

namespace Audio
{
    [Serializable]
    public class Music
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