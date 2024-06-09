using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.Mathematics;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("监听事件")]
    public SceneLoadEventSO loadEvent;
    public VoidEventSO SceneLoadedEvent;

    public PlayerInputControl inputControl;
    public Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private PhysicsCheck physicsCheck;
    private CapsuleCollider2D capsuleCollider;
    private PlayerAnimation playerAnimation;
    private Character character;

    private Vector2 originalOffset;
    private Vector2 originalSize;

    public Vector2 inputDirection;

    [Header("基本参数")]
    public float speed;
    private float runSpeed;
    private float walkSpeed;
    public float jumpForce;
    public float wallJumpForce;
    public float hurtForce;
    public float slideDistance;
    public float slideSpeed;
    public float slideTime;
    public float slideTimeCounter;
    public int slidePowerCost;
    
    [Header("物理材质")]
    public PhysicsMaterial2D normal;
    public PhysicsMaterial2D wall;

    [Header("基本状态")]
    public bool isCrouch;
    public bool isHurt;
    public bool isDead;
    public bool isAttack;
    public bool wallJump;
    public bool isSlide;

    private void Awake() {
        inputControl = new PlayerInputControl();

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        physicsCheck = GetComponent<PhysicsCheck>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        playerAnimation = GetComponent<PlayerAnimation>();
        character = GetComponent<Character>();

        originalOffset = capsuleCollider.offset;
        originalSize = capsuleCollider.size;
        
        // jump
        inputControl.GamePlay.Jump.started += Jump;

        // force to walk
        runSpeed = speed;
        walkSpeed = speed / 2.5f;
        inputControl.GamePlay.WalkButton.performed += ctx => {
            if(physicsCheck.isGround)
                speed = walkSpeed;
        };

        inputControl.GamePlay.WalkButton.canceled += ctx => {
                speed = runSpeed;
        };

        //attack
        inputControl.GamePlay.Attack.started += Attack;

        //slide
        inputControl.GamePlay.Slide.started += Slide;
    }

    private void OnEnable() {
        inputControl.Enable();
        // //加载场景时对玩家输入的控制；
        // loadEvent.LoadRequestEvent += OnLoadEvent;
        // SceneLoadedEvent.OnEventRaised += OnSceneLoadedEvent;
    }

    private void OnDisable() {
        inputControl.Disable();
        loadEvent.LoadRequestEvent -= OnLoadEvent;
        SceneLoadedEvent.OnEventRaised -= OnSceneLoadedEvent;
    }

    private void Update() {
        inputDirection = inputControl.GamePlay.Move.ReadValue<Vector2>();
    }

    private void FixedUpdate() {
        if(!isHurt && !isAttack)
            Move();

        //判断材质
        CheckState(); //物理判断应放入fixupdate,这解决了player在重新激活后蹬墙跳力度变化的bug
    }

    //场景加载时禁用控制
    private void OnLoadEvent(GameSceneSO arg0, Vector3 arg1, bool arg2)
    {
        inputControl.GamePlay.Disable();
    }

    //场景加载结束启用控制
    private void OnSceneLoadedEvent()
    {
        inputControl.GamePlay.Enable();
    }

    public void Move() {
        if(!isCrouch && !wallJump)
            rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime, rb.velocity.y);
 
        if(inputDirection.x > 0)
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            physicsCheck.updateOffset();
        if(inputDirection.x < 0)
            transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
            physicsCheck.updateOffset();

        isCrouch = inputDirection.y < -0.5f && physicsCheck.isGround;
        if(isCrouch) {
            //下蹲并调整碰撞体
            rb.velocity = new Vector2(0.0f, rb.velocity.y);
            capsuleCollider.offset = new Vector2(-0.05f, 0.85f);
            capsuleCollider.size = new Vector2(0.7f, 1.7f);
        }
        else {
            //还原碰撞体
            capsuleCollider.offset = originalOffset;
            capsuleCollider.size = originalSize;
        }
    }

    private void Jump(InputAction.CallbackContext context)
    {
        // Debug.Log("Jump");
        if(physicsCheck.isGround){
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
            //打断滑铲
            isSlide = false;
            StopAllCoroutines();
            GetComponent<AudioDefination>()?.PlayAudioClip();
        }
        else if(physicsCheck.onWall){
            rb.AddForce(new Vector2(-inputDirection.x, 2.5f) * wallJumpForce, ForceMode2D.Impulse);
            wallJump = true;
            GetComponent<AudioDefination>()?.PlayAudioClip();
        }
    }

    
    private void Attack(InputAction.CallbackContext context)
    {
        if(physicsCheck.isGround) {
            isAttack = true;
            playerAnimation.playAttack();
        }
    }

    private void Slide(InputAction.CallbackContext context)
    {
        if(!isSlide && physicsCheck.isGround && character.currentPower > slidePowerCost){
            isSlide = true;
            slideTimeCounter = slideTime;

            var targetPos = new Vector3(transform.position.x + slideDistance * transform.localScale.x, transform.position.y);

            gameObject.layer = LayerMask.NameToLayer("Enemy");
            StartCoroutine(TriggerSlide(targetPos));

            character.OnSlide(slidePowerCost);
        }
    }

    private IEnumerator TriggerSlide(Vector3 target){
        do{
            yield return null;

            if(!physicsCheck.isGround)
                break;

            //滑动过程中撞墙
            if(physicsCheck.touchLeftWall && transform.localScale.x < 0.0f || physicsCheck.touchRightWall && transform.localScale.x > 0.0f){
                isSlide = false;
                break;
            }

            slideTimeCounter -= Time.deltaTime;
            rb.MovePosition(new Vector2(transform.position.x + transform.localScale.x * slideSpeed, transform.position.y));

        }while(Mathf.Abs(target.x - transform.position.x) > 0.1f && slideTimeCounter > 0);

        isSlide = false;
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    public void GetHurt(Transform attacker) {
        isHurt = true;
        rb.velocity = Vector2.zero;
        Vector2 hurtDir = new Vector2((transform.position.x - attacker.position.x), 0).normalized;
        rb.AddForce(hurtDir * hurtForce, ForceMode2D.Impulse);
    }

    public void PlayerDead() {
        isDead = true;
        inputControl.GamePlay.Disable();
    }

    private void CheckState() {
        capsuleCollider.sharedMaterial = physicsCheck.isGround ? normal : wall ;

        if(physicsCheck.onWall){
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2.0f);
        }

        if(wallJump && rb.velocity.y < 0.0f){
            wallJump = false;
        }
    }
}

