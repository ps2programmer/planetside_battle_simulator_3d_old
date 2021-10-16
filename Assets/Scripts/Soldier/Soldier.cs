using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BattleSimulator.Core;
using BattleSimulator.SpawnPointScripts;
using BattleSimulator.WeaponScripts;
using BattleSimulator.CapturePointScripts;

namespace BattleSimulator.SoldierScripts
{
    public class Soldier : MonoBehaviour, IDamageable, ISoldier
    {
        // PUBLIC

        // Components
        [HideInInspector]
        public NavMeshAgent agent { get; set; }
        [HideInInspector]
        public Renderer soldierRenderer { get; set; }
        [HideInInspector]
        public Collider soldierCollider { get; set; }
        [HideInInspector]
        public Transform soldierTransform { get; set; }
        [HideInInspector]
        public bool rendererEnabled { get; set; }
        [HideInInspector]
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
        [HideInInspector]
        public bool isAlive { get; set; }
        public int deathCount { get; set; }

        // Spawning
        [HideInInspector]
        public ISpawnPoint currentSpawnPoint { get; set; }
        [HideInInspector]
        public float nextSpawnTime { get; set; }

        // Weapon
        public Transform weaponPositionTransform { get; set; }
        public List<Weapon> weaponList { get; set; }
        [HideInInspector]
        public Weapon currentWeapon { get; set; }
        public bool weaponEnabled { get; set; }

        // Attacking
        public float skill { get; set; }
        [HideInInspector]
        public IDamageable currentTarget { get; set; }
        public bool previousStateWasAttacking { get; set; }
        public bool currentlyIgnoringEnemies { get; set; }
        public float timeToStopIgnoringEnemies { get; set; }
        public int killCount { get; set; }
        public int currentLifeKillCount { get; set; }
        public float timeWhenLastKillOccurred { get; set; }

        // Movement
        [HideInInspector]
        public Vector3 currentDestination { get; set; }
        [HideInInspector]
        public float distanceToleranceToConsiderDestinationReached { get; set; }
        [HideInInspector]
        public float currentDistanceToleranceToConsiderDestinationReached { get; set; }
        public float randomMovementDestinationSelectionRadius { get; set; }
        public bool navMeshAgentEnabled { get; set; }

        // Objectives
        public CapturePoint currentCapturePoint { get; set; }

        // Decision Making
        // public StateMachine stateMachine { get {return _stateMachine;} }
        public float probabilityOfCaringAboutCapturePoint { get; set; }
        public float probabilityOfEngagingEnemyTarget { get; set; }
        public Type stateToReturnToAfterExitingAttackingState { get; set; }

        // Selection By Player
        public bool isSelected { get; set; }
        public GameObject selectionIndicator { get; set; }

        // PRIVATE
        // private StateMachine _stateMachine;

        // Enemies
        private LayerMask _enemySoldierLayerMask;

        // Movement
        private float _meshRadius;

        // Logic Handler
        private SoldierHandler _soldierHandler;

        void Awake() {
            // Cache components
            agent = GetComponent<NavMeshAgent>();
            soldierRenderer = GetComponent<Renderer>();
            soldierCollider = GetComponent<Collider>();
            soldierTransform = GetComponent<Transform>();

            // Instantiate health variables
            startingHealth = 100f;
            startingShields = 100f;
            shieldRechargeDelay = 5.0f;
            shieldRechargeRate = 20f;

            // Instantiate weapon variables
            weaponList = GetComponent<WeaponList>().weaponList;
            weaponPositionTransform = transform.Find("WeaponPosition");

            // Instantiate movement variables
            _meshRadius = transform.localScale.x / 2;
            distanceToleranceToConsiderDestinationReached = _meshRadius + 1.0f;
            currentDistanceToleranceToConsiderDestinationReached = distanceToleranceToConsiderDestinationReached;
            randomMovementDestinationSelectionRadius = 50.0f;

            // Instantiate decision-making variables
            probabilityOfCaringAboutCapturePoint = (float)UtilityFunctions.random.NextDouble() * (0.6f - 0.0f);
            probabilityOfEngagingEnemyTarget = (float)UtilityFunctions.random.NextDouble() * (0.999f - 0.95f) + 0.95f;
            currentlyIgnoringEnemies = false;

            // Other variab;es
            selectionIndicator = transform.Find("SelectionIndicator").gameObject;

            _soldierHandler = new SoldierHandler(this, new Dying(this));
        }

