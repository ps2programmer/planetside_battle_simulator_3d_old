using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleSimulator.SpawnScripts;
using UnityEngine.UI;
using BattleSimulator.UIScripts;

namespace BattleSimulator
{
    public class PlayerInteraction : MonoBehaviour
    {
        // PUBLIC
        public GameObject gameManager;

        public Sunderer sundererPrefab;
        public LayerMask sundererLayerMask;

        public Dropdown sundererFactionDropdown;
        public InputField sundererSpawnTimerInputField;

        // PRIVATE
        private SimulationUI simulationUI;
        private Vector3 _positionWhereSundererSettingsMenuWasOpened;
        private Sunderer _selectedSunderer;

        void Awake() {
            simulationUI = gameManager.GetComponent<SimulationUI>();
        }

        // Update is called once per frame
        void Update()
        {
            // If player moves far away from sunderer then the menu closes automatically
            if (simulationUI.sundererSettingsMenuActive) {
                if (Vector3.Distance(transform.position, _positionWhereSundererSettingsMenuWasOpened) >= 10.0f) {
                    simulationUI.ToggleSundererSettingsMenu();
                }
            }

            if ((Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) && !simulationUI.AnyMenuIsActive()) {
                Sunderer newSunderer = Instantiate(sundererPrefab, transform.position, Quaternion.identity) as Sunderer;
            }

            if (Input.GetMouseButtonDown(0)) {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, 5.0f, sundererLayerMask)) {
                    if (!simulationUI.sundererSettingsMenuActive) {
                        simulationUI.ToggleSundererSettingsMenu();
                        _positionWhereSundererSettingsMenuWasOpened = transform.position;
                        _selectedSunderer = hit.collider.gameObject.GetComponent<Sunderer>();
                        switch (_selectedSunderer.faction) {
                            case "Neutral":
                                sundererFactionDropdown.value = 0;
                                break;
                            case "TR":
                                sundererFactionDropdown.value = 1;
                                break;
                            case "NC":
                                sundererFactionDropdown.value = 2;
                                break;
                            case "VS":
                                sundererFactionDropdown.value = 3;
                                break;
                        }
                        sundererSpawnTimerInputField.text = Convert.ToString(_selectedSunderer.spawnTimer);
                    }
                }
            }
        }

        public void OnSundererDropdownMenuUpdated() {
            if (_selectedSunderer != null) {
                string selectedFaction = sundererFactionDropdown.captionText.text;
                _selectedSunderer.faction = selectedFaction;
                switch (selectedFaction) {
                    case "Neutral":
                        _selectedSunderer.selfMaterial.color = Color.yellow;
                        break;
                    case "TR":
                        _selectedSunderer.selfMaterial.color = Color.red;
                        break;
                    case "NC":
                        _selectedSunderer.selfMaterial.color = Color.blue;
                        break;
                    case "VS":
                        _selectedSunderer.selfMaterial.color = Color.magenta;
                        break;
                }
            }
        }

        public void OnSundererSpawnTimerInputFieldEdited() {
            if (_selectedSunderer != null) {
                float newSpawnTime = float.Parse(sundererSpawnTimerInputField.text);
                _selectedSunderer.spawnTimer = newSpawnTime;
            }
        }
    }
}
