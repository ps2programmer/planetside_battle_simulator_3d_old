using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleSimulator.Core
{
    public interface IDamageable
    {
        public void TakeDamage(float damage);

        public bool IsAlive();
    }
}
