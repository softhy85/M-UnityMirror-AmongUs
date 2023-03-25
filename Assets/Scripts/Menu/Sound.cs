﻿using System;
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
        [Range(0f, 0.5f)]
        public float pitchMin;
        [Range(0.5f, 1f)]
        public float pitchMax;

        [HideInInspector]
        public AudioSource audioSource;
    }
}