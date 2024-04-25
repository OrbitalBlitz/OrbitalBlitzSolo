using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using OrbitalBlitz.Game.Features.Player;
using OrbitalBlitz.Game.Features.Ship.Controllers;
using OrbitalBlitz.Game.Scenes.Race.Scripts;
using Unity.Barracuda;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace OrbitalBlitz.Game.Features.Ship {
    public class PlayerAgent : Agent {
        [SerializeField] private OrbitalBlitzPlayer player;

        public bool IsHuman = false;

        private bool is_asking_for_respawn = false;
        private bool is_asking_for_restart = false;

        private float base_respawn_timer = 1f;
        private float respawn_timer = 1f;

        public event Action OnPlayerRespawnedToLastCheckpoint;
        public event Action OnPlayerToggledBoost;


        private void Update() {
            respawn_timer = Math.Max(0, respawn_timer - Time.deltaTime);
            AddReward(-Time.deltaTime / 2);
            // if (player.AbstractShipController.is_drifting) AddReward(Time.deltaTime / 2); 
        }

        #if UNITY_EDITOR
        private void OnDrawGizmos() {
            var total_reward = GetCumulativeReward();
            var style = new GUIStyle();
            style.normal.textColor = total_reward > 0 ? UnityEngine.Color.green : UnityEngine.Color.red;
            Handles.Label(
                player.AbstractShipController.transform.position + Vector3.up * 2, 
                $"{total_reward:F}",
                style);

            if (!IsHuman) return;
            style.normal.textColor = Color.yellow;
            string cp_angles = "";
            for (int i = 1; i < 8; i++) {

                var reward_checkpoint =
                    RaceStateManager.Instance.circuit.NthNextRewardCheckpoint(player.Info.lastRewardCheckpoint, i);
                var cp_angle = AlgebraUtils.SignedAngleToObject(
                    player.AbstractShipController.gameObject,
                    reward_checkpoint.gameObject,
                    true
                );
                var cp_index = RaceStateManager.Instance.circuit.RewardCheckpoints.IndexOf(reward_checkpoint);
        
                cp_angles += $"{i}th next = cp {cp_index} ({cp_angle:F3})\n";
            }
            Handles.Label(
                player.AbstractShipController.transform.position + Vector3.up * 1.5f, 
                $"{cp_angles}",
                style);

            /*
            var inertia_angle = AlgebraUtils.SignedAngleBetween(
                player.AbstractShipController.transform.forward,
                player.AbstractShipController.RB.velocity,
                player.AbstractShipController.transform.up,
                true
            );
            Handles.Label(
                player.AbstractShipController.transform.position + Vector3.up * 1.5f, 
                $"inertia_angle: {inertia_angle}) \n", style);
            */
            
            // var angle_to_next_cp = AlgebraUtils.SignedAngleToObject(
            //     player.AbstractShipController.gameObject,
            //     RaceStateManager.Instance.circuit.NthNextRewardCheckpoint(player.Info.lastCheckpoint, 1).gameObject,
            //     true
            // );
            // var angle_to_second_next_cp = AlgebraUtils.SignedAngleToObject(
            //     player.AbstractShipController.gameObject,
            //     RaceStateManager.Instance.circuit.NthNextRewardCheckpoint(player.Info.lastCheckpoint, 2).gameObject,
            //     true
            // );
            // Handles.Label(
            //     player.AbstractShipController.transform.position + Vector3.up * 1.5f, 
            //     $"inertia_angle: {inertia_angle}) \n" +
            //     $"angle_to_next_cp: {angle_to_next_cp})\n" +
            //     $"angle_to_second_next_cp: {angle_to_second_next_cp})",
            //     style);
        }
        #endif
        public void Init() {
            // collision handling examples :
            if (IsHuman 
                && RaceStateManager.Instance.TrainingMode != RaceStateManager.TrainingModeTypes.Testing 
                && RaceStateManager.Instance.TrainingMode != RaceStateManager.TrainingModeTypes.Recording
                ) {
                
                // player.Info.onHasFinished += timer => { ; };
                player.Info.onFall += (timer) => { player.RespawnToLastCheckpoint(); };
                return;
            }

            player.Info.onHasFinished += timer => {
                Debug.Log($"{gameObject.name} finsihed");
                // AddReward(10f);
                EndEpisode();
            };
            // player.Info.onCorrectCheckpointCrossed += (cp, timer) => {
            //     Debug.Log($"{gameObject.name} crossed correct CheckPoint");
            //     AddReward(1f);
            // };
            player.Info.onCorrectRewardCheckpointCrossed += (cp, timer) => {
                Debug.Log($"{gameObject.name} crossed correct reward CP");
                AddReward(cp.Reward);
            };
            player.Info.onWrongRewardCheckpointCrossed += (cp, timer) => {
                Debug.Log($"{gameObject.name} crossed wrong reward CP");
                AddReward(-cp.PenaltyForWrongOrder);
            };
            player.Info.onPenaltyTrigger += (trigger, timer) => {
                Debug.Log($"{gameObject.name} crossed penalty trigger {trigger.gameObject.name}");
                AddReward(-trigger.Penalty);
            };
            // player.Info.onWrongCheckpointCrossed += (cp, timer) => {
            //     Debug.Log($"{gameObject.name} crossed wrong Checkpoint");
            //     AddReward(-5f);
            //     EndEpisode();
            // };
            player.Info.onFall += (timer) => {
                Debug.Log($"{gameObject.name} fell");
                // AddReward(-5f);
                EndEpisode();
            };
        }


        public override void OnEpisodeBegin() {
            // Debug.Log($"{gameObject.name} : OnEpisodeBegin (reward is {GetCumulativeReward()})!");
            if (player == null) return; 
            player.Respawn();
        }

        public override void CollectObservations(VectorSensor sensor) {
            // Debug.Log("Observations size: " + sensor.ObservationSize());
            // sensor.AddObservation(player.AbstractShipController.transform.position); // 3 floats
            // sensor.AddObservation(player.AbstractShipController.transform.rotation); // 1 floats

            sensor.AddObservation(
                player.AbstractShipController.RB.velocity.magnitude / player.AbstractShipController.max_speed_forward
                ); // 1 float
            
            var inertia_angle = AlgebraUtils.SignedAngleBetween(
                player.AbstractShipController.transform.forward,
                player.AbstractShipController.RB.velocity,
                player.AbstractShipController.transform.up,
                true
            );
            sensor.AddObservation(inertia_angle); // 1 float

            for (int i = 1; i <= 7; i++) { // 7 floats
                var cp_angle = AlgebraUtils.SignedAngleToObject(
                    player.AbstractShipController.gameObject,
                    RaceStateManager.Instance.circuit.NthNextRewardCheckpoint(player.Info.lastRewardCheckpoint, i).gameObject,
                    true
                );  
                sensor.AddObservation(cp_angle); // 1 float
            }
        }

        public override void OnActionReceived(ActionBuffers actions) {
            if (player.AbstractShipController == null) return;
            
            player.AbstractShipController.Accelerate(actions.ContinuousActions[0]);
            player.AbstractShipController.Steer(actions.ContinuousActions[1]);

            player.AbstractShipController.Brake(actions.DiscreteActions[1]);
            if (Convert.ToBoolean(actions.DiscreteActions[2])) {
                player.AbstractShipController.ActivateBlitz();
                OnPlayerToggledBoost?.Invoke();
            }
            
            if (!IsHuman) return;
            
            if (Convert.ToBoolean(actions.DiscreteActions[0]))
                if (respawn_timer == 0) {
                    AddReward(-5f);
                    player.RespawnToLastCheckpoint();
                    OnPlayerRespawnedToLastCheckpoint?.Invoke();
                    respawn_timer = base_respawn_timer;
                }

            if (Convert.ToBoolean(actions.DiscreteActions[3]))
                if (respawn_timer == 0) { 
                    // AddReward(-1f);
                    // EndEpisode();
                    // player.Respawn();
                    RaceStateManager.Instance.RestartRace();
                    respawn_timer = base_respawn_timer;
                }
        }

        private void LateUpdate() {
            //// Move and rotate the player to match the ship's position and rotation
        }

        public override void Heuristic(in ActionBuffers actionsOut) {
            Vector2 inputVector = player.Input.defaultMap.Move.ReadValue<Vector2>();
            float brakeInput = player.Input.defaultMap.Brake.ReadValue<float>();
            float blitzInput = player.Input.defaultMap.UseBlitz.ReadValue<float>();
            float respawnInput = player.Input.defaultMap.Respawn.ReadValue<float>();
            float restartInput = player.Input.defaultMap.Restart.ReadValue<float>();

            var continuousActionsOut = actionsOut.ContinuousActions;
            continuousActionsOut[0] = inputVector.y;
            continuousActionsOut[1] = inputVector.x;

            var discreteActionsOut = actionsOut.DiscreteActions;
            discreteActionsOut[0] = respawnInput > 0 ? 1 : 0;
            discreteActionsOut[1] = brakeInput > 0 ? 1 : 0;
            discreteActionsOut[2] = blitzInput > 0 ? 1 : 0;
            discreteActionsOut[3] = restartInput > 0 ? 1 : 0;
        }

        private void OnTriggerEnter(Collider other) {
            // /!\
            // Do not implement this function (player's collider != ship's collider)
            // Subscribe collision callbacks to the ship's collider in the Awake function  
            // /!\
        }


        private void OnCollisionEnter(Collision collision) {
            // /!\
            // Do not implement this function (player's collider != ship's collider)
            // Subscribe collision callbacks to the ship's collider in the Awake function
            // /!\
        }
    }
}