using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleSimulator.Core;
using BattleSimulator.SoldierScripts;

namespace BattleSimulator.WeaponScripts
{
    public class Weapon : MonoBehaviour
    {
        // PUBLIC
        public Projectile projectilePrefab;
        public Transform projectileSpawnTransform;
        public float damage;
        public float bulletVelocity;
        [Tooltip("The time between shots. Lower values represent larger firerates.")]
        public float firerate;
        public ISoldier soldier;

        // PRIVATE
        public float _nextFireTime;

        void Awake() {
            _nextFireTime = Time.time;
        }

        public void Shoot(IDamageable target, bool miss) {
            if (Time.time >= _nextFireTime) {
                _nextFireTime = Time.time + firerate;
                Projectile newProjectile = Instantiate(projectilePrefab, projectileSpawnTransform.position, Quaternion.identity) as Projectile;
                newProjectile.damage = damage;
                newProjectile.speed = bulletVelocity;
                newProjectile.target = target;
                newProjectile.soldier = soldier;
                if (miss) {
                    newProjectile.isGoingToMissTarget = true;
                } else {
                    newProjectile.isGoingToMissTarget = false;
                }
            }
        }

        public void DisableWeapon() {
            gameObject.SetActive(false);
        }

        public void EnableWeapon() {
            gameObject.SetActive(true);
        }
    }
}
