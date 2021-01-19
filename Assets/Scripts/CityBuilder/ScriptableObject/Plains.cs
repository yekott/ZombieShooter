using UnityEngine;
using System.Collections;

namespace Zombie
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Plains")]
    public class Plains : ScriptableObject
    {
        public GameObject[] Fields;
    }
}