        void Start() {
            // Determine faction related stuff
            switch (faction) {
                case "TR":
                    soldierRenderer.material.color = Color.red;
                    factionColor = Color.red;
                    gameObject.layer = LayerMask.NameToLayer("TRSoldier");
                    _enemySoldierLayerMask = LayerMask.GetMask("NCSoldier", "VSSoldier");
                    break;
                case "NC":
                    soldierRenderer.material.color = Color.blue;
                    factionColor = Color.blue;
                    gameObject.layer = LayerMask.NameToLayer("NCSoldier");
                    _enemySoldierLayerMask = LayerMask.GetMask("TRSoldier", "VSSoldier");
                    break;
                case "VS":
                    soldierRenderer.material.color = Color.magenta;
                    factionColor = Color.magenta;
                    gameObject.layer = LayerMask.NameToLayer("VSSoldier");
                    _enemySoldierLayerMask = LayerMask.GetMask("NCSoldier", "TRSoldier");
                    break;
            }
        }

        public void Update() {
            if (isAlive) {
                // SHIELDS
                // Recharge shields
                if (currentShields < startingShields && Time.time >= shieldRechargeDelayIsDoneTime) {
                    float amountRestored = shieldRechargeRate * Time.deltaTime;
                    if (currentShields + amountRestored >= startingShields) {
                        currentShields = startingShields;
                    } else {
                        currentShields += amountRestored;
                    }
                }
            }

            _soldierHandler.stateMachine.Tick();
        }

        public void TakeDamage(float damage, Projectile projectile) {
            float damageToHealth = 0;
            if (currentShields >= damage) {
                currentShields -= damage;
            } else if (currentShields < damage) {
                damageToHealth = damage - currentShields;
                currentShields = 0;
                StartCoroutine(ShieldDownBlinkingEffect());
            }
            shieldRechargeDelayIsDoneTime = Time.time + shieldRechargeDelay;
            currentHealth -= damageToHealth;
            if (currentHealth <= 0) {
                projectile.soldier.killCount++;
                projectile.soldier.currentLifeKillCount++;
                projectile.soldier.timeWhenLastKillOccurred = Time.time;
            }
        }

        public bool IsAlive() {
            return isAlive;
        }

        public Transform GetTransform() {
            return transform;
        }

        IEnumerator ShieldDownBlinkingEffect() {
            float blinkingSpeed = 3.0f;
            float blinkTimer = 0f;
            float blinkingTime = shieldRechargeDelayIsDoneTime - Time.time;
            while (blinkTimer < blinkingTime) {
                soldierRenderer.material.color = Color.Lerp(factionColor, Color.white, Mathf.PingPong(blinkTimer * blinkingSpeed, 1.0f));
                blinkTimer += Time.deltaTime;
                if (!isAlive) {
                    yield break;
                }
                yield return null;
            }
            soldierRenderer.material.color = factionColor;
        }

