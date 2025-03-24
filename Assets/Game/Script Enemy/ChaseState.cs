using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : BaseState
{
    private float originalSpeed;
    private float originalPitch;
    public void EnterState(Enemy enemy)
    {
        // Debug.Log("Start Chasing");
        enemy.Animator.SetTrigger("ChaseState");
        enemy.Animator.SetBool("isPatrol", false);
        enemy.Animator.SetBool("isChasing", true);

        originalSpeed = enemy.NavMeshAgent.speed;
        enemy.NavMeshAgent.speed *= 1.5f;
        originalPitch = enemy.AudioSource.pitch;
        enemy.AudioSource.pitch *= 1.5f;
    }
    public void UpdateState(Enemy enemy)
    {
        // Debug.Log("Chasing");
        if (enemy.Player != null)
        {
            enemy.NavMeshAgent.destination = enemy.Player.transform.position;
            if (Vector3.Distance(enemy.transform.position, enemy.Player.transform.position) > enemy.ChaseDistance)
            {
                enemy.SwitchState(enemy.PatrolState);
            }
        }
    }
    public void ExitState(Enemy enemy)
    {
        Debug.Log("Stop Chasing");
        enemy.NavMeshAgent.speed = originalSpeed;
        enemy.AudioSource.pitch = originalPitch;
    }
}