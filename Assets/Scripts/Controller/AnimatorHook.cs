using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zombie.Item;

namespace Zombie
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorHook : MonoBehaviour
    {
        Animator animator;

        Transform shoulderTransform;
        Transform aimPivot;
        Transform rh_target;
        public Transform lookAtTarget;

        WeaponHook currentWeapon;

        private void Start()
        {
            animator = GetComponent<Animator>();
            shoulderTransform = animator.GetBoneTransform(HumanBodyBones.RightShoulder);
        }

        public void LoadCurrentWeapon(WeaponHook hook, Transform pivot, Transform rhTarget)
        {
            currentWeapon = hook;
            if(aimPivot != null)
            {
                Destroy(aimPivot.gameObject);
            }
            aimPivot = pivot;
            rh_target = rhTarget;
        }

        private void OnAnimatorMove()
        {
            if (currentWeapon == null || aimPivot == null)
                return;
            
            aimPivot.transform.position = shoulderTransform.position;
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (currentWeapon == null || rh_target == null)
                return;

            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            animator.SetIKRotation(AvatarIKGoal.RightHand, rh_target.rotation);
            animator.SetIKPosition(AvatarIKGoal.RightHand, rh_target.position);

            if (currentWeapon.lh_ik_target != null)
            {
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                animator.SetIKRotation(AvatarIKGoal.LeftHand, currentWeapon.lh_ik_target.rotation);
                animator.SetIKPosition(AvatarIKGoal.LeftHand, currentWeapon.lh_ik_target.position);
            }

            animator.SetLookAtWeight(1f, 1f, 1f, 1f, 0.5f);
            animator.SetLookAtPosition(lookAtTarget.position);
        }
    }
}