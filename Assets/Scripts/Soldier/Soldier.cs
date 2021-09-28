using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BattleSimulator.Core;
using BattleSimulator.WeaponScripts;
using BattleSimulator.SpawnScripts;
using BattleSimulator.CapturePointScripts;

namespace BattleSimulator.SoldierScripts
{
    public class Soldier : MonoBehaviour, IDamageable
    {

        // PUBLIC

        // Faction
        public string faction;

        // Weapon
        public Transform weaponPositionTransform;
        [Tooltip("Skill represents the probability of landing shots. A value of 1.0 means they land all shots and 0.0 menas they land none.")]
        [Range(0.0f, 1.0f)]
        public float skill;

        // Health
        [Tooltip("The maximum health the soldier could have.")]
        public float startingHealth;
        [Tooltip("The maximum shields the soldier could have.")]
        public float startingShields;
        [Tooltip("The time it takes for shields to begin recharging")]
        public float shieldRechargeDelay;
        [Tooltip("The rate at which shields recharge")]
        public float shieldRechargeRate;

        // Movement
        [Tooltip("The maximum distance a random destination can be selected when the soldier is in the state of moving randomly.")]
        public float randomMovementDestinationSelectionRadius;

        // PRIVATE

        // Game Manager
        private GameObject _gameManager;

        // Faction
        private Color _factionColor;

        // Components
        private NavMeshAgent _agent;
        private Renderer _selfRenderer;
        private Material _selfMaterial;
        private Collider _selfCollider;

        // Health
        private float _currentHealth;
        private float _currentShields;
        private float _shieldRechargeTime;
        private bool _shieldsRechargeDelayActive;
        private bool _isAlive;

        // Movement
        private Vector3 _currentDestination;
        private Queue<Vector3> _destinationQueue;
        private int _randomMovementMaxDestinationQueueSize;
        private Vector3 _lastFramePosition;
        private bool _hasCurrentDestination;
        private bool _isMoving;

        // Weapon and Shooting
        private Weapon _currentWeapon;
        private LayerMask _enemySoldierLayerMask;
        private GameObject _currentTarget;
        private IDamageable _currentTargetIDamageable;
        private Transform _currentTargetTransform;
        private bool _hasCurrentTarget;
        private bool _isShooting;

        // Spawning
        private SpawnPoint _currentSpawnPoint;
        private float _nextSpawnTime;

        // Objectives
        private CapturePoint[] _allCapturePoints;
        private CapturePoint _currentTargetCapturePoint;
        private bool _isIntendingToCapturePoint;

        // Other
        private float _meshRadius;

        void Awake() {
            // Cache components
            _agent = GetComponent<NavMeshAgent>();
            _selfRenderer = GetComponent<Renderer>();
            _selfMaterial = _selfRenderer.material;
            _selfCollider = GetComponent<Collider>();

            // Cache Game Manager
            _gameManager = GameObject.Find("GameManager");

            // Set health
            _currentHealth = startingHealth;
            _currentShields = startingShields;
            _shieldRechargeTime = Time.time;
            _shieldsRechargeDelayActive = false;
            _isAlive = true;

            // Set destination variables
            _destinationQueue = new Queue<Vector3>();
            _randomMovementMaxDestinationQueueSize = 3;
            _hasCurrentDestination = false;
            _isMoving = false;

            // Set weapon variables
            _hasCurrentTarget = false;
            _isShooting = false;

            // Set spawn variables
            _nextSpawnTime = Time.time;

            // Find and cache capture points
            _allCapturePoints = FindObjectsOfType<CapturePoint>();
            _isIntendingToCapturePoint = false;

            // Other
            _meshRadius = transform.localScale.x / 2;
        }

        void Start() {
            // Determine faction related stuff
            switch (faction) {
                case "TR":
                    _selfMaterial.color = Color.red;
                    _factionColor = Color.red;
                    gameObject.layer = LayerMask.NameToLayer("TRSoldier");
                    _enemySoldierLayerMask = LayerMask.GetMask("NCSoldier", "VSSoldier");
                    break;
                case "NC":
                    _selfMaterial.color = Color.blue;
                    _factionColor = Color.blue;
                    gameObject.layer = LayerMask.NameToLayer("NCSoldier");
                    _enemySoldierLayerMask = LayerMask.GetMask("TRSoldier", "VSSoldier");
                    break;
                case "VS":
                    _selfMaterial.color = Color.magenta;
                    _factionColor = Color.magenta;
                    gameObject.layer = LayerMask.NameToLayer("VSSoldier");
                    _enemySoldierLayerMask = LayerMask.GetMask("NCSoldier", "TRSoldier");
                    break; 
            }

            // Select Weapon
            SelectRandomWeapon();
        }

