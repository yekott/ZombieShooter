using UnityEngine;
using System.Collections;
using Zombie.Item;

namespace Zombie
{
    public class LevelObject : MonoBehaviour, IShootable
    {

        new Rigidbody rigidbody;


        void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
        }
        public void OnHit(Vector3 dir)
        {
            Vector3 direction = dir.normalized;
            rigidbody.AddForce(direction * 3, ForceMode.Impulse);
        }

    }
}