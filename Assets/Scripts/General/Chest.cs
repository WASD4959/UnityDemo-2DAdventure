using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    private SpriteRenderer spriteRenderer;
    public Sprite openSprite;
    public Sprite closeSprite;
    public bool isDone;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable() {
        spriteRenderer.sprite = isDone? openSprite: closeSprite;
    }

    public void TriggerAction()
    {
        Debug.Log("Open Chest!");
        if(!isDone){
            OpenChest();
            GetComponent<AudioDefination>()?.PlayAudioClip();
        }
    }

    private void OpenChest(){
        spriteRenderer.sprite = openSprite;
        isDone = true;
        this.gameObject.tag = "Untagged";
    }
}
