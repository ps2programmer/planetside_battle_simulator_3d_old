using NSubstitute;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleSimulator.SoldierScripts;
using BattleSimulator.Core;
using BattleSimulator.SpawnPointScripts;

namespace BattleSimulator.Tests
{
    public class SoldierSelectingSpawnPointTests
    {
        [Test]
        public void Soldier_Transitions_To_Selecting_Spawn_Point_State_From_Dying_State() {
            ISoldier soldierMock = Substitute.For<ISoldier>();
            SoldierHandler soldierHandler = new SoldierHandler(soldierMock, new Dying(soldierMock));
            soldierHandler.stateMachine.Tick();
            Assert.AreEqual(soldierHandler.stateMachine.CurrentState.GetType(), soldierHandler.selectingSpawnPoint.GetType());
        }

        [Test]
        public void aSoldier_Transitions_To_Selecting_Spawn_Point_State_From_Dying_State() {
            ISoldier soldierMock = Substitute.For<ISoldier>();
            SoldierHandler soldierHandler = new SoldierHandler(soldierMock, new Dying(soldierMock));
            soldierHandler.stateMachine.Tick();
            Assert.AreEqual(soldierHandler.stateMachine.CurrentState.GetType(), soldierHandler.selectingSpawnPoint.GetType());
        }
    }
}
