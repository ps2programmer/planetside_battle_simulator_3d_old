using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleSimulator.Core;

namespace BattleSimulator.WeaponScripts
{
    public class Projectile : MonoBehaviour
    {
        // PUBLIC
        [HideInInspector]
        public GameObject target;
        [HideInInspector]
        public float damage;
        [HideInInspector]
        public float speed;
        [HideInInspector]
        public bool isGoingToMissTarget;

        // PRIVATE
        private Vector3 _missPosition;
        private Transform _targetTransform;
        private IDamageable _targetIDamageable;

        void Start()
        {
            _targetTransform = target.GetComponent<Transform>();
            _targetIDamageable = target.GetComponent<IDamageable>();
            if (isGoingToMissTarget) {
                _missPosition = _targetTransform.position + new Vector3(UtilityFunctions.RandInt(1, 3), UtilityFunctions.RandInt(1, 3), UtilityFunctions.RandInt(1, 3));
            }
            if (_targetIDamageable == null) {
                Destroy(gameObject);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!_targetIDamageable.IsAlive()) {
                Destroy(gameObject);
                return;
            }
            MoveToTarget();
            if (ReachedTarget()) {
                if (!isGoingToMissTarget) {
                    _targetIDamageable.TakeDamage(damage);
                }
                Destroy(gameObject);
            }
        }

        void MoveToTarget() {
            if (!isGoingToMissTarget) {
                transform.LookAt(_targetTransform.position);
            } else {
                transform.LookAt(_missPosition);
            }
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        bool ReachedTarget() {
            if (!isGoingToMissTarget) {
                if (Vector3.Distance(transform.position, _targetTransform.position) <= speed * Time.deltaTime) {
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
