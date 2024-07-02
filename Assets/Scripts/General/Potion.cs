using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
    public float healValue;
    private PhysicsCheck physicsCheck;

    private void Awake() {
        physicsCheck = GetComponent<PhysicsCheck>();
    }

    private void OnTriggerStay2D(Collider2D other) {
        if(other.CompareTag("Player") && physicsCheck.isGround){
            Character character = other.GetComponent<Character>();
            if(character.currentHealth + healValue > character.maxHealth){
                character.currentHealth = character.maxHealth;
            }
            else{
                character.currentHealth += healValue;
            }
            //更新UI
            character.OnHealthChange?.Invoke(character);
            GetComponent<AudioDefination>()?.PlayAudioClip();
            gameObject.SetActive(false);
        }
    }
}
