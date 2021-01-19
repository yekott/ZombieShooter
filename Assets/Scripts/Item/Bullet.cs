using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zombie
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField]
        float speed;
        [SerializeField]
        float damage_impact;
        [SerializeField]
        float damage_penetrate;
        [SerializeField]
        float fireDistance;
        [SerializeField]
        Vector3 whereShotFired;
        Vector3 dir;
        new Rigidbody rigidbody;

        private void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            rigidbody.isKinematic = true;
        }

        private void OnTriggerExit(Collider other)
        {
            IShootable shoot = other.GetComponentInParent<IShootable>();
            shoot.OnHit(other.gameObject.transform.position);
            
            Invoke("DestroyBullet", 5f);
        }

        public void SetSpeed(float speed)
        {
            this.speed = speed;
        }

        public void SetDamage(float Impact, float Penetrate)
        {
            damage_impact = Impact;
            damage_penetrate = Penetrate;
        }

        public void SetDistance(float fireDistance)
        {
            this.fireDistance = fireDistance;
        }

        public void DestroyBullet()
        {
            Destroy(gameObject);
        }

        public void SetShotFireLocation(Vector3 location, Vector3 dir)
        {
            whereShotFired = location;
            transform.LookAt(dir);
        }

        void FixedUpdate()
        {
            if(Vector3.Distance(whereShotFired, transform.position) > fireDistance)
            {
                Destroy(gameObject);
            }
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
        }
    }
}