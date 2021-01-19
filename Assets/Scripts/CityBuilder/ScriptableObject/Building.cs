using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zombie
{
    [CreateAssetMenu(menuName ="ScriptableObjects/Building")]
    public class Building : ScriptableObject
    {
        public GameObject Prefab;
        public int WidthX;
        public int HeightZ;

        public float offSetX;
        public float offSetZ;
    }
}