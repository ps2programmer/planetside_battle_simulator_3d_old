using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleSimulator.Core;

namespace BattleSimulator.SoldierScripts
{
    public class Attacking : IState
    {
        public float probabilityOfDecidingToIgnoreEnemy;
        public float percentIncreaseToProbabilityOfDecidingToIgnoreEnemyWhenMovingToCapturePoint;

        private readonly ISoldier _soldier;
        private float _currentProbabilityOfDecidingToIgnoreEnemy;

        public Attacking(ISoldier soldier) {
            _soldier = soldier;
            probabilityOfDecidingToIgnoreEnemy = (float)UtilityFunctions.random.NextDouble() * 0.01f;
            percentIncreaseToProbabilityOfDecidingToIgnoreEnemyWhenMovingToCapturePoint = (float)UtilityFunctions.random.NextDouble() * 100.0f;
            _currentProbabilityOfDecidingToIgnoreEnemy = probabilityOfDecidingToIgnoreEnemy;
        }

        public void Tick() {
            // Decide whether to ignore enemy or not, probability increased by X% when moving to point unless already within radius of point
            /* if (_soldier.currentCapturePoint != null && Vector3.Distance(_soldier.soldierTransform.position, _soldier.currentCapturePoint.GetPosition()) <= _soldier.currentCapturePoint.flipRadius) {
                _currentProbabilityOfDecidingToIgnoreEnemy += percentIncreaseToProbabilityOfDecidingToIgnoreEnemyWhenMovingToCapturePoint * (1 - probabilityOfDecidingToIgnoreEnemy);
                _currentProbabilityOfDecidingToIgnoreEnemy = Mathf.Min(1.0f, _currentProbabilityOfDecidingToIgnoreEnemy);
            } else {
                _currentProbabilityOfDecidingToIgnoreEnemy = probabilityOfDecidingToIgnoreEnemy;
            } */
            /* if (UtilityFunctions.random.NextDouble() <= _currentProbabilityOfDecidingToIgnoreEnemy) {
                _soldier.ClearCurrentTarget();
                _soldier.currentlyIgnoringEnemies = true;
                _soldier.timeToStopIgnoringEnemies = Time.time + 2.0f;
                return;
            } */
            bool miss = false;
            if (UtilityFunctions.random.NextDouble() >= _soldier.skill) {
                miss = true;
            }
            _soldier.currentWeapon.Shoot(_soldier.currentTarget, miss);
            _soldier.soldierTransform.LookAt(_soldier.currentTarget.GetTransform().position);
        }

        public void OnEnter() {
            Debug.Log("Entered Attacking State");
            DecideWhetherToIgnoreCurrentTargetAndAllOtherTargetsForCertainPeriodOfTime();
            _soldier.agent.ResetPath();
        }

        public void OnExit() {
            _soldier.ClearCurrentTarget();
            // _soldier.previousStateWasAttacking = true;
        }

        void DecideWhetherToIgnoreCurrentTargetAndAllOtherTargetsForCertainPeriodOfTime() {
            if (UtilityFunctions.random.NextDouble() >= _soldier.probabilityOfEngagingEnemyTarget && !_soldier.currentlyIgnoringEnemies) {
                int timeToIgnoreEnemiesFor = UtilityFunctions.RandInt(3, 5);
                _soldier.StartIgnoreEnemiesCoroutine(timeToIgnoreEnemiesFor);
            }
        }
    }
}
