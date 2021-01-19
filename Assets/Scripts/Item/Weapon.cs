using UnityEngine;
using System.Collections;

namespace Zombie.Item
{
    [CreateAssetMenu(menuName ="Items/Weapon")]
    public class Weapon : Item
    {
        //탄창 상태 enum
        public enum MagState
        {
            Ready,
            Empty,
            Reloading
        }

        public MagState state { get; set; }
        
        public GameObject modelPrefab;
        public GameObject aimPivotPrefab;

        public AudioClip shotClip;
        public AudioClip reloadClip;
        public AudioClip emptyClip;

        public float timeBetFire = 0.12f; // 연사속도
        public float reloadTime = 1.8f;
        public float lastFireTime;

        public bool isAttachedRazerSight;
        public bool isAttachedFlashLight;        
    }
}