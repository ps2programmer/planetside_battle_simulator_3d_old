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
        private MouseLook mouseLookScript;
        private PlayerMovement playerMovementScript;

        void Awake() {
            mouseLookScript = GetComponent<MouseLook>();
            playerMovementScript = GetComponent<PlayerMovement>();
        }

        public void OnCameraBaseMovementSpeedInputFieldEdited() {
            string fieldText = cameraBaseMovementSpeedInputField.text;
            if (fieldText != "") {
                playerMovementScript.movementSpeed = float.Parse(fieldText);
            } else {
                playerMovementScript.movementSpeed = 10.0f;
            }
        }

        public void OnCameraShiftHeldMovementSpeedInputFieldEdited() {
            string fieldText = cameraShiftHeldMovementSpeedInputField.text;
            if (fieldText != "") {
                playerMovementScript.sprintSpeed = float.Parse(fieldText);
            } else {
                playerMovementScript.sprintSpeed = 20.0f;
            }
        }

        public void OnCameraMouseSensitivityInputFieldEdited() {
            string fieldText = cameraMouseSensitivityInputField.text;
            if (fieldText != "") {
                mouseLookScript.sensitivity = float.Parse(fieldText);
            } else {
                mouseLookScript.sensitivity = 100.0f;
            }
        }
    }
}
