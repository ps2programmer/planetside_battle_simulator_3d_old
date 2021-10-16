using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleSimulator.Core;

namespace BattleSimulator.SoldierScripts
{
    public class MovingToCapturePoint : IState
    {
        public bool capturePointCaptured;

        private ISoldier _soldier;

        public MovingToCapturePoint(ISoldier soldier) {
            _soldier = soldier;
        }

        public void Tick() {
            if (_soldier.currentCapturePoint.currentlyControllingFaction == _soldier.faction) {
                _soldier.currentCapturePoint = null;
                capturePointCaptured = true;
            }
        }

        public void OnEnter() {
            Debug.Log("Entered Moving To Capture Point State");
            capturePointCaptured = false;
            _soldier.agent.SetDestination(_soldier.currentCapturePoint.GetPosition());
            _soldier.currentDestination = _soldier.currentCapturePoint.GetPosition();
            _soldier.currentDistanceToleranceToConsiderDestinationReached = _soldier.distanceToleranceToConsiderDestinationReached + _soldier.currentCapturePoint.meshRadius;
        }

        public void OnExit() {
            // _soldier.previousStateWasAttacking = false;
            _soldier.currentDistanceToleranceToConsiderDestinationReached = _soldier.distanceToleranceToConsiderDestinationReached;
        }
    }
}
