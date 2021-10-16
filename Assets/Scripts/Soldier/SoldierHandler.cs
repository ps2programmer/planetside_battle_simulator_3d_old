using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleSimulator.Core;

namespace BattleSimulator.SoldierScripts
{
    public class SoldierHandler
    {   
        public StateMachine stateMachine { get {return _stateMachine;} }
        public ISoldier soldier { get {return _soldier;} }

        public Dying dying;
        public SelectingSpawnPoint selectingSpawnPoint;
        public Respawning respawning;
        public Attacking attacking;
        public MoveRandomly moveRandomly;
        public DecisionMaking decisionMaking;
        public SelectingCapturePoint selectingCapturePoint;
        public MovingToCapturePoint movingToCapturePoint;
        public HoldingPosition holdingPosition;

        private StateMachine _stateMachine;
        private Dictionary<Type, IState> _dictOfStateInstances;
        private readonly ISoldier _soldier;

        public SoldierHandler(ISoldier soldier, IState initialState) {
            _soldier = soldier;

            _stateMachine = new StateMachine();
            _dictOfStateInstances = new Dictionary<Type, IState>(); // Useful for retrieving the specific instance of the state using the state's type as the key

            // Instantiate all states
            dying = new Dying(_soldier);
            selectingSpawnPoint = new SelectingSpawnPoint(_soldier);
            respawning = new Respawning(_soldier);
            attacking = new Attacking(_soldier);
            moveRandomly = new MoveRandomly(_soldier);
            decisionMaking = new DecisionMaking(_soldier);
            selectingCapturePoint = new SelectingCapturePoint(_soldier);
            movingToCapturePoint = new MovingToCapturePoint(_soldier);
            holdingPosition = new HoldingPosition(_soldier);

            // Add them to the dictionary
            _dictOfStateInstances[dying.GetType()] = dying;
            _dictOfStateInstances[selectingSpawnPoint.GetType()] = selectingSpawnPoint;
            _dictOfStateInstances[respawning.GetType()] = respawning;
            _dictOfStateInstances[decisionMaking.GetType()] = decisionMaking;
            _dictOfStateInstances[moveRandomly.GetType()] = moveRandomly;
            _dictOfStateInstances[attacking.GetType()] = attacking;
            _dictOfStateInstances[selectingCapturePoint.GetType()] = selectingCapturePoint;
            _dictOfStateInstances[movingToCapturePoint.GetType()] = movingToCapturePoint;
            _dictOfStateInstances[holdingPosition.GetType()] = holdingPosition;

            // Create state transitions that can occur from any state
            _stateMachine.AddAnyTransition(dying, () => _soldier.isAlive && _soldier.currentHealth <= 0);
            _stateMachine.AddAnyTransition(attacking, () => _soldier.isAlive && !_soldier.currentlyIgnoringEnemies && _soldier.currentTarget == null && _soldier.SelectClosestEnemySoldierWithLineOfSight());

            // Create all state transitions for state machine that can only occur from certain states
            AddTransition(dying, selectingSpawnPoint, () => dying.readyToSelectSpawnPoint);
            AddTransition(selectingSpawnPoint, respawning, () => selectingSpawnPoint.spawnPointSelected);
            AddTransition(respawning, decisionMaking, () => Time.time >= respawning.spawnTime);
            AddTransition(attacking, decisionMaking, () => !_soldier.currentTarget.IsAlive() || !_soldier.LineOfSightExistsToTarget() || _soldier.currentlyIgnoringEnemies);
            AddTransition(decisionMaking, selectingCapturePoint, () => decisionMaking.decidedToTransitionTo == selectingCapturePoint.GetType());
            AddTransition(selectingCapturePoint, movingToCapturePoint, () => selectingCapturePoint.selectedContestedCapturePoint);
            AddTransition(selectingCapturePoint, moveRandomly, () => selectingCapturePoint.allCapturePointsAreControlled);
            // AddTransition(decisionMaking, movingToCapturePoint, () => decisionMaking.decisionHasBeenMade && decisionMaking.decidedToTransitionTo == movingToCapturePoint.GetType());
            AddTransition(movingToCapturePoint, decisionMaking, () => movingToCapturePoint.capturePointCaptured);
            AddTransition(decisionMaking, moveRandomly, () => decisionMaking.decidedToTransitionTo == moveRandomly.GetType());
            AddTransition(moveRandomly, decisionMaking, () => moveRandomly.reachedDestination);
            AddTransition(movingToCapturePoint, holdingPosition, () => Vector3.Distance(_soldier.soldierTransform.position, _soldier.currentCapturePoint.GetPosition()) <= _soldier.currentCapturePoint.flipRadius - 1.0f);
            AddTransition(holdingPosition, decisionMaking, () => holdingPosition.doneWithHoldingPosition);
            AddTransition(moveRandomly, holdingPosition, () => UtilityFunctions.random.NextDouble() <= 0.001f);

            _stateMachine.SetState(_dictOfStateInstances[initialState.GetType()]);
        }

        void AddTransition(IState from, IState to, Func<bool> condition) {
            _stateMachine.AddTransition(from, to, condition);
        }
    }
}
