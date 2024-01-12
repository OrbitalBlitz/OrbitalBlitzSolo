using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace OrbitalBlitz.Game.Scenes.Circuits.Scripts {
    public class CircuitData : MonoBehaviour {
        public static CircuitData Instance;

        public List<Checkpoint> Checkpoints;
        public List<Spawnpoint> Spawnpoints;
        public int Laps;
        
        
        private void OnEnable() {
            Instance = this;
        }

        
    }
}