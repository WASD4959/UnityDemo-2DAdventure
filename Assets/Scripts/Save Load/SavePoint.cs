using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour, IInteractable
{
    [Header("事件广播")]
    public VoidEventSO LoadGameEvent;

    [Header("变量参数")]
    public SpriteRenderer spriteRenderer;
    public Sprite darkSprite;
    public Sprite lightSprite;
    public GameObject lightObj;
    public bool isDone;

    private void OnEnable() {
        spriteRenderer.sprite = isDone? lightSprite: darkSprite;
        lightObj.SetActive(isDone);
    }

    public void TriggerAction()
    {
        if(!isDone){
            isDone = true;
            spriteRenderer.sprite = lightSprite;
            lightObj.SetActive(true);
            

            //TODO:保存数据
            LoadGameEvent.RaiseEvent();

            this.gameObject.tag = "Untagged";
        }
    }


}
