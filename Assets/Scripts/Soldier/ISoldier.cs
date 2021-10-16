using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BattleSimulator.SpawnPointScripts;
using BattleSimulator.Core;
using BattleSimulator.CapturePointScripts;
using BattleSimulator.WeaponScripts;

namespace BattleSimulator.SoldierScripts
{
    public interface ISoldier
    {
        // Cached components
        public NavMeshAgent agent { get; set; }
        public Renderer soldierRenderer { get; set; }
        public Collider soldierCollider { get; set; }
        public Transform soldierTransform { get; set; }
        public bool rendererEnabled { get; set; }
        public bool colliderEnabled { get; set; }

        // Faction
        public string faction { get; set; }
        public Color factionColor { get; set; }

        // Health
        public float startingHealth { get; set; }
        public float currentHealth { get; set; }
        public float startingShields { get; set; }
        public float currentShields { get; set; }
        public float shieldRechargeDelay { get; set; }
        public float shieldRechargeRate { get; set; }
        public float shieldRechargeDelayIsDoneTime { get; set; }
        public bool isAlive { get; set; }
        public int deathCount { get; set; }

        // Spawning
        public ISpawnPoint currentSpawnPoint { get; set; }
        public float nextSpawnTime { get; set; }

        // Weapon
        public Transform weaponPositionTransform { get; set; }
        public Weapon currentWeapon { get; set; }
        public List<Weapon> weaponList { get; set; }
        public bool weaponEnabled { get; set; }

        // Attacking
        public float skill { get; set; }
        public IDamageable currentTarget { get; set; }
        public bool currentlyIgnoringEnemies { get; set; }
        public int killCount { get; set; }
        public int currentLifeKillCount { get; set; }
        public float timeWhenLastKillOccurred { get; set; }

        // Movement
        public Vector3 currentDestination { get; set; }
        public float distanceToleranceToConsiderDestinationReached { get; set; }
        public float currentDistanceToleranceToConsiderDestinationReached { get; set; }
        public float randomMovementDestinationSelectionRadius { get; set; }
        public bool navMeshAgentEnabled { get; set; }

        // Objectives
        public CapturePoint currentCapturePoint { get; set; }

        // Decision Making
        public float probabilityOfCaringAboutCapturePoint { get; set; }
        public float probabilityOfEngagingEnemyTarget { get; set; }
        public Type stateToReturnToAfterExitingAttackingState { get; set; }

        // Selection By Player
        public bool isSelected { get; set; }
        public GameObject selectionIndicator { get; set; }

        public bool SelectClosestEnemySoldierWithLineOfSight();
        public bool LineOfSightExistsToTarget();
        public void DisableNavMeshAgent();
        public void EnableNavMeshAgent();
        public void DisableRenderer();
        public void EnableRenderer();
        public void DisableCollider();
        public void EnableCollider();
        public void DisableWeapon();
        public void EnableWeapon();
        public void ClearCurrentTarget();
        public void EndShieldRechargeDelay();
        public void ReplenishHealthAndShields();
        public void StartIgnoreEnemiesCoroutine(int timeToIgnoreEnemiesFor);
        public IEnumerator IgnoreEnemiesForTime(int timeToIgnoreEnemiesFor);
        public void IncrementKillCount();
    }
}
