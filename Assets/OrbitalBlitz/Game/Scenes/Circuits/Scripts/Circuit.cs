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
        public List<RewardCheckpoint> RewardCheckpoints;
        public List<Spawnpoint> Spawnpoints;
        [SerializeField] public List<Phantom> Phantoms;
        public int Laps;
        public long Id; // unix timestamp of creation time
        
        
        private void OnEnable() {
            Instance = this;
        }

        public Checkpoint NthNextCheckpoint(int current_index, int n) {
            return Checkpoints[(current_index + n) % Checkpoints.Count];
        }

        public Checkpoint NthNextCheckpoint(Checkpoint current_cp, int n) {
            return NthNextCheckpoint(Checkpoints.IndexOf(current_cp), n);
        }

        public RewardCheckpoint NthNextRewardCheckpoint(int current_index, int n) {
            return RewardCheckpoints[(current_index + n) % RewardCheckpoints.Count];
        }

        public RewardCheckpoint NthNextRewardCheckpoint(RewardCheckpoint current_cp, int n) {
            return NthNextRewardCheckpoint(RewardCheckpoints.IndexOf(current_cp), n);
        }
        
    }
}