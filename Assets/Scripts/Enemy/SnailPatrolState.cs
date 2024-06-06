using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnailPatrolState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.normalSpeed;
    }

    public override void LogicUpdate()
    {
        if(currentEnemy.FoundPlayer()){
            currentEnemy.SwitchState(NPCState.Skill);
        }

        if(!currentEnemy.physicsCheck.isGround || (currentEnemy.physicsCheck.touchLeftWall && currentEnemy.faceDir.x < 0) || (currentEnemy.physicsCheck.touchRightWall && currentEnemy.faceDir.x > 0)) {
            currentEnemy.wait = true;
            currentEnemy.animator.SetBool("isWalk", false);
        }
        else if(!currentEnemy.wait){
            currentEnemy.animator.SetBool("isWalk", true);
        }
    }

    public override void PhysicsUpdate()
    {
        if(!currentEnemy.isHurt && !currentEnemy.isDead && !currentEnemy.wait && !currentEnemy.animator.GetCurrentAnimatorStateInfo(0).IsName("snailPreMove") && !currentEnemy.animator.GetCurrentAnimatorStateInfo(0).IsName("snailRecover") )
            currentEnemy.move();          
    }

    public override void OnExit()
    {
        currentEnemy.animator.SetBool("isWalk", false);
    }
}
