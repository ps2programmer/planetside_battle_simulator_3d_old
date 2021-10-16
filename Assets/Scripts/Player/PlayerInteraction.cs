using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleSimulator.SpawnPointScripts;
using UnityEngine.UI;
using BattleSimulator.UIScripts;
using BattleSimulator.SoldierScripts;

namespace BattleSimulator
{
    public class PlayerInteraction : MonoBehaviour
    {
        // PUBLIC
        public GameObject gameManager;

        public Sunderer sundererPrefab;
        public LayerMask sundererLayerMask;
        public LayerMask soldierLayerMask;

        // Sunderer Menu
        public Dropdown sundererFactionDropdown;
        public InputField sundererSpawnTimerInputField;

        // Soldier Menu
        public Text soldierKillCountText;
        public Text soldierDeathCountText;
        public Text soldierSkillText;
        public Text soldierCurrentHealthText;
        public Text soldierCurrentShieldsText;

        // PRIVATE
        private SimulationUI simulationUI;
        private Vector3 _positionWhereSundererSettingsMenuWasOpened;
        private Sunderer _selectedSunderer;
        private ISoldier _selectedSoldier;

        void Awake() {
            simulationUI = gameManager.GetComponent<SimulationUI>();
        }

        // Update is called once per frame
        void Update()
        {
            // Sunderer Settings
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
                // Sunderer Settings
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
                // Soldier Settings
                if (Physics.Raycast(transform.position, transform.forward, out hit, 7.0f, soldierLayerMask)) {
                    ISoldier clickedSoldier = hit.collider.gameObject.GetComponent<ISoldier>();
                    if (clickedSoldier == _selectedSoldier) {
                        simulationUI.ToggleSoldierSettingsMenu();
                    }
                    if (_selectedSoldier != null) {
                        _selectedSoldier.selectionIndicator.SetActive(false);
                        _selectedSoldier.isSelected = false;
                        _selectedSoldier = null;
                    } else {
                        simulationUI.ToggleSoldierSettingsMenu();
                    }
                    if (clickedSoldier != _selectedSoldier) {
                        _selectedSoldier = clickedSoldier;
                        _selectedSoldier.selectionIndicator.SetActive(true);
                        _selectedSoldier.isSelected = true;
                    }
                }
            }

            // Update soldier menu in real-time
            if (simulationUI.soldierSettingsMenuActive && _selectedSoldier != null) {
                soldierKillCountText.text = $"Kills: {_selectedSoldier.killCount}";
                soldierDeathCountText.text = $"Deaths: {_selectedSoldier.deathCount}";
                soldierCurrentHealthText.text = $"Current Health: {(int)_selectedSoldier.currentHealth}";
                soldierCurrentShieldsText.text = $"Current Shields: {(int)_selectedSoldier.currentShields}";
                soldierSkillText.text = $"Skill: {_selectedSoldier.skill}";
            } else if (_selectedSoldier != null && !simulationUI.soldierSettingsMenuActive) {
                _selectedSoldier.selectionIndicator.SetActive(false);
                _selectedSoldier.isSelected = false;
                _selectedSoldier = null;
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