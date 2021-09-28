using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleSimulator.Core
{
    public class PauseGame
    {
        // PUBLIC
        public bool gameIsPaused = false;

        public void TogglePauseGame() {
            if (!gameIsPaused) {
                gameIsPaused = true;
                Time.timeScale = 0.0f;
            } else {
                gameIsPaused = false;
                Time.timeScale = 1.0f;
            }
        }
    }
}
