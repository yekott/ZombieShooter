using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zombie.Item;

namespace Zombie
{
    public interface IShootable
    {
        void OnHit(Vector3 dir);
    }
}