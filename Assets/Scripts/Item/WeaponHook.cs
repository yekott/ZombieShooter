using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zombie.Item
{
    public class WeaponHook : MonoBehaviour
    {
        ParticleSystem[] particles;

        Weapon baseWeapon;
        public Transform fireTransform;
        public AudioClip getReloadFX { get { return baseWeapon.reloadClip; } }
        public AudioClip getShotFX { get { return baseWeapon.shotClip; } }
        public AudioClip getEmptyFX { get { return baseWeapon.emptyClip; } }

        public Vector3 thisforward;
        public AudioSource gunAudioPlayer;
        public Transform lh_ik_target;

        public MagazineHook currentMag;
        public Ammo insertAmmo;
        Ammo FiredAmmo;
        
        float lastShot;

        public void Init(Weapon weapon)
        {
            baseWeapon = weapon;
            gunAudioPlayer = GetComponent<AudioSource>();

            currentMag.ReloadAmmo(insertAmmo, currentMag.magCapacity);

            particles = transform.GetComponentsInChildren<ParticleSystem>();

            baseWeapon.state = Weapon.MagState.Ready;
            baseWeapon.lastFireTime = 0;
        }

        private void Update()
        {
            thisforward = transform.forward;
        }
        public void Shoot(Vector3 targetLocation)
        {
            if (baseWeapon.state == Weapon.MagState.Ready &&
                Time.time >= baseWeapon.lastFireTime + baseWeapon.timeBetFire)
            {
                baseWeapon.lastFireTime = Time.time;
                FiredAmmo = currentMag.Shoot();
                if(FiredAmmo == null)
                {
                    gunAudioPlayer.clip = getEmptyFX;
                    gunAudioPlayer.Play();
                }
                else
                {
                    GameObject go = Instantiate(FiredAmmo.AmmoPrefab, fireTransform.position, fireTransform.rotation); 
                    Bullet bullet = go.AddComponent<Bullet>();
                    bullet.SetDistance(FiredAmmo.getFireDistance());
                    bullet.SetDamage(FiredAmmo.getDamageImpact(), FiredAmmo.getDamagePenetrate());
                    bullet.SetShotFireLocation(fireTransform.position, targetLocation);
                    bullet.SetSpeed(FiredAmmo.getFireDistance());
                    //go.transform.localScale = new Vector3(10, 10, 10);

                    for (int i = 0; i < particles.Length; i++)
                    {
                        particles[i].Play();
                    }
                    gunAudioPlayer.clip = getShotFX;
                    gunAudioPlayer.Play();

                }
            }
        }
    }
}