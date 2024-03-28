#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using OrbitalBlitz.Game.Features.Ship.Controllers;
using OrbitalBlitz.Game.Scenes.Circuits.Scripts;
using OrbitalBlitz.Game.Scenes.Race.Scripts;
using UnityEngine;

namespace OrbitalBlitz.Game.Features.Ship {
    public class PlayerInfo : MonoBehaviour {
        public int lastCheckpoint;
        public float timer;
        public int lap;
        public bool hasFinished = false;

        [SerializeField] OrbitalBlitzPlayer player;

        public event Action<Checkpoint, float> onWrongCheckpointCrossed;
        public event Action<Checkpoint, float> onCorrectCheckpointCrossed;
        public event Action<float> onHasFinished;

        public Collider collider;

        public void Reset() {
            timer = 0f;
            lastCheckpoint = 0;
            lap = 1;
            hasFinished = false;
        }

        private void FixedUpdate() {
            timer += Time.deltaTime;
        }

        public void SetShipCollider(ShipCollider collider) {
            Debug.Log($"setting callback");
            collider.onTrigger += CheckpointCallback;
            collider.onTrigger += FallCatcherCallback;
        }
        
        public void CheckpointCallback(Collider other) {
            if (other.gameObject.TryGetComponent<Checkpoint>(out var checkpoint)) {
                Debug.Log($"{gameObject.name} passed {other.gameObject.name}");
                UpdateShipCheckpointAndLap(checkpoint);
                UpdateHasFinished();
                UpdateShipLastCheckpointPositionAndVelocity();
            }
        }
        
        public void FallCatcherCallback(Collider other) {
            if (other.gameObject.TryGetComponent<FallCatcher>(out var fall_catcher)) {
                Debug.Log($"{gameObject.name} fell !");
                player.RespawnToLastCheckpoint();
            }
        }

        private void UpdateShipLastCheckpointPositionAndVelocity() {
            player.SaveCheckpoint();
        }

        private void UpdateShipCheckpointAndLap(Checkpoint crossedCheckpoint) {
            //Debug.Log("updatePlayerCheckpointAndLap called.");
            int number_of_checkpoints = RaceStateManager.Instance.circuit.Checkpoints.Count;
            int passed_cp = RaceStateManager.Instance.circuit.Checkpoints.IndexOf(crossedCheckpoint);

            // special cases 
            bool passed_last_cp = passed_cp == number_of_checkpoints - 1;
            if (passed_last_cp) {
                //Debug.Log("going through last checkpoint...");

                if (lastCheckpoint == 0) {
                    //Debug.Log("... backward");

                    lastCheckpoint = passed_cp;
                    lap -= 1;

                    onWrongCheckpointCrossed?.Invoke(crossedCheckpoint, timer);

                    return;
                }

                if (lastCheckpoint == number_of_checkpoints - 2) { 
                    //Debug.Log("... forward");

                    lastCheckpoint = passed_cp;

                    onCorrectCheckpointCrossed?.Invoke(crossedCheckpoint, timer);

                    return;
                }
            }
            else if (passed_cp == 0) {
                //Debug.Log("going through first checkpoint...");
                if (lastCheckpoint == number_of_checkpoints - 1) {
                    //Debug.Log("... forwards");

                    lastCheckpoint = passed_cp;
                    lap += 1;
                    
                    onCorrectCheckpointCrossed?.Invoke(crossedCheckpoint, timer);

                    return;
                }

                if (lastCheckpoint == 1) {
                    //Debug.Log("... backward");
                    lastCheckpoint = passed_cp;
                    onWrongCheckpointCrossed?.Invoke(crossedCheckpoint, timer);
                }
            }
            
            // General case
            else {
                //Debug.Log("General case.");
                if (passed_cp == lastCheckpoint + 1 || passed_cp == lastCheckpoint - 1) {
                    lastCheckpoint = passed_cp;
                    onCorrectCheckpointCrossed?.Invoke(crossedCheckpoint, timer);

                    return;
                }
                onWrongCheckpointCrossed?.Invoke(crossedCheckpoint, timer);

            }
            //Debug.Log("Ship " + player.name + " induly passed checkpoint " + crossed_checkpoint.gameObject.name);
        }

        private void UpdateHasFinished() {
            if (lap == RaceStateManager.Instance.circuit.Laps + 1) {
                hasFinished = true;
                onHasFinished?.Invoke(timer);
            }
        }

        #if UNITY_EDITOR
        private void OnDrawGizmos() {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = hasFinished ? UnityEngine.Color.green : UnityEngine.Color.red;
            Handles.Label(transform.position + Vector3.up * 2,
                "cp " + lastCheckpoint.ToString() + ",lap " + lap.ToString(), style);
        }
        #endif
    }
}