using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("---- Audio Sources ----")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("---- Music Clips ----")]
    public AudioClip musicMainMenu;
    public AudioClip musicInGame;

    [Header("---- SFX Clips ----")]
    public AudioClip sfxClick;
    public AudioClip sfxWalk;
    public AudioClip sfxBook;
    public AudioClip sfxGhost;
    public AudioClip sfxTing;
    public AudioClip sfxAngkat;
    public AudioClip sfxTaruh;
    public AudioClip sfxMemberiPesanan;
    public AudioClip sfxMenang;
    public AudioClip sfxMengaduk;
    public AudioClip sfxMenggoreng;
    public AudioClip sfxMenkukus;
    public AudioClip sfxMerebus;

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
    }

    // ==========================================
    // FUNGSI UNTUK MEMUTAR MUSIC (BGM)
    // ==========================================

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;

        // Jangan putar ulang jika musik yang sama sedang dimainkan
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.loop = true; // BGM biasanya berulang
        musicSource.Play();
    }

    // Fungsi khusus untuk dipanggil di Scene berbeda (Bisa dipanggil dari Button OnClick)
    public void PlayMainMenuMusic() => PlayMusic(musicMainMenu);
    public void PlayInGameMusic()   => PlayMusic(musicInGame);

    public void StopMusic()
    {
        musicSource.Stop();
    }

    // ==========================================
    // FUNGSI UNTUK MEMUTAR SFX
    // ==========================================

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    // Fungsi khusus SFX agar bisa dipanggil langsung dari UI Button OnClick()
    public void PlaySFXClick() => PlaySFX(sfxClick);
    public void PlaySFXWalk()  => PlaySFX(sfxWalk);
    public void PlaySFXBook()  => PlaySFX(sfxBook);
    public void PlaySFXGhost() => PlaySFX(sfxGhost);

    public void PlaySFXTing()           => PlaySFX(sfxTing);
    public void PlaySFXAngkat()         => PlaySFX(sfxAngkat);
    public void PlaySFXTaruh()          => PlaySFX(sfxTaruh);
    public void PlaySFXMemberiPesanan() => PlaySFX(sfxMemberiPesanan);
    public void PlaySFXMenang()         => PlaySFX(sfxMenang);
    public void PlaySFXMengaduk()       => PlaySFX(sfxMengaduk);
    public void PlaySFXMenggoreng()     => PlaySFX(sfxMenggoreng);
    public void PlaySFXMenkukus()       => PlaySFX(sfxMenkukus);
    public void PlaySFXMerebus()        => PlaySFX(sfxMerebus);


    // ==========================================
    // KONTROL VOLUME & TOGGLE
    // ==========================================

    public void ToggleBGM(bool isOn)
    {
        if (musicSource != null) musicSource.mute = !isOn;
    }

    public void ToggleSFX(bool isOn)
    {
        if (sfxSource != null) sfxSource.mute = !isOn;
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = Mathf.Clamp01(volume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = Mathf.Clamp01(volume);
    }

     public void StopSFXWalk()
    {
        if (sfxSource.isPlaying)
        {
            sfxSource.Stop();
        }
    }
}