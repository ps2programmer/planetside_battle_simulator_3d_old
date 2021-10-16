using NSubstitute;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleSimulator.SoldierScripts;
using BattleSimulator.Core;

namespace BattleSimulator.Tests
{
    public class SoldierDyingTests
    {
        [Test]
        public void Soldier_Current_State_Set_To_Dying_When_Initialized_With_Dying_State() {
            ISoldier soldierMock = Substitute.For<ISoldier>();
            IState dying = new Dying(soldierMock);
            SoldierHandler soldierHandler = new SoldierHandler(soldierMock, dying);
            Assert.AreEqual(dying.GetType(), soldierHandler.stateMachine.CurrentState.GetType());
        }

        [Test]
        public void Soldier_NavMeshAgentEnabled_Flag_Set_To_False_On_Dying() {
            ISoldier soldierMock = Substitute.For<ISoldier>();
            IState dying = new Dying(soldierMock);
            SoldierHandler soldierHandler = new SoldierHandler(soldierMock, dying);
            Assert.AreEqual(false, soldierHandler.soldier.navMeshAgentEnabled);
        }

        [Test]
        public void Soldier_IsAlive_Flag_Set_To_False_On_Dying() {
            ISoldier soldierMock = Substitute.For<ISoldier>();
            IState dying = new Dying(soldierMock);
            SoldierHandler soldierHandler = new SoldierHandler(soldierMock, dying);
            Assert.AreEqual(false, soldierHandler.soldier.isAlive);
        }

        [Test]
        public void Soldier_RendererEnabled_Flag_Set_To_False_On_Dying() {
            ISoldier soldierMock = Substitute.For<ISoldier>();
            IState dying = new Dying(soldierMock);
            SoldierHandler soldierHandler = new SoldierHandler(soldierMock, dying);
            Assert.AreEqual(false, soldierHandler.soldier.rendererEnabled);
        }

        [Test]
        public void Soldier_WeaponEnabled_Flag_Set_To_False_On_Dying() {
            ISoldier soldierMock = Substitute.For<ISoldier>();
            IState dying = new Dying(soldierMock);
            SoldierHandler soldierHandler = new SoldierHandler(soldierMock, dying);
            Assert.AreEqual(false, soldierHandler.soldier.weaponEnabled);
        }
    }
}