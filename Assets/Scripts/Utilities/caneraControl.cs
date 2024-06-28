using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class caneraControl : MonoBehaviour
{
    private CinemachineConfiner2D confiner2D;
    public CinemachineImpulseSource ImpulseSource;
    
    [Header("事件监听")]
    public VoidEventSO cameraShakeEvent;
    public VoidEventSO SceneLoadedEvent;

    private void Awake() {
        confiner2D = GetComponent<CinemachineConfiner2D>();       
    }

    // private void Start() {
    //     GetNewCameraBounds();
    // }

    private void OnEnable(){
        cameraShakeEvent.OnEventRaised += OnCameraShakeEvent;
        SceneLoadedEvent.OnEventRaised += GetNewCameraBounds;
    }

    private void OnDisable() {
        cameraShakeEvent.OnEventRaised -= OnCameraShakeEvent;
        SceneLoadedEvent.OnEventRaised -= GetNewCameraBounds;
    }

    private void OnCameraShakeEvent()
    {
        ImpulseSource.GenerateImpulse();
    }

    private void GetNewCameraBounds(){
        var obj = GameObject.FindGameObjectWithTag("Bounds");
        if(obj == null)
            return;
        
        confiner2D.m_BoundingShape2D = obj.GetComponent<Collider2D>();

        confiner2D.InvalidateCache();
    }
}
