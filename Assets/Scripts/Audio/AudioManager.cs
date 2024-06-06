using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("事件监听")]
    public PlayAudioEventSO FXEvent;
    public PlayAudioEventSO BGMEvent;

    [Header("组件")]
    public AudioSource BGMSource;
    public AudioSource FXSource;

    private void OnEnable() {
        FXEvent.onEventRaised += OnFXEvent;
        BGMEvent.onEventRaised += onBGMEvent;
    }

    private void OnDisable() {
        FXEvent.onEventRaised -= OnFXEvent;
        BGMEvent.onEventRaised -= onBGMEvent;
    }

    private void onBGMEvent(AudioClip audioClip)
    {
        BGMSource.clip = audioClip;
        BGMSource.Play();
    }

    private void OnFXEvent(AudioClip audioClip)
    {
        FXSource.clip = audioClip;
        FXSource.Play();
    }
}
