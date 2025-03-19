using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioMixer AudioMixer;

    public AudioSource BGMAudioSource;
    public List<AudioSource> SFXAudioSources;

    public string BGMVol = "BGMVol";
    public string SFXVol = "SFXVol";

    private int _SFXAudioSourceIndex;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
    }

    public float GetVolumeSliderValue(string volumeParameter)
    {
        if (AudioMixer.GetFloat(volumeParameter, out float currentVolume))
        {
            return Mathf.Pow(10, currentVolume / 20); // Convert dB to linear (0-1)
        }
        return 1f; // Default to full volume if not found
    }

    public void OnVolumeSliderChanged(string volumeParameter, float sliderValue)
    {
        float volume = Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20; // Prevent log errors
        AudioMixer.SetFloat(volumeParameter, volume);
    }

    internal void PlayMusic(AudioClip music)
    {
        AudioSource audioSource = BGMAudioSource;
        audioSource.clip = music;
        audioSource.loop = true;
        audioSource.Play();
    }

    internal void StopMusic()
    {
        BGMAudioSource.Stop();
    }

    internal void PlaySound(AudioClip audioClip)
    {
        if (audioClip  == null) { return; }

        _SFXAudioSourceIndex++;
        _SFXAudioSourceIndex %= SFXAudioSources.Count;
        AudioSource audioSource = SFXAudioSources[_SFXAudioSourceIndex];
        audioSource.clip = audioClip;
        audioSource.loop = false;
        audioSource.Play();
    }
}
