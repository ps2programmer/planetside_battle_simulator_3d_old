using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleSimulator.Core;

namespace BattleSimulator.SoldierScripts
{
    public class HoldingPosition : IState
    {
    	public bool doneWithHoldingPosition;

        private ISoldier _soldier;
        private Vector3 _anchorPosition; // The center of the radius where soldier is holding position
        private float _holdPositionRadius;
        private bool _hasCurrentDestination;
        private Vector3 _nullDestination = Vector3.one * 1000000f;
        private Vector3 _soldierLastFramePosition;
        private int _currentNumberOfFramesWithoutMovement;
        private int _maximumNumberOfFramesWithoutMovement;
        private float _timeWithoutKillsUntilSoldierGetsBoredAndWantsToMoveSomewhereElse;

        public HoldingPosition(ISoldier soldier) {
        	_soldier = soldier;
        	_maximumNumberOfFramesWithoutMovement = 100;
        	_timeWithoutKillsUntilSoldierGetsBoredAndWantsToMoveSomewhereElse = (float)UtilityFunctions.random.NextDouble() * (60.0f - 10.0f) + 10.0f;
        }

        public void Tick() {
        	// If holding position due to capturing a capture point then stop holding once captured
        	if (_soldier.currentCapturePoint != null) {
        		if (_soldier.currentCapturePoint.currentlyControllingFaction == _soldier.faction) {
        			doneWithHoldingPosition = true;
        			_soldier.currentCapturePoint = null;
        			return;
        		}
        	}
        	// If just holding position at any other place then check if kill hasn't been gotten for a while
        	if (Time.time - _soldier.timeWhenLastKillOccurred >= _timeWithoutKillsUntilSoldierGetsBoredAndWantsToMoveSomewhereElse) {
        		doneWithHoldingPosition = true;
        		return;
        	}
        	if (!_hasCurrentDestination) {
                AttemptToFindAndSetRandomDestinationWithinAnchorRadius();
            } 
            if (_hasCurrentDestination) {
            	// Check if current destination has been reached
                if (Vector3.Distance(_soldier.soldierTransform.position, _soldier.currentDestination) <= _soldier.distanceToleranceToConsiderDestinationReached) {
                    OnDestinationReached();
                }
            }
            // See if soldier is moving or stuck
            if (Vector3.Distance(_soldier.soldierTransform.position, _soldierLastFramePosition) <= 0.1) {
                _currentNumberOfFramesWithoutMovement += 1;
                if (_currentNumberOfFramesWithoutMovement >= _maximumNumberOfFramesWithoutMovement) {
                    OnDestinationReached();
                }
            } else {
                _currentNumberOfFramesWithoutMovement = 0;
            }
            _soldierLastFramePosition = _soldier.soldierTransform.position;
        }

        public void OnEnter() {
        	Debug.Log("Entered Holding Position State");
        	// Narrow radius if holding capture point
        	if (_soldier.currentCapturePoint != null) {
        		_holdPositionRadius = (float)UtilityFunctions.random.NextDouble() * (5.0f - 1.0f) + 1.0f;
        	} else {
        		_holdPositionRadius = (float)UtilityFunctions.random.NextDouble() * (7.0f - 2.0f) + 2.0f;
        	}
        	_anchorPosition = _soldier.soldierTransform.position;
        	_currentNumberOfFramesWithoutMovement = 0;
        }

        public void OnExit() {
        	// If interrupted by attacking state
        	if (!doneWithHoldingPosition) {
        		_soldier.stateToReturnToAfterExitingAttackingState = this.GetType();
        	} else {
        		_soldier.stateToReturnToAfterExitingAttackingState = null;
        		doneWithHoldingPosition = false;
	        	_hasCurrentDestination = false;
	        	_soldier.currentDestination = _nullDestination;
        	}
        }

        void AttemptToFindAndSetRandomDestinationWithinAnchorRadius() {
            Vector3 randomPoint = _anchorPosition + Random.insideUnitSphere * _holdPositionRadius;
            UnityEngine.AI.NavMeshHit hit;
            if (UnityEngine.AI.NavMesh.SamplePosition(randomPoint, out hit, 1.0f, UnityEngine.AI.NavMesh.AllAreas)) {
                _soldier.currentDestination = hit.position;
                _hasCurrentDestination = true;
                _soldier.agent.SetDestination(_soldier.currentDestination);
            }
        }

        void OnDestinationReached() {
            _hasCurrentDestination = false;
            _soldier.currentDestination = _nullDestination;
        }
    }
}