        void Update() {
            if (_isAlive) {
            	// Check if shield recharge delay is up
            	if (Time.time >= _shieldRechargeTime) {
            		_shieldsRechargeDelayActive = false;
            	}

            	// Recharge shields
            	if (_currentShields < startingShields && !_shieldsRechargeDelayActive) {
            		float amountRestored = shieldRechargeRate * Time.deltaTime;
            		if (_currentShields + amountRestored >= startingShields) {
            			_currentShields = startingShields;
            		} else {
            			_currentShields += amountRestored;
            		}
            	}

                // Determine if current target is still alive
                if (_currentTargetIDamageable != null) {
                    if (!_currentTargetIDamageable.IsAlive()) {
                        _hasCurrentTarget = false;
                        _isShooting = false;
                    }
                }   

                // Find target if a current target is not set
                if (!_isShooting && !_hasCurrentTarget) {
                    GameObject potentialTarget = SelectClosestEnemySoldierWithLineOfSight();
                    if (potentialTarget != null) {
                        _hasCurrentTarget = true;
                        _currentTarget = potentialTarget;
                        _currentTargetIDamageable = potentialTarget.GetComponent<IDamageable>();
                        _currentTargetTransform = potentialTarget.transform;
                    }
                }

                // Shoot current target if a current target exists
                if (_hasCurrentTarget) {
                    if (UtilityFunctions.LineOfSightExistsToTarget(transform.position, _currentTargetTransform.position, _enemySoldierLayerMask)) {
                        _isMoving = false;
                        _agent.ResetPath(); // Cancel NavMeshAgent's movement
                        _isShooting = true;
                        transform.LookAt(_currentTargetTransform.position);
                        ShootTarget();
                    } else {
                        _hasCurrentTarget = false;
                        _isShooting = false;
                    }
                }

                // Determine if current target capture point has been flipped yet
                if (_currentTargetCapturePoint != null) {
                    if (_currentTargetCapturePoint.currentlyControllingFaction == faction) {
                        _agent.ResetPath();
                        _hasCurrentDestination = false;
                        _isIntendingToCapturePoint = false;
                    }
                }

                // Set destination to capture points if they are contested otherwise set random destination
                if (!_isShooting && !_isIntendingToCapturePoint) {
                    if (FoundAndTargetedUncontrolledCapturePoint()) {
                        _destinationQueue.Clear();
                        _destinationQueue.Enqueue(_currentTargetCapturePoint.GetPosition());
                    } else {
                        QueueRandomDestination();
                    }
                }

                // Set current destination if available
                if (!_isShooting && !_isMoving && !_hasCurrentDestination && _destinationQueue.Count > 0) {
                    _currentDestination = _destinationQueue.Dequeue();
                    _hasCurrentDestination = true;
                }

                // Move if current destination is set
                if (!_isShooting && !_isMoving && _hasCurrentDestination) {
                    _isMoving = true;
                    StartCoroutine(MoveToCurrentDestination());
                }

                // Determine if destination has been reached
                if (!_isShooting && _hasCurrentDestination) {
                    float tolerance = _meshRadius + 1.0f;
                    // If destination is a capture point then increase tolerance
                    if (_isIntendingToCapturePoint) {
                        tolerance += _currentTargetCapturePoint.meshRadius;
                    }
                    if (Vector3.Distance(transform.position, _currentDestination) <= tolerance) {
                        _agent.ResetPath();
                        _hasCurrentDestination = false;
                    }
                }

                if (transform.position == _lastFramePosition) {
                    _isMoving = false;
                }
                _lastFramePosition = transform.position;

            } else if (!_isAlive) {
                // Try to find spawn point if none available
                if (_currentSpawnPoint == null) {
                    SelectRandomSpawnPoint();
                }

                // If respawn timer is up then spawn in at the currently selected spawn point
                if (Time.time > _nextSpawnTime && _currentSpawnPoint != null) {
                    Respawn();
                }
            }
        }

        void SelectRandomWeapon() {
            List<Weapon> weaponList = _gameManager.GetComponent<WeaponList>().weaponList;
            Weapon weaponPrefab = weaponList[UtilityFunctions.RandInt(0, weaponList.Count)];
            Weapon newWeapon = Instantiate(weaponPrefab, weaponPositionTransform.position, Quaternion.identity, weaponPositionTransform) as Weapon;
            _currentWeapon = newWeapon;
            if (!_isAlive) {
                _currentWeapon.DisableWeapon();
            }
        }

        IEnumerator MoveToCurrentDestination() {
            // This determines a path on the NavMesh to the destination. Call it every 2 seconds instead of every frame
            // to boost performance so that a path is not recalculated each frame
            float refreshRate = 2.0f;
            _agent.SetDestination(_currentDestination);
            yield return new WaitForSeconds(refreshRate);
        }

        void QueueRandomDestination() {
            if (_destinationQueue.Count < _randomMovementMaxDestinationQueueSize) {
                Vector3 randomPoint = transform.position + Random.insideUnitSphere * randomMovementDestinationSelectionRadius;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) {
                    _destinationQueue.Enqueue(hit.position);
                    Debug.DrawRay(hit.position, Vector3.up, Color.blue, 1.0f);
                }
            }
        }

