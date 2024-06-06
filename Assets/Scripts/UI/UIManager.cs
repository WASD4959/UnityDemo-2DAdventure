using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public PlayerStatBar playerStatBar;

    [Header("事件监听")]
    public CharacterEventSo healthEvent;

    private void OnEnable() {
        healthEvent.OnEventRaised += OnHealthEvent;
    }

    private void OnDisable() {
        healthEvent.OnEventRaised -= OnHealthEvent;
    }

    private void OnHealthEvent(Character character)
    {
        var percentage = character.currentHealth / character.maxHealth;
        playerStatBar.OnHealthChange(percentage);
        playerStatBar.OnPowerChange(character);
    }
}
