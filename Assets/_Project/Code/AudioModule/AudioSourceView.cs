using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    public class AudioSourceView : MonoBehaviour
    {
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _soundSource;
        [SerializeField] private AudioSource _envSoundSource;
        [SerializeField] private AudioMixer _audioMixer;
        
        public AudioSource MusicSource => _musicSource;
        public AudioSource SoundSource => _soundSource;
        public AudioSource EnvSoundSource => _envSoundSource;
        public AudioMixer AudioMixer => _audioMixer;
    }
}

