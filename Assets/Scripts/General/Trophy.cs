using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trophy : MonoBehaviour
{
    [Header("广播")]
    public VoidEventSO gameWinEvent;

    public float amplitude = 0.5f;
    public float frequency = 0.5f;

    private Vector3 startPos;

    private void Start() {
        startPos = transform.position;
    }

    private void Update() {
        float newY = startPos.y + amplitude * Mathf.Sin(Time.time * frequency);
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")){
            gameWinEvent.RaiseEvent();
            GetComponent<AudioDefination>()?.PlayAudioClip(); 
        }
    }
}