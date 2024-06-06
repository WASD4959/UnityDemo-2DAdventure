using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarPatrolState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.normalSpeed;
    }

    public override void LogicUpdate()
    {
        // TODO: 发现player切换到chaseState
        if(currentEnemy.FoundPlayer()){
            currentEnemy.SwitchState(NPCState.Chase);
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
        if(!currentEnemy.isHurt && !currentEnemy.isDead && !currentEnemy.wait)
            currentEnemy.move();
    }

    public override void OnExit()
    {
        currentEnemy.animator.SetBool("isWalk", false);
    }
}
