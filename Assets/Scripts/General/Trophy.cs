using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trophy : MonoBehaviour
{
    [Header("广播")]
    public VoidEventSO gameWinEvent;

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")){
            gameWinEvent.RaiseEvent();
        }
    }
}