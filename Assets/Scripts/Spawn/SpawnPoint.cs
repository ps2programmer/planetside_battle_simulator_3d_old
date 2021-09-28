using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleSimulator.SpawnScripts
{
    public abstract class SpawnPoint : MonoBehaviour
    {
        // PUBLIC
        public Transform spawnLocationTransform;
        public string faction;
        public float spawnTimer;
    }
}
