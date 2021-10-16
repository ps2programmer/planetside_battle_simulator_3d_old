using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleSimulator.SoldierScripts;

namespace BattleSimulator.Core
{
    public class StateMachine
    {
        private IState _currentState;
        public IState CurrentState { get {return _currentState;} }

        public Dictionary<Type, List<Transition>> transitions = new Dictionary<Type, List<Transition>>();
        public List<Transition> currentTransitions = new List<Transition>();
        public List<Transition> anyTransitions = new List<Transition>();
        private static List<Transition> EmptyTransitions = new List<Transition>(0);

        public void Tick() {
            var transition = GetTransition();
            if (transition != null) {
                SetState(transition.To);
            }
            _currentState?.Tick();
        }

        public void SetState(IState state) {
            if (state == _currentState) {
                return;
            }
            _currentState?.OnExit();
            _currentState = state;

            transitions.TryGetValue(_currentState.GetType(), out currentTransitions);
            if (currentTransitions == null) {
                currentTransitions = EmptyTransitions;
            }

            _currentState.OnEnter();
        }

        public void AddTransition(IState from, IState to, Func<bool> predicate) {
            if (transitions.TryGetValue(from.GetType(), out var statesToTransitionToList) == false) {
                statesToTransitionToList = new List<Transition>();
                transitions[from.GetType()] = statesToTransitionToList;
            }
            statesToTransitionToList.Add(new Transition(to, predicate));
        }

        public void AddAnyTransition(IState state, Func<bool> predicate) {
            anyTransitions.Add(new Transition(state, predicate));
        }

        private Transition GetTransition() {
            foreach (var transition in anyTransitions) {
                if (transition.Condition()) {
                    return transition;
                }
            }
            foreach (var transition in currentTransitions) {
                if (transition.Condition()) {
                    return transition;
                }
            }
            return null;
        }
    }

    public class Transition {
        public Func<bool> Condition { get; }
        public IState To { get; }
        public Transition(IState to, Func<bool> condition) {
            To = to;
            Condition = condition;
        }

        public override bool Equals(object obj) {
        	var other = obj as Transition;
        	if (other != null) {
        		if (other.To == To && other.Condition == Condition) {
        			return true;
        		}
        	}
        	return false;
        }

        public override int GetHashCode() {
        	return this.GetHashCode();
        }
    }
}
