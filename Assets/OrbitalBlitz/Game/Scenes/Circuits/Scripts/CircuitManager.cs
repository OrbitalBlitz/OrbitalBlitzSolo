using System.Collections.Generic;
using UnityEngine;

namespace OrbitalBlitz.Game.Scenes.Circuits.Scripts {
    public class CircuitManager : MonoBehaviour {
        [SerializeField] List<Checkpoint> checkpoints;
        [SerializeField] List<Spawnpoint> spawnpoints;
        [SerializeField] public int Laps;

        public List<Checkpoint> Checkpoints {
            get { return checkpoints; }
        }

        public List<Spawnpoint> Spawnpoints {
            get { return spawnpoints; }
        }

        public static CircuitManager Instance { get; private set; }

        private void Awake() {
            Instance = this;
        }
    }
}