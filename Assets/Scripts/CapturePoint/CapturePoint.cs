using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleSimulator.SoldierScripts;

namespace BattleSimulator.CapturePointScripts
{
    public class CapturePoint : MonoBehaviour
    {
        // PUBLIC
        [HideInInspector]
        public string currentlyControllingFaction;
        [Tooltip("How quickly the capture point changes hands.")]
        public float flipRate;
        [Tooltip("The time it takes for the capture point to change from enemy to neutral, or neutral to friendly.")]
        public float flipTimer;
        [Tooltip("The radius within which the capture point can be flipped.")]
        public float flipRadius;
        [HideInInspector]
        public float meshRadius;

        // PRIVATE
        private Renderer _selfRenderer;
        private Material _selfMaterial;
        private string _currentMajorityFactionInRadius;
        private float _flipProgress;

        void Awake() {
            // Cache components
            _selfRenderer = GetComponent<Renderer>();
            _selfMaterial = _selfRenderer.material;

            meshRadius = transform.localScale.x / 2;

            _currentMajorityFactionInRadius = "Neutral";
            _flipProgress = 0f;
        }

        // Start is called before the first frame update
        void Start()
        {
            switch (currentlyControllingFaction) {
                case "Neutral":
                    _selfMaterial.color = Color.yellow;
                    break;
                case "TR":
                    _selfMaterial.color = Color.red;
                    break;
                case "NC":
                    _selfMaterial.color = Color.blue;
                    break;
                case "VS":
                    _selfMaterial.color = Color.magenta;
                    break; 
            }
        }

        // Update is called once per frame
        void Update()
        {
            CalculateMajorityFactionInRadius();
            if (_currentMajorityFactionInRadius != currentlyControllingFaction && _currentMajorityFactionInRadius != "Neutral") {
                _flipProgress += flipRate * Time.deltaTime;
                if (_flipProgress >= flipTimer) {
                    if (currentlyControllingFaction != "Neutral") {
                        currentlyControllingFaction = "Neutral";
                        _selfMaterial.color = Color.yellow;
                    } else {
                        currentlyControllingFaction = _currentMajorityFactionInRadius;
                        switch (_currentMajorityFactionInRadius) {
                            case "TR":
                                _selfMaterial.color = Color.red;
                                break;
                            case "NC":
                                _selfMaterial.color = Color.blue;
                                break;
                            case "VS":
                                _selfMaterial.color = Color.magenta;
                                break;
                        }
                    }
                    _flipProgress = 0;
                }
            }
        }

        void CalculateMajorityFactionInRadius() {
            var allSoldiers = FindObjectsOfType<Soldier>();
            int numberOfTR = 0;
            int numberOfNC = 0;
            int numberOfVS = 0;
            foreach (Soldier soldier in allSoldiers) {
                if (soldier.isAlive && Vector3.Distance(soldier.soldierTransform.position, transform.position) <= flipRadius) {
                    switch (soldier.faction) {
                    case "TR":
                        numberOfTR++;
                        break;
                    case "NC":
                        numberOfNC++;
                        break;
                    case "VS":
                        numberOfVS++;
                        break;
                    }
                }
            }
            string currentStatus = "Neutral";
            if (numberOfTR > numberOfVS && numberOfTR > numberOfNC) {
                currentStatus = "TR";
            } else if (numberOfNC > numberOfVS && numberOfNC > numberOfTR) {
                currentStatus = "NC";
            } else if (numberOfVS > numberOfNC && numberOfVS > numberOfTR) {
                currentStatus = "VS";
            } else if ((numberOfVS == numberOfTR && numberOfVS != 0) || (numberOfVS == numberOfNC && numberOfVS != 0) || (numberOfTR == numberOfNC && numberOfTR != 0)) {
                currentStatus = "Neutral";
            }
            switch (currentStatus) {
                case "Neutral":
                    _currentMajorityFactionInRadius = "Neutral";
                    break;
                case "TR":
                    _currentMajorityFactionInRadius = "TR";
                    break;
                case "NC":
                    _currentMajorityFactionInRadius = "NC";
                    break;
                case "VS":
                    _currentMajorityFactionInRadius = "VS";
                    break;
            }
        }

        public Vector3 GetPosition() {
            return transform.position;
        }
    }
}