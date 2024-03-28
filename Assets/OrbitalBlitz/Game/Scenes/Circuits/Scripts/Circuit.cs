using System;
using System.Collections.Generic;
using Unity.Barracuda;
using UnityEngine;
using UnityEngine.Serialization;

namespace OrbitalBlitz.Game.Scenes.Circuits.Scripts {
    
    public class Circuit : MonoBehaviour {
        public static Circuit Instance;
        
        public enum MedalType {
            Gold,
            Silver,
            Bronze,
            NoMedal
        }
        
        [Serializable]
        public struct Phantom {
            public NNModel Model;
            public MedalType Medal;
        }

        public List<Checkpoint> Checkpoints;
        public List<Spawnpoint> Spawnpoints;
        [SerializeField] public List<Phantom> Phantoms;
        public int Laps;
        public long Id; // unix timestamp of creation time
        
        
        private void OnEnable() {
            Instance = this;
        }

        
    }
}