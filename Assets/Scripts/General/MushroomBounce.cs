using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomBounce : MonoBehaviour
{
    public Animator mushroomAnimator; // 蘑菇的动画控制器
    public float bounceForce; // 弹起的力量
    private bool isSteppedOn = false;
    private bool isRestoring = false;
    private bool isStayCollider = false;

    void Start()
    {
        if (mushroomAnimator == null)
        {
            mushroomAnimator = GetComponent<Animator>();
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        if(other.CompareTag("Player"))
            isStayCollider = true;
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("Player"))
            isStayCollider = false;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player"))
        {
            if (!isSteppedOn && !isRestoring)
            {
                isSteppedOn = true;
                mushroomAnimator.SetTrigger("SteppedOn");
                Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
                StartCoroutine(BouncePlayer(playerRb));
            }
        }
    }


    IEnumerator BouncePlayer(Rigidbody2D playerRb)
    {
        // 等待直到蘑菇进入回弹状态
        while (!isRestoring)
        {
            if (mushroomAnimator.GetCurrentAnimatorStateInfo(0).IsName("Restore")) // 替换为实际的回弹动画状态名称
            {
                isRestoring = true;
                if (playerRb != null && isStayCollider)
                {
                    playerRb.AddForce(transform.up * bounceForce, ForceMode2D.Impulse); // 给玩家向上的力量
                }
            }
            yield return null;
        }

        // 等待回弹动画完成
        yield return new WaitForSeconds(0.5f);
        isSteppedOn = false;
        isRestoring = false;
    }
}