        public bool SelectClosestEnemySoldierWithLineOfSight() {
            var allSoldiers = FindObjectsOfType<Soldier>();
            Soldier closestEnemySoldier = null;
            float closestEnemyDistance = 1000000f;
            float yAxisAngleFieldOfViewLimit = 180.0f / 2;
            float xAxisAngleFieldOfViewLimit = 140.0f / 2;
            // For debugging purposes, to be able to see limits of field of view if uncommented
            // Debug.DrawRay(transform.position, transform.forward * 100.0f, Color.green, 0.01f);
            // Quaternion bottomLeftCornerOfFOVLineXRotation = Quaternion.AngleAxis(-xAxisAngleFieldOfViewLimit, transform.up);
            // Quaternion bottomLeftCornerOfFOVLineYRotation = Quaternion.AngleAxis(yAxisAngleFieldOfViewLimit, transform.right);
            // Vector3 bottomLeftCornerOfFOV = bottomLeftCornerOfFOVLineXRotation * transform.forward;
            // bottomLeftCornerOfFOV = bottomLeftCornerOfFOVLineYRotation * bottomLeftCornerOfFOV;
            // Debug.DrawRay(transform.position, bottomLeftCornerOfFOV * 100.0f, Color.blue, 0.01f);
            // Quaternion bottomRightCornerOfFOVLineXRotation = Quaternion.AngleAxis(xAxisAngleFieldOfViewLimit, transform.up);
            // Quaternion bottomRightCornerOfFOVLineYRotation = Quaternion.AngleAxis(yAxisAngleFieldOfViewLimit, transform.right);
            // Vector3 bottomRightCornerOfFOV = bottomRightCornerOfFOVLineXRotation * transform.forward;
            // bottomRightCornerOfFOV = bottomRightCornerOfFOVLineYRotation * bottomRightCornerOfFOV;
            // Debug.DrawRay(transform.position, bottomRightCornerOfFOV * 100.0f, Color.blue, 0.01f);
            // Quaternion topLeftCornerOfFOVLineXRotation = Quaternion.AngleAxis(-xAxisAngleFieldOfViewLimit, transform.up);
            // Quaternion topLeftCornerOfFOVLineYRotation = Quaternion.AngleAxis(-yAxisAngleFieldOfViewLimit, transform.right);
            // Vector3 topLeftCornerOfFOV = topLeftCornerOfFOVLineXRotation * transform.forward;
            // topLeftCornerOfFOV = topLeftCornerOfFOVLineYRotation * topLeftCornerOfFOV;
            // Debug.DrawRay(transform.position, topLeftCornerOfFOV * 100.0f, Color.blue, 0.01f);
            // Quaternion topRightCornerOfFOVLineXRotation = Quaternion.AngleAxis(xAxisAngleFieldOfViewLimit, transform.up);
            // Quaternion topRightCornerOfFOVLineYRotation = Quaternion.AngleAxis(-yAxisAngleFieldOfViewLimit, transform.right);
            // Vector3 topRightCornerOfFOV = topRightCornerOfFOVLineXRotation * transform.forward;
            // topRightCornerOfFOV = topRightCornerOfFOVLineYRotation * topRightCornerOfFOV;
            // Debug.DrawRay(transform.position, topRightCornerOfFOV * 100.0f, Color.blue, 0.01f);
            for (int i = 0; i < allSoldiers.Length; i++) {
                if (allSoldiers[i].faction != faction && allSoldiers[i].IsAlive()) {
                    Vector3 enemySoldierPosition = allSoldiers[i].GetTransform().position;
                    Vector3 lineOfSightRayDirection = enemySoldierPosition - transform.position;
                    float lineOfSightRayDistance = lineOfSightRayDirection.magnitude;
                    // Determine if within field of vision
                    float xAngle = Vector3.SignedAngle(transform.forward, lineOfSightRayDirection, transform.up);
                    float yAngle = Vector3.SignedAngle(transform.forward, lineOfSightRayDirection, transform.right);
                    if (xAngle >= -xAxisAngleFieldOfViewLimit && xAngle <= xAxisAngleFieldOfViewLimit && yAngle >= -yAxisAngleFieldOfViewLimit && yAngle <= yAxisAngleFieldOfViewLimit) {
                        // Debug.DrawRay(transform.position, lineOfSightRayDirection * lineOfSightRayDistance, Color.red, 0.01f);
                        bool noLineOfSight = Physics.Raycast(transform.position, lineOfSightRayDirection, lineOfSightRayDistance, ~_enemySoldierLayerMask);
                        if (!noLineOfSight) {
                            if (closestEnemySoldier == null || lineOfSightRayDistance < closestEnemyDistance) {
                                closestEnemySoldier = allSoldiers[i];
                                closestEnemyDistance = lineOfSightRayDistance;
                            } 
                        }
                    }
                }
            }
            if (closestEnemySoldier == null) {
                ClearCurrentTarget();
                return false;
            } else {
            	currentTarget = closestEnemySoldier;
                return true;
            }
        }

        public bool LineOfSightExistsToTarget() {
            if (currentTarget != null) {
                Vector3 rayDirection = currentTarget.GetTransform().position - transform.position;
                float rayDistance = rayDirection.magnitude;
                bool noLineOfSight = Physics.Raycast(transform.position, rayDirection, rayDistance, ~_enemySoldierLayerMask);
                return !noLineOfSight;
            }
            ClearCurrentTarget();
            return false;
        }

        public void StartIgnoreEnemiesCoroutine(int timeToIgnoreEnemiesFor) {
            StartCoroutine(IgnoreEnemiesForTime(timeToIgnoreEnemiesFor));
        }

        public IEnumerator IgnoreEnemiesForTime(int timeToIgnoreEnemiesFor) {
            currentlyIgnoringEnemies = true;
            yield return new WaitForSeconds(timeToIgnoreEnemiesFor);
            currentlyIgnoringEnemies = false;
        }

        public void DisableNavMeshAgent() {
        	agent.enabled = false;
        	navMeshAgentEnabled = false;
        }

        public void EnableNavMeshAgent() {
        	agent.enabled = true;
        	navMeshAgentEnabled = true;
        }

        public void DisableRenderer() {
        	soldierRenderer.enabled = false;
        	rendererEnabled = false;
        }

        public void EnableRenderer() {
        	soldierRenderer.enabled = true;
        	rendererEnabled = true;
        }

        public void DisableCollider() {
        	soldierCollider.enabled = false;
        	colliderEnabled = false;
        }

        public void EnableCollider() {
        	soldierCollider.enabled = true;
        	colliderEnabled = true;
        }

        public void DisableWeapon() {
        	currentWeapon.DisableWeapon();
        	weaponEnabled = false;
        }

        public void EnableWeapon() {
        	currentWeapon.EnableWeapon();
        	weaponEnabled = true;
        }

        public void ClearCurrentTarget() {
        	currentTarget = null;
        }

        public void EndShieldRechargeDelay() {
        	shieldRechargeDelayIsDoneTime = Time.time;
        }

        public void ReplenishHealthAndShields() {
            currentHealth = startingHealth;
            currentShields = startingShields;
        }

        public void IncrementKillCount() {
            killCount++;
        }
    }
}