        bool FoundAndTargetedUncontrolledCapturePoint() {
            List<CapturePoint> uncontrolledCapturePoints = new List<CapturePoint>();
            foreach (CapturePoint capturePoint in _allCapturePoints) {
                if (capturePoint.currentlyControllingFaction != faction) {
                    uncontrolledCapturePoints.Add(capturePoint);
                }
            }
            if (uncontrolledCapturePoints.Count > 0) {
                int randomIndex = UtilityFunctions.RandInt(0, uncontrolledCapturePoints.Count);
                _currentTargetCapturePoint = uncontrolledCapturePoints[randomIndex];
                _isIntendingToCapturePoint = true;
                return true;
            } else {
                _isIntendingToCapturePoint = false;
                return false;
            }
        }

        public Vector3 GetPosition() {
            return transform.position;
        }

        GameObject SelectClosestEnemySoldierWithLineOfSight() {
            var allSoldiers = FindObjectsOfType<Soldier>();
            GameObject closestEnemySoldier = null;
            float closestEnemyDistance = 1000000f;
            for (int i = 0; i < allSoldiers.Length; i++) {
                if (allSoldiers[i].faction != faction && allSoldiers[i].IsAlive()) {
                    Vector3 enemySoldierPosition = allSoldiers[i].gameObject.transform.position;
                    Vector3 lineOfSightRayDirection = enemySoldierPosition - transform.position;
                    float lineOfSightRayDistance = lineOfSightRayDirection.magnitude;
                    bool noLineOfSight = Physics.Raycast(transform.position, lineOfSightRayDirection, lineOfSightRayDistance, ~_enemySoldierLayerMask);
                    if (!noLineOfSight) {
                        if (closestEnemySoldier == null || lineOfSightRayDistance < closestEnemyDistance) {
                            closestEnemySoldier = allSoldiers[i].gameObject;
                            closestEnemyDistance = lineOfSightRayDistance;
                        } 
                    }
                }
            }
            return closestEnemySoldier;
        }

        void ShootTarget() {
            bool miss;
            if (UtilityFunctions.random.NextDouble() <= skill) {
                miss = false;
            } else {
                miss = true;
            }
            _currentWeapon.Shoot(_currentTarget, miss);
        }

        public void TakeDamage(float damage) {
        	float damageToHealth = 0;
            if (_currentShields >= damage) {
            	_currentShields -= damage;
            } else if (_currentShields < damage) {
            	damageToHealth = damage - _currentShields;
            	_currentShields = 0;
            	StartCoroutine(ShieldDownBlinkingEffect());
            }
            _shieldRechargeTime = Time.time + shieldRechargeDelay;
            _shieldsRechargeDelayActive = true;
            _currentHealth -= damageToHealth;
            if (_currentHealth <= 0) {
                Die();
            }
        }

        IEnumerator ShieldDownBlinkingEffect() {
        	float blinkingSpeed = 3.0f;
        	float blinkTimer = 0f;
        	float blinkingTime = _shieldRechargeTime - Time.time;
        	while (blinkTimer < blinkingTime) {
        		_selfMaterial.color = Color.Lerp(_factionColor, Color.white, Mathf.PingPong(blinkTimer * blinkingSpeed, 1.0f));
        		blinkTimer += Time.deltaTime;
        		yield return null;
        	}
        	_selfMaterial.color = _factionColor;
        }

        public void Die() {
            _selfRenderer.enabled = false;
            _selfCollider.enabled = false;
            _agent.enabled = false;
            if (_currentWeapon != null) {
                _currentWeapon.DisableWeapon();
            }
            _isAlive = false;

            _destinationQueue.Clear();
            _hasCurrentDestination = false;
            _isMoving = false;
            _isShooting = false;
            _hasCurrentTarget = false;
            _isIntendingToCapturePoint = false;

            SelectRandomSpawnPoint();
            if (_currentSpawnPoint != null) {
                _nextSpawnTime = Time.time + _currentSpawnPoint.spawnTimer;
            }
        }

        public bool IsAlive() {
            if (_isAlive) {
                return true;
            } else {
                return false;
            }
        }

        void SelectRandomSpawnPoint() {
            var allSpawnPoints = FindObjectsOfType<SpawnPoint>();
            List<SpawnPoint> allFriendlySpawnPoints = new List<SpawnPoint>();
            for (int i = 0; i < allSpawnPoints.Length; i++) {
                if (allSpawnPoints[i].faction == faction) {
                    allFriendlySpawnPoints.Add(allSpawnPoints[i]);
                }
            }
            if (allFriendlySpawnPoints.Count > 0) {
                SpawnPoint selectedSpawnPoint = allFriendlySpawnPoints[UtilityFunctions.RandInt(0, allFriendlySpawnPoints.Count)];
                _currentSpawnPoint = selectedSpawnPoint;
            }
        }

        void Respawn() {
            transform.position = _currentSpawnPoint.spawnLocationTransform.position;
            _selfRenderer.enabled = true;
            _selfCollider.enabled = true;
            _agent.enabled = true;
            if (_currentWeapon != null) {
                _currentWeapon.EnableWeapon();
            } else {
                SelectRandomWeapon();
            }
            _isAlive = true;

            _currentHealth = startingHealth;
            _currentShields = startingShields;
            _shieldsRechargeDelayActive = false;
            _shieldRechargeTime = Time.time;
            _lastFramePosition = transform.position;
        }
    }
}
