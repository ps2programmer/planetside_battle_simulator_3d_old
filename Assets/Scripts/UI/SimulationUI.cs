using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BattleSimulator.CapturePointScripts;
using UnityEngine.SceneManagement;
using BattleSimulator.PlayerScripts;

namespace BattleSimulator.UIScripts
{
    public class SimulationUI : MonoBehaviour
    {
        // PUBLIC
        public Text scoreText;
        public GameObject helpText;

        // Escape Menu
        public GameObject initialEscapeMenu;
        public GameObject settingsMenu;
        public GameObject soldierSpawnMenu;
        public GameObject cameraSettingsMenu;
        public GameObject sundererSettingsMenu;

        public GameObject currentActiveMenu;
        public bool escapeMenuActive;

        // Player interaction menus
        // When a new one is added make sure to update DisableActivePlayerInteractionMenus()
        // and PlayerInteractionMenuIsActive()
        public bool sundererSettingsMenuActive;

        // PRIVATE
        private float _pointRate;
        private float _scoreTR;
        private float _scoreNC;
        private float _scoreVS;
        private CapturePoint[] _allCapturePoints;
        private bool _helpTextActive;

        void Awake() {
            _pointRate = 1.0f;
            _scoreTR = 0.0f;
            _scoreNC = 0.0f;
            _scoreVS = 0.0f;
            _allCapturePoints = FindObjectsOfType<CapturePoint>();
            escapeMenuActive = false;
            sundererSettingsMenuActive = false;
            _helpTextActive = true;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                ToggleEscapeMenu();
            }
            UpdateScore();
            StartCoroutine(UpdateDisplayOfScore());
        }

        void UpdateScore() {
            foreach (CapturePoint capturePoint in _allCapturePoints) {
                switch (capturePoint.currentlyControllingFaction) {
                    case "TR":
                        _scoreTR += _pointRate * Time.deltaTime;
                        break;
                    case "NC":
                        _scoreNC += _pointRate * Time.deltaTime;
                        break;
                    case "VS":
                        _scoreVS += _pointRate * Time.deltaTime;
                        break;
                }
            }
        }

        IEnumerator UpdateDisplayOfScore() {
            float refreshRate = 1.0f;
            scoreText.text = $"TR: {(int)_scoreTR} NC: {(int)_scoreNC} VS: {(int)_scoreVS}";
            yield return new WaitForSeconds(refreshRate);
        }

        void DisableActivePlayerInteractionMenus() {
            if (sundererSettingsMenuActive) {
                sundererSettingsMenuActive = false;
                sundererSettingsMenu.SetActive(false);
            }
        }

        bool PlayerInteractionMenuIsActive() {
            if (sundererSettingsMenuActive) {
                return true;
            } else {
                return false;
            }
        }

        public bool AnyMenuIsActive() {
            if (escapeMenuActive || PlayerInteractionMenuIsActive()) {
                return true;
            } else {
                return false;
            }
        }

        void ToggleEscapeMenu() {
            if (!AnyMenuIsActive()) {
                initialEscapeMenu.SetActive(true);
                escapeMenuActive = true;
                currentActiveMenu = initialEscapeMenu;
                Cursor.lockState = CursorLockMode.None;
                helpText.SetActive(false);
                _helpTextActive = false;
            } else {
                currentActiveMenu.SetActive(false);
                escapeMenuActive = false;
                currentActiveMenu = null;
                DisableActivePlayerInteractionMenus();
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        // Regular Escape Menu

        public void OnSettingsButtonPressed() {
            initialEscapeMenu.SetActive(false);
            settingsMenu.SetActive(true);
            currentActiveMenu = settingsMenu;
        }

        public void OnMainMenuButtonPressed() {
            SceneManager.LoadScene("MainMenuScene");
        }

        public void OnHelpButtonPressed() {
        	if (!_helpTextActive) {
        		helpText.SetActive(true);
        		_helpTextActive = true;
        	} else {
        		helpText.SetActive(false);
        		_helpTextActive = false;
        	}
        }

        public void OnQuitButtonPressed() {
            Application.Quit();
        }

        public void OnBackToEscapeMenuButtonPressed() {
            settingsMenu.SetActive(false);
            initialEscapeMenu.SetActive(true);
            currentActiveMenu = initialEscapeMenu;
        }

        public void OnSoldierSpawnMenuButtonPressed() {
            soldierSpawnMenu.SetActive(true);
            settingsMenu.SetActive(false);
            currentActiveMenu = soldierSpawnMenu;
        }

        public void OnCameraSettingsButtonPressed() {
            cameraSettingsMenu.SetActive(true);
            settingsMenu.SetActive(false);
            currentActiveMenu = cameraSettingsMenu;
        }

        public void OnBackToSettingsMenuButtonPressed() {
            currentActiveMenu.SetActive(false);
            settingsMenu.SetActive(true);
            currentActiveMenu = settingsMenu;
        }

        // Player Interaction Menus

        public void ToggleSundererSettingsMenu() {
            if (!escapeMenuActive && !sundererSettingsMenuActive) {
                Cursor.lockState = CursorLockMode.None;
                currentActiveMenu = sundererSettingsMenu;
                sundererSettingsMenuActive = true;
                sundererSettingsMenu.SetActive(true);
            } else if (!escapeMenuActive) {
                Cursor.lockState = CursorLockMode.Locked;
                currentActiveMenu = null;
                sundererSettingsMenuActive = false;
                sundererSettingsMenu.SetActive(false);
            }
        }
    }
}
