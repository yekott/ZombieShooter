using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Zombie
{
    public class MagazineHook : MonoBehaviour
    {
        Stack<Ammo> ammo;

        public int ammoRemain = 100;
        public int magCapacity = 25;

        //한알씩 푸쉬해서 넣음
        public bool ReloadOneAmmo(Ammo bullet)
        {
            if (ammo.Count >= magCapacity)
            {
                return false;
            }
            ammo.Push(bullet);
            return true;
        }
        //넣은 탄알 수를 계산해서 남는 탄환수 리턴
        public int ReloadAmmo(Ammo bullets, int amount)
        {
            if(ammo == null)
            {
                ammo = new Stack<Ammo>();
            }

            if(ammo.Count >= magCapacity)
            {
                Debug.Log("so many bullets!");
                return amount;
            }

            for(int i = 0; i<amount; i++)
            {
                if (!ReloadOneAmmo(bullets))
                {
                    Debug.Log("Reload one Bullet");
                    return amount - i;
                }
            }

            return 0;
        }

        public Ammo Shoot()
        {
            if(ammo.Count < 1)
            {
                return null;
            }
            Ammo tmpBullet = ammo.Pop();
            return tmpBullet;
        }
    }
}