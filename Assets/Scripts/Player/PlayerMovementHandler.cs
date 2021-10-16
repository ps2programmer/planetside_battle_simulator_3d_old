using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleSimulator.PlayerScripts
{
    public class PlayerMovementHandler
    {
        public float ChangeBetweenLeftShiftHeldSpeedAndNormalSpeed(float normalSpeed, float leftShiftHeldSpeed, bool toggleToLeftShiftHeldSpeed) {
            if (toggleToLeftShiftHeldSpeed) {
                return leftShiftHeldSpeed;
            } else {
                return normalSpeed;
            }
        }

        public Vector3 CalculateMovement(float x, float y, float z, Vector3 forwardVector, Vector3 rightVector, float maxYMovementDistance) {
            Vector3 move = (forwardVector * z) + (rightVector * x);
            move.y += y;
            move.y = Mathf.Clamp(move.y, -maxYMovementDistance, maxYMovementDistance);
            return move;
        }
    }
}
