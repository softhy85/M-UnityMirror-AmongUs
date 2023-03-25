using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager instance;

        [SerializeField] private Music[] musics;

        [SerializeField] private Sound[] sounds;

        [SerializeField] private AudioMixerGroup  audioMixerGroup ;

        private MusicType actualMusic = MusicType.NoMusic;
        private SoundType lastSound = SoundType.NoSound;

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

            InitializeMusic();
            InitializeSound();
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        #region Music

        private void InitializeMusic()
        {
            foreach (var music in musics)
            {
                music.audioSource = gameObject.AddComponent<AudioSource>();
                music.audioSource.clip = music.clip;
                music.audioSource.playOnAwake = false;
                music.audioSource.loop = true;
                music.audioSource.pitch = music.pitch;
                music.audioSource.outputAudioMixerGroup =
                    audioMixerGroup;
                music.audioSource.volume = music.volume;
            }
        }

        public void StartMusic(MusicType musicType)
        {
            if (actualMusic != MusicType.NoMusic)
            {
                StopMusic();
            }
            foreach (var music in musics)
            {
                if (music.musicType == musicType && music.audioSource)
                {
                    music.audioSource.Play();
                    actualMusic = musicType;
                }
            }
        }

        public void StopMusic()
        {
            foreach (var music in musics)
            {
                if (music.musicType == actualMusic && music.audioSource)
                {
                    music.audioSource.Stop();
                    actualMusic = MusicType.NoMusic;
                }
            }
        }

        #endregion

        #region Sound

        private void InitializeSound()
        {
            foreach (var sound in sounds)
            {
                sound.audioSource = gameObject.AddComponent<AudioSource>();
                sound.audioSource.clip = sound.clip;
                sound.audioSource.playOnAwake = false;
                sound.audioSource.loop = false;
                sound.audioSource.outputAudioMixerGroup =
                    audioMixerGroup;
                sound.audioSource.volume = sound.volume;
            }
        }
        public void StartSound(SoundType soundType)
        {
            foreach (var sound in sounds)
            {
                if (sound.soundType == soundType && sound.audioSource)
                {
                    if (!sound.audioSource.isPlaying) {
                        sound.audioSource.pitch =
                            UnityEngine.Random.Range(sound.pitchMin, sound.pitchMax);
                        sound.audioSource.Play();
                        lastSound = soundType;
                    }
                }
            }
        }

        #endregion
    }
}
