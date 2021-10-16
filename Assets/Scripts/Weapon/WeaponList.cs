using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleSimulator.WeaponScripts
{
    public class WeaponList: MonoBehaviour
    {
        [SerializeField]
        public List<Weapon> weaponList = new List<Weapon>();
    }
}
