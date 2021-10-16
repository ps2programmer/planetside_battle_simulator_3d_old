using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleSimulator.Core
{
    public interface IState
    {
        void Tick();
        void OnEnter();
        void OnExit();
    }
}
