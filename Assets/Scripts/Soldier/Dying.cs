using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleSimulator.Core;
using BattleSimulator.WeaponScripts;

namespace BattleSimulator.SoldierScripts
{
    public class Dying : IState
    {
        public bool readyToSelectSpawnPoint;

        private readonly ISoldier _soldier;

        public Dying(ISoldier soldier) {
            _soldier = soldier;
        }

        public void Tick() {

        }

        public void OnEnter() {
            Debug.Log("Entered Dying State");
            Debug.DrawRay(_soldier.soldierTransform.position, Vector3.up * 3f, Color.yellow, Mathf.Infinity, false);
            _soldier.DisableNavMeshAgent();
            _soldier.DisableRenderer();
            _soldier.DisableCollider();
            _soldier.isAlive = false;
            if (_soldier.currentWeapon != null) {
                _soldier.currentWeapon.DisableWeapon();
            }
            _soldier.ClearCurrentTarget();
            _soldier.currentlyIgnoringEnemies = false;
            _soldier.deathCount++;
            _soldier.currentLifeKillCount = 0;
            _soldier.selectionIndicator.SetActive(false);
            readyToSelectSpawnPoint = true;
        }

        public void OnExit() {
            readyToSelectSpawnPoint = false;
            // _soldier.previousStateWasAttacking = false;
        }
    }
}
