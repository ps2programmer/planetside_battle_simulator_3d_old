using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleSimulator.Core;
using BattleSimulator.SoldierScripts;

namespace BattleSimulator.WeaponScripts
{
    public class Projectile : MonoBehaviour
    {
        // PUBLIC
        [HideInInspector]
        public IDamageable target;
        [HideInInspector]
        public float damage;
        [HideInInspector]
        public float speed;
        [HideInInspector]
        public bool isGoingToMissTarget;
        public ISoldier soldier;

        // PRIVATE
        private Vector3 _missPosition;

        void Start()
        {
            if (isGoingToMissTarget) {
                _missPosition = target.GetTransform().position + new Vector3(UtilityFunctions.RandInt(1, 2), UtilityFunctions.RandInt(1, 2), UtilityFunctions.RandInt(1, 2));
            }
            if (target == null) {
                Destroy(gameObject);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!target.IsAlive()) {
                Destroy(gameObject);
                return;
            }
            MoveToTarget();
            if (ReachedTarget()) {
                if (!isGoingToMissTarget) {
                    target.TakeDamage(damage, this);
                }
                Destroy(gameObject);
            }
        }

        void MoveToTarget() {
            if (!isGoingToMissTarget) {
                transform.LookAt(target.GetTransform().position);
            } else {
                transform.LookAt(_missPosition);
            }
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        bool ReachedTarget() {
            if (!isGoingToMissTarget) {
                if (Vector3.Distance(transform.position, target.GetTransform().position) <= speed * Time.deltaTime) {
                    return true;
                } else {
                    return false;
                }
            } else {
                if (Vector3.Distance(transform.position, _missPosition) <= speed * Time.deltaTime) {
                    return true;
                } else {
                    return false;
                }
            }
        }
    }
}
