using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BattleSimulator.PlayerScripts
{
    public class PlayerSettingsMenu : MonoBehaviour
    {
        // PUBLIC
        public InputField cameraBaseMovementSpeedInputField;
        public InputField cameraShiftHeldMovementSpeedInputField;
        public InputField cameraMouseSensitivityInputField;

        // PRIVATE
        private MouseLook _mouseLookScript;
        private PlayerMover _playerMoverScript;

        void Awake() {
            _mouseLookScript = GetComponent<MouseLook>();
            _playerMoverScript = GetComponent<PlayerMover>();
        }

        public void OnCameraBaseMovementSpeedInputFieldEdited() {
            string fieldText = cameraBaseMovementSpeedInputField.text;
            if (fieldText != "") {
                _playerMoverScript.normalSpeed = float.Parse(fieldText);
            } else {
                _playerMoverScript.normalSpeed = 40.0f;
            }
            _playerMoverScript.updateSpeed = true;
        }

        public void OnCameraShiftHeldMovementSpeedInputFieldEdited() {
            string fieldText = cameraShiftHeldMovementSpeedInputField.text;
            if (fieldText != "") {
                _playerMoverScript.leftShiftHeldSpeed = float.Parse(fieldText);
            } else {
                _playerMoverScript.leftShiftHeldSpeed = 60.0f;
            }
            _playerMoverScript.updateSpeed = true;
        }

        public void OnCameraMouseSensitivityInputFieldEdited() {
            string fieldText = cameraMouseSensitivityInputField.text;
            if (fieldText != "") {
                _mouseLookScript.sensitivity = float.Parse(fieldText);
            } else {
                _mouseLookScript.sensitivity = 100.0f;
            }
        }
    }
}