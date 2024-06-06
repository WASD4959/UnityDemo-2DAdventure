using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatBar : MonoBehaviour
{
    private Character currentCharacter;
    public Image healthImage;
    public Image healthDelayImage;
    public Image powerImage;

    private bool isRecovering;

    private void Update() {
        if(healthDelayImage.fillAmount > healthImage.fillAmount)
            healthDelayImage.fillAmount -= Time.deltaTime;

        if(isRecovering){
            float percentage = currentCharacter.currentPower / currentCharacter.maxPower;
            powerImage.fillAmount = percentage;

            if(percentage >= 1){
                isRecovering = false;
                return;
            }
        }
    }


    /// <summary>
    /// 接受当前Health百分比
    /// </summary>
    /// <param name="percentage"> currentHealth/maxHealth </param>
    public void OnHealthChange(float percentage){
        healthImage.fillAmount = percentage;
    }

    public void OnPowerChange(Character character){
        isRecovering = true;
        currentCharacter = character;
    }
}
