using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;

public class Sign : MonoBehaviour
{
    private PlayerInputControl playerInput;
    private Animator anim;
    public Transform playerTrans;
    public GameObject signSprite;
    public bool canPress;
    private IInteractable targetItem;

    private void Awake() {
        // anim = GetComponentInChildren<Animator>();
        anim = signSprite.GetComponent<Animator>();
        playerInput = new PlayerInputControl();
        playerInput.Enable();
    }

    private void OnEnable() {
        InputSystem.onActionChange += OnActionChange;
        playerInput.GamePlay.Confirm.started += OnConfirm;
    }

    private void OnDisable() {
        InputSystem.onActionChange -= OnActionChange;
        playerInput.GamePlay.Confirm.started -= OnConfirm;
    }

    /// <summary>
    /// 切换设备时切换动画
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="actionChange"></param>
    private void OnActionChange(object obj, InputActionChange actionChange)
    {
        if(actionChange == InputActionChange.ActionStarted){
            // Debug.Log(((InputAction)obj).activeControl.device);

            var d = ((InputAction)obj).activeControl.device;

            switch(d.device){
                case Keyboard:
                    anim.Play("keyboard");
                    break;
                case XInputController:
                    anim.Play("xbox");
                    break;
            }
        }
    }

    private void Update() {
        signSprite.GetComponent<SpriteRenderer>().enabled = canPress;
        signSprite.transform.localScale = playerTrans.localScale;
    }

    private void OnConfirm(InputAction.CallbackContext obj)
    {
        if(canPress){
            targetItem.TriggerAction();
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        if(other.CompareTag("Interactable")){
            canPress = true;
            targetItem = other.GetComponent<IInteractable>();
        }
        else
            canPress = false;
    }

    private void OnTriggerExit2D(Collider2D other) {
        canPress = false;
    }
}
