using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zombie
{
    public class SetBoolOnEnter : StateMachineBehaviour
    {
        public bool status;
        public string boolName;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool(boolName, status);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            animator.SetBool(boolName, !status);
        }
    }
}
