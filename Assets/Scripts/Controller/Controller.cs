using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using Zombie.Item;

namespace Zombie
{
    public class Controller : MonoBehaviour
    {
        NavMeshAgent agent;
        public GameObject character;
        Animator anim;
        public float moveSpeed = 6f;
        public float rotateSpeed = 9f;
        Transform mTransform;
        Vector3 dir;
        public Transform AimTarget;

        public Weapon defaultWeapon;
        WeaponHook currentWeaponHook;
        AnimatorHook animatorHook;
        LineRenderer lineRenderer;
        
        public void Execute()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.updatePosition = false;
            mTransform = this.transform;
            anim = GetComponentInChildren<Animator>();
            animatorHook = GetComponentInChildren<AnimatorHook>();
            lineRenderer = GetComponent<LineRenderer>();
        }
        public void MoveCharacter(Vector3 moveDirection)
        {
            agent.velocity = moveDirection * moveSpeed;
            character.transform.position = agent.nextPosition;
        }

        public void HandleAnimation(Vector3 moveDirection)
        {
            Vector3 relative = mTransform.InverseTransformDirection(moveDirection);
            float v = relative.z;
            float h = relative.x;

            anim.SetFloat("forward", v);
            anim.SetFloat("sideways", h);
        }
        
        public void RotateToDirectionMouse(Vector3 mousePosition, float delta)
        {
            dir = mousePosition - mTransform.position;
            dir.Normalize();
            dir.y = 0;
            RaycastHit hit;

            int layermask = (1 << 9 | 1 << 10 | 1 << 12);

            lineRenderer.SetPosition(0, currentWeaponHook.fireTransform.position);
            if (Physics.Raycast(currentWeaponHook.fireTransform.position, (AimTarget.position- currentWeaponHook.fireTransform.position).normalized, out hit, 30f, layermask))
            {
                lineRenderer.SetPosition(1, hit.point);
            }
            else
            {
                lineRenderer.SetPosition(1, AimTarget.position);
            }
            mTransform.rotation = Quaternion.Lerp(mTransform.rotation, Quaternion.LookRotation(dir),Time.deltaTime*rotateSpeed);
        }
        
        public void LoadWeapon(Weapon targetWeapon)
        {
            GameObject go = Instantiate(targetWeapon.modelPrefab) as GameObject;
            Transform rh = anim.GetBoneTransform(HumanBodyBones.RightHand);
            go.transform.parent = rh;
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
            WeaponHook hook = go.GetComponent<WeaponHook>();
            hook.Init(targetWeapon);
            currentWeaponHook = hook;

            GameObject aimPivot = Instantiate(targetWeapon.aimPivotPrefab) as GameObject;
            Transform rh_ik = aimPivot.transform.GetChild(0);
            aimPivot.transform.parent = this.transform;

            animatorHook.LoadCurrentWeapon(currentWeaponHook, aimPivot.transform, rh_ik);
        }

        public void HandleShooting()
        {
            currentWeaponHook.Shoot(AimTarget.position);
        }
    }
}