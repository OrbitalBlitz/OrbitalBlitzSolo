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
        public int lastRewardCheckpoint;
        public float timer;
        public int lap;
        public bool hasFinished = false;
        public Circuit.MedalType wonMedal = Circuit.MedalType.NoMedal;

        [SerializeField] OrbitalBlitzPlayer player;

        public event Action<Checkpoint, float> onWrongCheckpointCrossed;
        public event Action<Checkpoint, float> onCorrectCheckpointCrossed;
        public event Action<PenaltyTrigger, float> onPenaltyTrigger;
        public event Action<RewardCheckpoint, float> onWrongRewardCheckpointCrossed;
        public event Action<RewardCheckpoint, float> onCorrectRewardCheckpointCrossed;
        public event Action<float> onFall;
        public event Action<float> onHasFinished;


        public Collider collider;

        public void Reset() {
            timer = 0f;
            lastCheckpoint = 0;
            lastRewardCheckpoint = 0;
            lap = 1;
            hasFinished = false;
        }

        private void FixedUpdate() {
            if (hasFinished) return;
            timer += Time.deltaTime;
        }

        public void SetShipCollider(ShipCollider collider) {
            Debug.Log($"setting callback");
            collider.onTrigger += reactToCollision;
        }

        private void reactToCollision(Collider other) {
            Debug.Log($"{gameObject.name} collided with {other.gameObject.name}");
            if (other.gameObject.TryGetComponent<Checkpoint>(out var checkpoint)) {
                CheckpointCallback(checkpoint);
                return;
            }

            if (other.gameObject.TryGetComponent<FallCatcher>(out var fall_catcher)) {
                FallCatcherCallback(fall_catcher);
                return;
            }

            // if (RaceStateManager.Instance.TrainingMode == RaceStateManager.TrainingModeTypes.Disabled)
            //     return;

            if (other.gameObject.TryGetComponent<RewardCheckpoint>(out var reward_checkpoint)) {
                RewardCheckpointCallback(reward_checkpoint);
                return;
            }

            if (other.gameObject.TryGetComponent<PenaltyTrigger>(out var penalty_trigger)) {
                onPenaltyTrigger?.Invoke(penalty_trigger, timer);
                return;
            }
        }

        public void CheckpointCallback(Checkpoint checkpoint) {
            UpdateShipCheckpointAndLap(checkpoint);
            UpdateHasFinished();
            UpdateShipLastCheckpointPositionAndVelocity();
        }

        public void RewardCheckpointCallback(RewardCheckpoint checkpoint) {
            UpdateRewardCheckpoint(checkpoint);
        }

        public void FallCatcherCallback(FallCatcher fall_catcher) {
            Debug.Log($"{gameObject.name} fell !");
            onFall?.Invoke(timer);
        }

        private void UpdateShipLastCheckpointPositionAndVelocity() {
            player.SaveCheckpoint();
        }

        private void UpdateRewardCheckpoint(RewardCheckpoint crossedCheckpoint) {
            int number_of_checkpoints = RaceStateManager.Instance.circuit.RewardCheckpoints.Count;
            int passed_cp = RaceStateManager.Instance.circuit.RewardCheckpoints.IndexOf(crossedCheckpoint);

            // special cases 
            bool passed_last_cp = (passed_cp == number_of_checkpoints - 1);
            if (passed_last_cp) {
                Debug.Log("going through last checkpoint...");

                if (lastRewardCheckpoint == 0) {
                    Debug.Log("... backward");

                    lastRewardCheckpoint = passed_cp;

                    onWrongRewardCheckpointCrossed?.Invoke(crossedCheckpoint, timer);

                    return;
                }

                if (lastRewardCheckpoint == number_of_checkpoints - 2) {
                    Debug.Log("... forward");

                    lastRewardCheckpoint = passed_cp;

                    onCorrectRewardCheckpointCrossed?.Invoke(crossedCheckpoint, timer);

                    return;
                }
            }
            else if (passed_cp == 0) {
                Debug.Log("going through first checkpoint...");
                if (lastRewardCheckpoint == number_of_checkpoints - 1) {
                    Debug.Log("... forwards");

                    lastRewardCheckpoint = passed_cp;

                    onCorrectRewardCheckpointCrossed?.Invoke(crossedCheckpoint, timer);

                    return;
                }

                if (lastRewardCheckpoint == 1) {
                    Debug.Log("... backward");
                    lastRewardCheckpoint = passed_cp;
                    onWrongRewardCheckpointCrossed?.Invoke(crossedCheckpoint, timer);
                }
            }

            // General case
            else {
                Debug.Log("General case.");
                if (passed_cp == lastRewardCheckpoint + 1) {
                    lastRewardCheckpoint = passed_cp;
                    onCorrectRewardCheckpointCrossed?.Invoke(crossedCheckpoint, timer);

                    return;
                }

                lastRewardCheckpoint = passed_cp;
                onWrongRewardCheckpointCrossed?.Invoke(crossedCheckpoint, timer);
            }
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
            if (hasFinished == false && lap == RaceStateManager.Instance.circuit.Laps + 1) {
                hasFinished = true;
                
                
                onHasFinished?.Invoke(timer);
                if (UserSession.Instance == null) return;
                
                var final_time = TimeSpan.FromSeconds(timer).TotalMilliseconds;
                StartCoroutine(UserSession.Instance.SaveRecord(
                    RaceStateManager.Instance.circuit.Id.ToString(), 
                    (int)final_time,
                    m => { Debug.Log(m);},
                    e => { Debug.Log(e); }
                ));

                if (RaceStateManager.Instance.raceMode != RaceStateManager.RaceMode.Classic) return;
                wonMedal = RaceStateManager.Instance.CurrentWinnableMedal;
                StartCoroutine(UserSession.Instance.SaveMedal(
                    RaceStateManager.Instance.circuit.Id.ToString(), 
                    wonMedal,
                    m => { Debug.Log(m);},
                    e => { Debug.Log(e); }
                ));
                
            }
        }

        #if UNITY_EDITOR
        private void OnDrawGizmos() {
            // GUIStyle style = new();
            // style.normal.textColor = hasFinished ? Color.green : Color.yellow;
            // Handles.Label(player.AbstractShipController.transform.position + Vector3.up * 1.75f,
            //     "cp " + lastCheckpoint + ",lap " + lap, style);
        }
        #endif
    }
}