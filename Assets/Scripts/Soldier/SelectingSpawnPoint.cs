using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleSimulator.Core;
using BattleSimulator.SpawnPointScripts;

namespace BattleSimulator.SoldierScripts
{
    public class SelectingSpawnPoint : IState
    {
        public bool spawnPointSelected;
        public SpawnPoint[] allSpawnPoints;
        public List<SpawnPoint> allFriendlySpawnPoints;

        private readonly ISoldier _soldier;

        public SelectingSpawnPoint(ISoldier soldier) {
            _soldier = soldier;
        }

        public void Tick() {
            FindAllSpawnPoints();
            foreach (SpawnPoint spawnPoint in allSpawnPoints) {
                if (spawnPoint.faction == _soldier.faction) {
                    allFriendlySpawnPoints.Add(spawnPoint);
                }
            }
            if (allFriendlySpawnPoints.Count != 0) {
                _soldier.currentSpawnPoint = allFriendlySpawnPoints[UtilityFunctions.RandInt(0, allFriendlySpawnPoints.Count)];
                spawnPointSelected = true;
            } else {
                _soldier.currentSpawnPoint = null;
                spawnPointSelected = false;
            }
        }

        public void OnEnter() {
            Debug.Log("Entered Selecting Spawn Point State");
            spawnPointSelected = false;
            allFriendlySpawnPoints = new List<SpawnPoint>();
        }

        public void OnExit() {
            // _soldier.previousStateWasAttacking = false;
        }

        private void FindAllSpawnPoints() {
            allSpawnPoints = Object.FindObjectsOfType<SpawnPoint>();
        }
    }
}
