using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Singleton Instance
    public static AudioManager instance;

    [Header("---- Audio Sources ----")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("---- Music ----")]
    public Sound musicSounds;
     [Header("---- SFX ----")]
    public AudioClip sfxClick;
    public AudioClip sfxBook;
    public AudioClip sfxGhost;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        bool bgmOn = PlayerPrefs.GetInt("BGM", 1) == 1;
        bool sfxOn = PlayerPrefs.GetInt("SFX", 1) == 1;

        ToggleBGM(bgmOn);
        ToggleSFX(sfxOn);

        PlayMusic();
    }

    public void ToggleBGM(bool isOn)
    {
        if(musicSource != null)
        {
            musicSource.mute = !isOn;
        }
    }

    public void ToggleSFX(bool isOn)
    {
        if(sfxSource != null)
        {
            sfxSource.mute = !isOn;
        }
    }

    // ==========================================
    // FUNGSI UNTUK MEMUTAR AUDIO
    // ==========================================

    public void PlayMusic()
    {
        Sound s = musicSounds;

        if (s == null)
        {
            Debug.LogWarning("Music tidak ditemukan: " + name);
            return;
        }

        musicSource.clip = s.clip;
        musicSource.volume = s.volume;
        musicSource.pitch = s.pitch;
        musicSource.loop = s.loop;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip, volume);
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    // ==========================================
    // FUNGSI UNTUK KONTROL VOLUME (Cocok untuk Menu Settings)
    // ==========================================

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = Mathf.Clamp01(volume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = Mathf.Clamp01(volume);
    }
}

// Class penampung data audio agar rapi di Inspector
[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.1f, 3f)] public float pitch = 1f;
    public bool loop;
}