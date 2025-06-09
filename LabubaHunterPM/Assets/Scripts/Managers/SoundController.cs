using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    [Header("Проигрыватель звука")]
    [SerializeField] private AudioSource _audioSource;
    [Header("Звук нажатия на кнопку")]
    [SerializeField] private AudioClip _soundClick;
    [Header("Звук прокачки или разблокировки героя")]
    [SerializeField] private AudioClip _soundLevelUp;

    private static SoundController _instance;
    public static SoundController Instance { get => _instance; set => _instance = value; }

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

    public void PlayClick()
    {
        if (_audioSource != null)
        {
            SetAudioSource(_soundClick);
            _audioSource.Play();
        }
    }

    public void PlayLevelUp()
    {
        if (_audioSource != null)
        {
            SetAudioSource(_soundLevelUp);
            _audioSource.Play();
        }
    }
}
