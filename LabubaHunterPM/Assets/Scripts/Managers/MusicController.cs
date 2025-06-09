using DatabaseSystem.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    [Header("Проигрыватель музыки")]
    [SerializeField] private AudioSource _audioSource;
    [Header("Музыка в игре")]
    [Tooltip("Музыка в лобби")]
    [SerializeField] private AudioClip _musicLobby;
    [Tooltip("Музыка в геймплее")]
    [SerializeField] private AudioClip _musicGameplay;

    private static MusicController _instance;
    public static MusicController Instance { get => _instance; set => _instance = value; }

    private void Awake()
    {
        //if (_instance == null)
        //{
            _instance = this;
            //DontDestroyOnLoad(this);
        //}
        //else
            //Destroy(gameObject);
    }

    private void SetAudioSource(AudioClip audioClip)
    {
        if (audioClip != null)
        {
            _audioSource.clip = audioClip;
        }
    }

    public void PlayMusicLobby()
    {
        if (_audioSource != null)
        {
            SetAudioSource(_musicLobby);
            _audioSource.Play();
        }           
    }

    public void PlayMusicGameplay()
    {
        if (_audioSource != null)
        {
            SetAudioSource(_musicGameplay);
            _audioSource.Play();
        }
    }
}
