using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleSimulator.SpawnPointScripts
{
    public interface ISpawnPoint
    {
        public float spawnTimer { get; set; }
        public string faction { get; set; }
        public Transform spawnLocationTransform { get; set; }
    }
}
