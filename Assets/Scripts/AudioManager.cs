using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip spawnSound;
    public AudioClip ballPickupSound;
    public AudioClip winSound;
    public AudioClip loseSound;

    [Header("Audio Settings")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float bgmVolume = 0.5f;
    [Range(0f, 1f)] public float sfxVolume = 0.7f;

    private Dictionary<AudioClip, float> lastPlayTimes = new Dictionary<AudioClip, float>();
    private const float MIN_PLAY_INTERVAL = 0.1f;
    private Queue<AudioSource> sfxPool;
    private const int POOL_SIZE = 5;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioPool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeAudioSources();
    }

    void InitializeAudioSources()
    {
        if (bgmSource != null)
        {
            bgmSource.loop = true;
            UpdateBGMVolume();
        }

        if (sfxSource != null)
        {
            UpdateSFXVolume();
        }
    }

    void InitializeAudioPool()
    {
        sfxPool = new Queue<AudioSource>();
        for (int i = 0; i < POOL_SIZE; i++)
        {
            CreatePooledAudioSource();
        }
    }

    void CreatePooledAudioSource()
    {
        GameObject obj = new GameObject("PooledAudio");
        obj.transform.parent = transform;
        AudioSource source = obj.AddComponent<AudioSource>();
        source.playOnAwake = false;
        sfxPool.Enqueue(source);
    }

    public void PlaySound(AudioClip clip, Vector3 position = default, float volume = 1f)
    {
        if (clip == null) return;

        if (lastPlayTimes.ContainsKey(clip))
        {
            if (Time.time - lastPlayTimes[clip] < MIN_PLAY_INTERVAL)
            {
                return;
            }
        }

        AudioSource source = GetAvailableAudioSource();
        if (source != null)
        {
            source.transform.position = position;
            source.clip = clip;
            source.volume = volume * sfxVolume * masterVolume;
            source.Play();
            lastPlayTimes[clip] = Time.time;
        }
    }

    private AudioSource GetAvailableAudioSource()
    {
        foreach (AudioSource source in sfxPool)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }
        CreatePooledAudioSource();
        return sfxPool.Dequeue();
    }

    public void PlaySpawnSound() => PlaySound(spawnSound);
    public void PlayBallPickupSound() => PlaySound(ballPickupSound);
    public void PlayWinSound() => PlaySound(winSound);
    public void PlayLoseSound() => PlaySound(loseSound);

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        UpdateAllVolumes();
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        UpdateBGMVolume();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        UpdateSFXVolume();
    }

    private void UpdateAllVolumes()
    {
        UpdateBGMVolume();
        UpdateSFXVolume();
    }

    private void UpdateBGMVolume()
    {
        if (bgmSource != null)
        {
            bgmSource.volume = bgmVolume * masterVolume;
        }
    }

    private void UpdateSFXVolume()
    {
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume * masterVolume;
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
} 