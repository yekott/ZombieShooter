using UnityEngine;
using System.Collections;

namespace Zombie
{
    [CreateAssetMenu(menuName = "ScriptableObjects/BuildingList")]
    public class BuildingList : ScriptableObject
    {
        public Building[] buildings;
    }
}