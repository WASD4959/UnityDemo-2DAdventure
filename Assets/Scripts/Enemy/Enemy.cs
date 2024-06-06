using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(PhysicsCheck))]
public class Enemy : MonoBehaviour
{
    [HideInInspector]public Rigidbody2D rb;
    [HideInInspector]public PhysicsCheck physicsCheck;
    [HideInInspector]public Animator animator;

    [Header("基本参数")]
    public float normalSpeed;
    public float chaseSpeed;
    [HideInInspector]public float currentSpeed;
    public Vector3 faceDir;
    public Transform attacker;
    public float hurtForce;
    public Vector3 spwanPoint;


    [Header("计时器")]
    public float waitTime;
    public float waitTimeCounter;
    public bool wait;
    public float lostTime;
    public float lostTimeCounter;

    [Header("检测")]
    public Vector2 centerOffset;
    public Vector2 checkSize;
    public float checkDistance;
    public LayerMask attackLayer;

    [Header("状态")]
    public bool isHurt;
    public bool isDead;

    private BaseState currentState;
    protected BaseState patrolState;
    protected BaseState chaseState;
    protected BaseState skillState;

    protected virtual void Awake() {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        physicsCheck = GetComponent<PhysicsCheck>();

        currentSpeed = normalSpeed;
        // waitTimeCounter = waitTime;
        spwanPoint = transform.position;
    }

    private void OnEnable() {
        currentState = patrolState;
        currentState.OnEnter(this);
    }
    private void Update() {
        faceDir = new Vector3(-transform.localScale.x, 0, 0);
        
        currentState.LogicUpdate();
        TimeCounter();
    }

    private void FixedUpdate() {
        physicsCheck.updateOffset();

        currentState.PhysicsUpdate();    
    }

    public virtual void move() {
        rb.velocity = new Vector2(currentSpeed * faceDir.x * Time.deltaTime, rb.velocity.y);
    }

    public void TimeCounter() {
        if(wait) {
            waitTimeCounter -= Time.deltaTime;
            rb.velocity = new Vector2(0, rb.velocity.y);
            if(waitTimeCounter <= 0) {
                wait = false;
                waitTimeCounter = waitTime;
                transform.localScale = new Vector3(faceDir.x, 1, 1);
            }
        }

        if(!FoundPlayer() && lostTimeCounter > 0){
            lostTimeCounter -= Time.deltaTime;
        }
    }

    public virtual bool FoundPlayer(){
        return Physics2D.BoxCast(transform.position + (Vector3)centerOffset, checkSize, 0, faceDir, checkDistance, attackLayer);
    }

    public void OnTakeDamage(Transform attackerTrans){
        attacker = attackerTrans;

        //转身
        if(attackerTrans.position.x - transform.position.x > 0)
            transform.localScale =new Vector3(-1, 1, 1);
        if(attackerTrans.position.x - transform.position.x < 0)
            transform.localScale =new Vector3(1, 1, 1);

        //受伤击退
        isHurt = true;
        animator.SetTrigger("hurt");
        Vector2 dir = new Vector2(transform.position.x - attackerTrans.position.x, 0).normalized;

        StartCoroutine(OnHurt(dir));
    }

    private IEnumerator OnHurt(Vector2 dir){
        rb.velocity = new Vector2(0, rb.velocity.y);
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.5f);
        isHurt = false;
    }

    public void OnDie(){
        gameObject.layer = 2;
        animator.SetBool("isDead", true);
        isDead = true;
        rb.velocity = Vector2.zero;
    }

    public void DestoryAfterAnimation(){
        Destroy(this.gameObject);
    }

    private void OnDisable() {
        currentState.OnExit();
    }

    public void SwitchState(NPCState state){
        var newState = state switch{
            NPCState.Patrol => patrolState,
            NPCState.Chase => chaseState,
            NPCState.Skill => skillState,
            _ => null
        };

        currentState.OnExit();
        currentState = newState;
        currentState.OnEnter(this);
    }

    public virtual Vector3 GetNewPoint(){
        return transform.position;
    }

    public virtual void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position + (Vector3)centerOffset + new Vector3(checkDistance * -transform.localScale.x, 0), 0.2f);
    }
}
