using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleSimulator.Core;

namespace BattleSimulator.SoldierScripts
{
    public class MoveRandomly : IState
    {
        public bool reachedDestination;

        private readonly ISoldier _soldier;

        private bool _hasCurrentDestination = false;
        private Vector3 _soldierLastFramePosition;
        private int _currentNumberOfFramesWithoutMovement;
        private int _maximumNumberOfFramesWithoutMovement;
        private Vector3 _nullDestination = Vector3.one * 1000000f;

        public MoveRandomly(ISoldier soldier) {
            _soldier = soldier;
            _maximumNumberOfFramesWithoutMovement = 100;
        }

        public void Tick() {
            if (!_hasCurrentDestination) {
                AttemptToFindAndSetRandomDestination();
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
            Debug.Log("Entered Move Randomly State");
            ResetDestinationVariables();
            _currentNumberOfFramesWithoutMovement = 0;
        }

        public void OnExit() {
            // _soldier.previousStateWasAttacking = false;
            reachedDestination = false;
        }

        void AttemptToFindAndSetRandomDestination() {
            Vector3 randomPoint = _soldier.soldierTransform.position + Random.insideUnitSphere * _soldier.randomMovementDestinationSelectionRadius;
            UnityEngine.AI.NavMeshHit hit;
            if (UnityEngine.AI.NavMesh.SamplePosition(randomPoint, out hit, 1.0f, UnityEngine.AI.NavMesh.AllAreas)) {
                _soldier.currentDestination = hit.position;
                _hasCurrentDestination = true;
                _soldier.agent.SetDestination(_soldier.currentDestination);
            }
        }

        void ResetDestinationVariables() {
            _hasCurrentDestination = false;
            _soldier.currentDestination = _nullDestination;
            reachedDestination = false;
            _currentNumberOfFramesWithoutMovement = 0;
        }

        void OnDestinationReached() {
            _hasCurrentDestination = false;
            _soldier.currentDestination = _nullDestination;
            reachedDestination = true;
        }
    }
}
