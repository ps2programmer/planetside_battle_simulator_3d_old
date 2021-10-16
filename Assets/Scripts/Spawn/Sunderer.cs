using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleSimulator.SpawnPointScripts
{
    public class Sunderer: SpawnPoint
    {
        // PUBLIC
        [HideInInspector]
        public Material selfMaterial;

        // PRIVATE
        private Renderer _selfRenderer;

        void Awake() {
            _selfRenderer = GetComponent<Renderer>();
            selfMaterial = _selfRenderer.material;
            spawnTimer = 10.0f;
            spawnLocationTransform = transform.GetChild(0);
        }

        void Start() {
            switch (faction) {
                case "TR":
                    selfMaterial.color = Color.red;
                    break;
                case "NC":
                    selfMaterial.color = Color.blue;
                    break;
                case "VS":
                    selfMaterial.color = Color.magenta;
                    break; 
            }
        }
    }
}