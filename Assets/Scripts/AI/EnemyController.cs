using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Zombie
{
    public class EnemyController : MonoBehaviour, IShootable
    {
        Animator animator;
        NavMeshAgent agent;
        Transform mTransform;

        Collider thisCapsule;

        Rigidbody[] ragdollBodies;
        Collider[] ragdollColliders;
        Collider Trigger;

        public int timesHit;
        int reactionChances;
        public bool isDead;
        float distanceFromPlayer;
        public float detectionDistance;
        public bool isWakeUp;

        public GameObject[] availableModels;
        public RuntimeAnimatorController[] runtimeAnimatorControllers;

        public Transform targetPlayer;


        private void Start()
        {
            int ran = Random.Range(0, availableModels.Length);
            availableModels[ran].SetActive(true);

            mTransform = this.transform;
            agent = GetComponentInChildren<NavMeshAgent>();
            animator = GetComponentInChildren<Animator>();
            ragdollBodies = GetComponentsInChildren<Rigidbody>();
            ragdollColliders = GetComponentsInChildren<Collider>();
            animator.applyRootMotion = false;
            thisCapsule = GetComponent<CapsuleCollider>();
            Trigger = GetComponent<Collider>();

            for(int i = 0; i < ragdollBodies.Length; i++)
            {
                ragdollBodies[i].isKinematic = true;
            }
            targetPlayer = FindObjectOfType<Controller>().transform;
            detectionDistance = Random.Range(3, 11);

            int animatorRan = Random.Range(0, runtimeAnimatorControllers.Length);
            animator.runtimeAnimatorController = runtimeAnimatorControllers[animatorRan];

            int run = Random.Range(0, 2);
            animator.SetInteger("run", run);

            isWakeUp = false;
            animator.applyRootMotion = true;
        }
        private void Update()
        {
            if (isDead)
                return;

            HandleDetect();

            bool rotateAvailable = animator.GetBool("RotateAvailable");
            if (rotateAvailable)
            {
                agent.SetDestination(targetPlayer.position);
                agent.updateRotation = rotateAvailable;
                animator.applyRootMotion = true;
                if (distanceFromPlayer < 1.5f)
                {
                    transform.LookAt(targetPlayer);
                    PlayAnimation("attack");
                }
            }
            else
            {
                animator.applyRootMotion = false;
            }

        }

        private void LateUpdate()
        {
            mTransform.position = agent.transform.position;
            agent.transform.localPosition = Vector3.zero;
            mTransform.rotation = agent.transform.rotation;
            agent.transform.localRotation = Quaternion.identity;
            animator.transform.localPosition = Vector3.zero;
            animator.transform.localRotation = Quaternion.identity;
        }

        public void OnHit(Vector3 dir)
        {
            if (!isDead && isWakeUp)
            {
                if (timesHit < 20)
                {
                    timesHit++;

                    int ran = Random.Range(0, 11);
                    if(ran < reactionChances)
                    {
                        PlayAnimation("reaction");
                    }
                    else
                    {
                        int ranChances = Random.Range(0, 4);
                        reactionChances += ranChances;
                    }
                }
                else
                {
                    thisCapsule.enabled = false;
                    Trigger.isTrigger = false;
                    agent.enabled = false;
                    EnableRagdoll();
                    isDead = true;
                    Invoke("DisableRagdoll",5f);
                }
            }
        }

        void DisableRagdoll()
        {
            for (int i = 0; i < ragdollBodies.Length; i++)
            {
                ragdollBodies[i].isKinematic = true;
            }

        }
        void EnableRagdoll()
        {
            animator.enabled = false;
            for (int i = 0; i < ragdollBodies.Length; i++)
            {
                ragdollBodies[i].isKinematic = false;
            }
            //for (int i = 0; i < ragdollColliders.Length; i++)
            //{
            //    ragdollColliders[i].isTrigger = false;
            //}
        }

        void HandleDetect()
        {
            if (targetPlayer == null)
                return;

            distanceFromPlayer = Vector3.Distance(mTransform.position, targetPlayer.position);
            if (!animator.GetBool("DetectPlayer"))
            {
                if (distanceFromPlayer < detectionDistance)
                {
                    animator.SetBool("DetectPlayer", true);
                    isWakeUp = true;
                }
            }
            else
            {
                if (distanceFromPlayer >= detectionDistance)
                {
                    animator.SetBool("DetectPlayer", false);
                }
            }

        }

        public void PlayAnimation(string targetAnim)
        {
            animator.Play(targetAnim);
        }
    }
}