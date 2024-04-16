using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Barracuda;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace OrbitalBlitz.Game.Scenes.Circuits.Scripts {
    
    public class Circuit : MonoBehaviour {
        [SerializeField] private bool draw_gizmos = false;
        public static Circuit Instance;
        
        public enum MedalType {
            NoMedal,
            Gold,
            Silver,
            Bronze
        }
        
        [SerializeField] public static Color GoldColor = new (255, 243, 100);
        [SerializeField] public static Color SilverColor = new (191, 191, 191);
        [SerializeField] public static Color BronzeColor = new (166, 81, 0);
        [SerializeField] public static Color DefaultColor = new (84, 84, 84);
        
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
        
        // private List<Loader.CircuitInfo> getCircuitInfo() {
        //     var circuits = Loader.Circuits;
        //
        //     var circuits_with_records = circuits
        //         .Select((c) => new Loader.CircuitInfo() {
        //             Name = c.Name,
        //             Id = c.Id,
        //             Scene = c.Scene,
        //             PersonalBests = UserSession.Instance.GetPlayerRecords(c.Id).Result,
        //             Medal = UserSession.Instance.GetPlayerMedal(c.Id).Result,
        //             WorldBests = UserSession.Instance.GetWorldRecords(c.Id).Result
        //         }).ToList();
        //
        //     return circuits_with_records;
        // }
        
        #if UNITY_EDITOR
        private void OnDrawGizmos() {
            if (!draw_gizmos) return;
            
            GUIStyle style = new ();
            style.normal.textColor = Color.magenta;
            
            foreach (var cp in Checkpoints) {
                Handles.Label(cp.transform.position + Vector3.up * 2, Checkpoints.IndexOf(cp).ToString());
            }
            
            foreach (var sp in Spawnpoints) {
                Handles.Label(sp.transform.position + Vector3.up * 2, "SPAWN");
            }
            
        }
        #endif
        
    }
}