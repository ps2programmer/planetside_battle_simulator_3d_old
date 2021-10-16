using System;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleSimulator.Core;

namespace BattleSimulator.Tests
{
    public class StateMachineTests
    {
        [Test]
        public void Correct_Key_Is_Added_To_State_Machine_Transitions_Dictionary_When_Add_Transition_Is_Called() {
            StateMachine stateMachine = new StateMachine();
            TestStateFrom fromState = new TestStateFrom();
            TestStateTo toState = new TestStateTo();
            Func<bool> predicate = () => true;
            stateMachine.AddTransition(fromState, toState, predicate);
            Assert.AreEqual(true, stateMachine.transitions.ContainsKey(fromState.GetType()));
        }

        [Test]
        public void Correct_Value_Type_Is_Added_To_Associated_Key_In_State_Machine_Transitions_Dictionary_When_Add_Transition_Is_Called() {
            StateMachine stateMachine = new StateMachine();
            TestStateFrom fromState = new TestStateFrom();
            TestStateTo toState = new TestStateTo();
            Func<bool> predicate = () => true;
            stateMachine.AddTransition(fromState, toState, predicate);
            Assert.AreEqual((new List<Transition>()).GetType(), stateMachine.transitions[fromState.GetType()].GetType());
        }

        [Test]
        public void Correct_Value_Is_Added_To_List_Of_Transitions_Associated_With_Key_When_Add_Transitions_Is_Called() {
            StateMachine stateMachine = new StateMachine();
            TestStateFrom fromState = new TestStateFrom();
            TestStateTo toState = new TestStateTo();
            Func<bool> predicate = () => true;
            stateMachine.AddTransition(fromState, toState, predicate);
            Assert.AreEqual(toState, stateMachine.transitions[fromState.GetType()][0].To);
        }

        [Test]
        public void Correct_Value_Is_Added_To_List_Of_Any_Transitions_When_Add_Any_Transition_Is_Called() {
            StateMachine stateMachine = new StateMachine();
            TestStateFrom anyState = new TestStateFrom();
            Func<bool> predicate = () => true;
            stateMachine.AddAnyTransition(anyState, predicate);
            Assert.AreEqual(anyState, stateMachine.anyTransitions[0].To);
        }

        [Test]
        public void Current_State_Set_Properly_When_Set_State_Is_Called() {
            StateMachine stateMachine = new StateMachine();
            TestStateFrom anyState = new TestStateFrom();
            stateMachine.SetState(anyState);
            Assert.AreEqual(stateMachine.CurrentState, anyState);
        }

        [Test]
        public void Current_Transitions_Set_Properly_When_Set_State_Is_Called() {
            StateMachine stateMachine = new StateMachine();
            TestStateFrom fromState = new TestStateFrom();
            TestStateTo toState = new TestStateTo();
            Func<bool> predicate = () => true;

            Transition transition = new Transition(toState, predicate);
            List<Transition> expectedTransitionList = new List<Transition>();
            expectedTransitionList.Add(transition);

            stateMachine.AddTransition(fromState, toState, predicate);
            stateMachine.SetState(fromState);
            Assert.AreEqual(expectedTransitionList[0], stateMachine.currentTransitions[0]);
        }
    }

    public class TestStateFrom: IState
    {
        public void Tick() {}
        public void OnEnter() {}
        public void OnExit() {}
    }

    public class TestStateTo: IState
    {
        public void Tick() {}
        public void OnEnter() {}
        public void OnExit() {}
    }
}