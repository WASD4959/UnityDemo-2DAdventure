using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{   
    private CapsuleCollider2D coll;
    private PlayerController playerController;
    private Rigidbody2D rb;
    [Header("检测参数")]
    public bool manual;
    public bool isPlayer;
    public Vector2 bottomOffset;
    public Vector2 leftOffset;
    public Vector2 rightOffset;
    public float checkRadius;
    public LayerMask groundlayer;
    [Header("状态")]
    public bool isGround;
    public bool touchLeftWall;
    public bool touchRightWall;
    public bool onWall;

    private void Awake() {
         coll = GetComponent<CapsuleCollider2D>();
         rb = GetComponent<Rigidbody2D>();

         if(!manual) {
            updateOffset();
         }

         if(isPlayer)
            playerController = GetComponent<PlayerController>();
    }

    private void Update() {
        Check();
    }

    public void Check() {
        //检测地面
        if(onWall)
            isGround = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(bottomOffset.x * transform.localScale.x, bottomOffset.y), checkRadius, groundlayer);
        else
            isGround = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(bottomOffset.x * transform.localScale.x, 0), checkRadius, groundlayer);
        
        //墙体判断
        touchLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, checkRadius, groundlayer);
        touchRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, checkRadius, groundlayer);
    
        //在墙上
        if(isPlayer)
            onWall = (touchLeftWall && playerController.inputDirection.x < 0.0f || touchRightWall && playerController.inputDirection.x > 0.0f) && rb.velocity.y < 0.0f;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(bottomOffset.x * transform.localScale.x, bottomOffset.y), checkRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, checkRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, checkRadius);
    }

    public void updateOffset() {
        rightOffset = new Vector2(coll.bounds.center.x + coll.bounds.size.x/2.0f - transform.position.x, coll.bounds.center.y  - transform.position.y);
        leftOffset = new Vector2(coll.bounds.center.x - coll.bounds.size.x/2.0f - transform.position.x, coll.bounds.center.y  - transform.position.y);
    }

}
