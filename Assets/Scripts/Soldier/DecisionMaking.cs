using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleSimulator.Core;
using BattleSimulator.SoldierScripts;

namespace BattleSimulator.SoldierScripts
{
    public class DecisionMaking : IState
    {
        public Type decidedToTransitionTo;

        private readonly ISoldier _soldier;

        public DecisionMaking(ISoldier soldier) {
            _soldier = soldier;
        }

        public void Tick() {
            // If coming out of attacking state decide whether to resume previous state actions
            // If currentCapturePoint exists then continue capturing it
            /* if (_soldier.previousStateWasAttacking) {
                if (_soldier.currentCapturePoint != null) {
                    decidedToTransitionTo = typeof(MovingToCapturePoint);
                    return;
                }
            } */
            // Determine if soldier wants to go to a capture point
            if (UtilityFunctions.random.NextDouble() <= _soldier.probabilityOfCaringAboutCapturePoint) {
                decidedToTransitionTo = typeof(SelectingCapturePoint);
                return;
            }
            // Else move randomly
            decidedToTransitionTo = typeof(MoveRandomly);
        }

        public void OnEnter() {
            Debug.Log("Entered Decision Making State");
            decidedToTransitionTo = null;
        }

        public void OnExit() {
            // _soldier.previousStateWasAttacking = false;
            decidedToTransitionTo = null;
        }
    }
}
