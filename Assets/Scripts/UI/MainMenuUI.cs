using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleSimulator.Core;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BattleSimulator.UIScripts
{
    public class MainMenuUI : MonoBehaviour
    {
        // PUBLIC
        public Dropdown mapSelectDropdown;
        public Text currentlySelectedMap;

        void Awake() {
            currentlySelectedMap = mapSelectDropdown.captionText;
        }

        public void OnMapSelectDropdownChange() {
            currentlySelectedMap = mapSelectDropdown.captionText;
        }

        public void StartGame() {
            SceneManager.LoadScene(currentlySelectedMap.text);
        }
    }
}
