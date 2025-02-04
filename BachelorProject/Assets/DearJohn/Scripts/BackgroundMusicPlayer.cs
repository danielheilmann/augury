using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BackgroundMusicPlayer : MonoBehaviour
{
    public static BackgroundMusicPlayer Instance { get; private set; }
    private AudioSource audioSource;
    [SerializeField] private AudioClip backgroundTrack;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        audioSource.clip = backgroundTrack;
    }

    private void OnEnable()
    {
        GameManager.OnGameStart.AddListener(Play);
        GameManager.OnGamePause.AddListener(Pause);
        GameManager.OnGameResume.AddListener(Resume);
    }

    private void OnDisable()
    {
        GameManager.OnGameStart.RemoveListener(Play);
        GameManager.OnGamePause.RemoveListener(Pause);
        GameManager.OnGameResume.RemoveListener(Resume);
    }

    public void Play()
    {
        audioSource.Play();
    }

    public void Pause()
    {
        audioSource.Pause();
    }

    public void Resume()
    {
        // Debug.Log($"Audiosource Time: {audioSource.time} | GameTime: {GameTimeHandler.currentGameTimeInSeconds}");
        if (GameTimeHandler.currentGameTimeInSeconds <= backgroundTrack.length)
            audioSource.time = GameTimeHandler.currentGameTimeInSeconds;

        audioSource.pitch = ReplayManager.Instance?.timeline?.speedMultiplier ?? 1f;
        audioSource.UnPause();
    }

    public void Stop()
    {
        audioSource.Stop();
    }
}
