using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleSimulator.Core;
using BattleSimulator.WeaponScripts;

namespace BattleSimulator.SoldierScripts
{
    public class Respawning : IState
    {
        public float spawnTime;

        private readonly ISoldier _soldier;

        public Respawning(ISoldier soldier) {
            _soldier = soldier;
        }

        public void Tick() {

        }

        public void OnEnter() {
            Debug.Log("Entered Respawning State");
            spawnTime = Time.time + _soldier.currentSpawnPoint.spawnTimer;
        }

        public void OnExit() {
            _soldier.ReplenishHealthAndShields();
            _soldier.EndShieldRechargeDelay();
            _soldier.soldierRenderer.material.color = _soldier.factionColor;
            _soldier.isAlive = true;
            // _soldier.previousStateWasAttacking = false;

            _soldier.soldierTransform.position = _soldier.currentSpawnPoint.spawnLocationTransform.position;
            _soldier.EnableNavMeshAgent();
            _soldier.EnableRenderer();
            _soldier.EnableCollider();
            if (_soldier.currentWeapon == null) {
                SelectRandomWeapon();
            }
            if (_soldier.currentWeapon != null) {
                _soldier.currentWeapon.EnableWeapon();
            }
            if (_soldier.isSelected) {
                _soldier.selectionIndicator.SetActive(true);
            }
        }

        void SelectRandomWeapon() {
            if (_soldier.weaponList.Count != 0) {
                var randomWeapon = _soldier.weaponList[UtilityFunctions.RandInt(0, _soldier.weaponList.Count)];
                Weapon newWeapon = Object.Instantiate(randomWeapon, _soldier.weaponPositionTransform.position, Quaternion.identity, _soldier.weaponPositionTransform) as Weapon;
                _soldier.currentWeapon = newWeapon;
                _soldier.currentWeapon.soldier = _soldier;
            }
        }
    }
}
