using UnityEngine;
using System.Collections;

namespace Zombie
{
    [CreateAssetMenu(menuName ="Items/Ammo")]
    public class Ammo : ScriptableObject
    {
        [SerializeField]
        float bulletSpeed = 50f;
        
        public GameObject AmmoPrefab;
        [SerializeField]
        float damage_impact = 25f;
        [SerializeField]
        float damage_penetrate = 25;
        [SerializeField]
        float fireDistance = 50f;

        public float getBulletSpeed() { return bulletSpeed; }
        public float getDamageImpact() { return damage_impact; }
        public float getDamagePenetrate() { return damage_penetrate; }
        public float getFireDistance() { return fireDistance; }
    }
}