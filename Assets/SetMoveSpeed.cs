using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SetMoveSpeed : StateMachineBehaviour
{
    public float agentSpeed = 3;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        NavMeshAgent agent = animator.transform.root.GetComponentInChildren<NavMeshAgent>();
        agent.speed = agentSpeed;
    }
}
