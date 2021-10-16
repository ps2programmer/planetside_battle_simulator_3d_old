using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleSimulator.PlayerScripts
{
    public interface IPlayerMover
    {
        public float normalSpeed { get; set; }
        public float leftShiftHeldSpeed { get; set; }
        public float currentSpeed { get; set; }
    }
}
