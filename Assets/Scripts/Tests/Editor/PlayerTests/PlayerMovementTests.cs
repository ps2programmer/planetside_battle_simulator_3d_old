using NSubstitute;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleSimulator.PlayerScripts;

namespace BattleSimulator.Tests
{
    public class PlayerMovementTests 
    {
        [Test]
        public void Movement_Speed_Changes_To_Left_Shift_Held_Speed_When_Left_Shift_Is_Held() {
            PlayerMovementHandler playerMovementHandler = new PlayerMovementHandler();
            float newSpeed = playerMovementHandler.ChangeBetweenLeftShiftHeldSpeedAndNormalSpeed(1.0f, 2.0f, true);
            Assert.AreEqual(2.0f, newSpeed);
        }

        [Test]
        public void Movement_Speed_Changes_To_Normal_Speed_When_Left_Shift_Is_Not_Held_Anymore() {
            PlayerMovementHandler playerMovementHandler = new PlayerMovementHandler();
            float newSpeed = playerMovementHandler.ChangeBetweenLeftShiftHeldSpeedAndNormalSpeed(1.0f, 2.0f, false);
            Assert.AreEqual(1.0f, newSpeed);
        }
    }
}