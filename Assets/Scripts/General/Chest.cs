using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    private SpriteRenderer spriteRenderer;
    public Sprite openSprite;
    public Sprite closeSprite;
    public GameObject whatInChest;
    public float upForce;
    public bool isDone;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable() {
        spriteRenderer.sprite = isDone? openSprite: closeSprite;
    }

    public void TriggerAction()
    {
        // Debug.Log("Open Chest!");
        if(!isDone){
            OpenChest();
            GetComponent<AudioDefination>()?.PlayAudioClip();
            whatInChest.SetActive(true);
            whatInChest.transform.position = gameObject.transform.position + new Vector3(0.0f, 0.5f, 0.0f);
            whatInChest.GetComponent<Rigidbody2D>()?.AddForce(transform.up * upForce, ForceMode2D.Impulse);
        }
    }

    private void OpenChest(){
        spriteRenderer.sprite = openSprite;
        isDone = true;
        this.gameObject.tag = "Untagged";
    }
}
