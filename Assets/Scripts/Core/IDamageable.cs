using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleSimulator.WeaponScripts;

namespace BattleSimulator.Core
{
    public interface IDamageable
    {
    	public Transform GetTransform();
        public void TakeDamage(float damage, Projectile projectile);
        public bool IsAlive();
    }
}
