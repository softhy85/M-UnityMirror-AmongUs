using System;
using System.Collections;
using System.Collections.Generic;
using Player.Information;
using UnityEngine;
using UnityEngine.Audio;

namespace Menu
{
    public class AudioManager : MonoBehaviour
    {
        private AudioManager instance;

        [SerializeField] private Sound[] sounds;

        [SerializeField] private AudioMixerGroup  audioMixerGroup ;
        private MusicType actualMusic = MusicType.NoMusic;

        public MusicType GetActualMusic()
        {
            return actualMusic;
        }

        private void Awake()
        {
            if (instance)
            {
                Destroy(gameObject);
                return;
            }

            foreach (var sound in sounds)
            {
                sound.audioSource = gameObject.AddComponent<AudioSource>();
                sound.audioSource.clip = sound.clip;
                sound.audioSource.playOnAwake = false;
                sound.audioSource.loop = true;
                sound.audioSource.pitch = sound.pitch;
                sound.audioSource.outputAudioMixerGroup =
                    audioMixerGroup;
                sound.audioSource.volume = sound.volume;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void StartMusic(MusicType musicType)
        {
            if (actualMusic != MusicType.NoMusic)
            {
                StopMusic();
            }
            foreach (var sound in sounds)
            {
                if (sound.musicType == musicType)
                {
                    sound.audioSource.Play();
                    actualMusic = musicType;
                }
            }
        }

        public void StopMusic()
        {

            foreach (var sound in sounds)
            {
                if (sound.musicType == actualMusic)
                {
                    sound.audioSource.Stop();
                    actualMusic = MusicType.NoMusic;
                }
            }
        }
    }
}
