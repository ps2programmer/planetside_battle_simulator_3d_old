using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleSimulator.Core;
using BattleSimulator.CapturePointScripts;

namespace BattleSimulator.SoldierScripts
{
    public class SelectingCapturePoint : IState
    {
        public bool selectedContestedCapturePoint;
        public bool allCapturePointsAreControlled;

        private readonly ISoldier _soldier;

        public SelectingCapturePoint(ISoldier soldier) {
            _soldier = soldier;
        }

        public void Tick() {

        }

        public void OnEnter() {
            Debug.Log("Entered Selecting Capture Point State");
            selectedContestedCapturePoint = false;
            allCapturePointsAreControlled = false;
            // This ensures that if the soldier is coming out of an attacking state and was previously going to a capture point,
            // then it continues going to the same capture point
            if (_soldier.currentCapturePoint != null) {
                if (_soldier.currentCapturePoint.currentlyControllingFaction != _soldier.faction) {
                    selectedContestedCapturePoint = true;
                    return;
                } else {
                    _soldier.currentCapturePoint = null;
                }
            }
            var allCapturePoints = Object.FindObjectsOfType<CapturePoint>();
            List<CapturePoint> allContestedCapturePoints = new List<CapturePoint>();
            foreach (CapturePoint capturePoint in allCapturePoints) {
                if (capturePoint.currentlyControllingFaction != _soldier.faction) {
                    allContestedCapturePoints.Add(capturePoint);
                }
            }
            if (allContestedCapturePoints.Count != 0) {
                _soldier.currentCapturePoint = allContestedCapturePoints[UtilityFunctions.RandInt(0, allContestedCapturePoints.Count)];
                selectedContestedCapturePoint = true;
            } else {
                _soldier.currentCapturePoint = null;
                allCapturePointsAreControlled = true;
            }
        }

        public void OnExit() {
            // _soldier.previousStateWasAttacking = false;
            selectedContestedCapturePoint = false;
        }
    }
}
