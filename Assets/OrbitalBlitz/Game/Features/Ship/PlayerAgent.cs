using System;
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

        private void Update() {
            respawn_timer = Math.Max(0, respawn_timer - Time.deltaTime);
            // AddReward(-Time.deltaTime / 2); // penalise l'ia au court du temps, favorise la vitesse
            // if (player.AbstractShipController.is_drifting) AddReward(Time.deltaTime / 2); // récompense l'ia pour le drift
        }

        private void FixedUpdate() {
            AddReward(-0.01f);
        }

        #if UNITY_EDITOR
        private void OnDrawGizmos() {
            var total_reward = GetCumulativeReward();
            var style = new GUIStyle();
            style.normal.textColor = total_reward > 0 ? UnityEngine.Color.green : UnityEngine.Color.red;
            Handles.Label(player.AbstractShipController.transform.position + Vector3.up * 2, $"{total_reward:F}", style);
        }
        #endif
        public void Init() {
            // collision handling examples :
            // if (IsHuman) {
            //     player.Info.onHasFinished += timer => { AddReward(10f); };
            //     player.Info.onFall += (timer) => { player.RespawnToLastCheckpoint(); };
            //     return;
            // }

            player.Info.onHasFinished += timer => {
                Debug.Log($"{gameObject.name} finsihed");
                AddReward(10f); // récompense l'ia pour avoir finis la course
                EndEpisode();
            };
            player.Info.onCorrectCheckpointCrossed += (cp, timer) => {
                Debug.Log($"{gameObject.name} crossed correct CheckPoint");
                AddReward(5f); // récompense l'ia pour avoir franchis un checkpoint
            };
            player.Info.onCorrectRewardCheckpointCrossed += (cp, timer) => {
                Debug.Log($"{gameObject.name} crossed correct reward CP");
                // AddReward(cp.Reward); // récompense l'ia pour avoir franchis un rewardCheckpoint
                AddReward(0.5f); // récompense l'ia pour avoir franchis un rewardCheckpoint
            };
            player.Info.onWrongRewardCheckpointCrossed += (cp, timer) => {
                Debug.Log($"{gameObject.name} crossed wrong reward CP");
                // AddReward(-cp.PenaltyForWrongOrder); // pénalise l'ia pour avoir franchis un mauvais reward CP (marche arrière)
                AddReward(-1f); // pénalise l'ia pour avoir franchis un mauvais reward CP (marche arrière)
            };
            player.Info.onPenaltyTrigger += (trigger, timer) => {
                Debug.Log($"{gameObject.name} crossed penalty trigger {trigger.gameObject.name}");
                AddReward(-trigger.Penalty); // pénalise l'ia pour avoir franchis le PenaltyCP
            };
            player.Info.onWrongCheckpointCrossed += (cp, timer) => {
                Debug.Log($"{gameObject.name} crossed wrong Checkpoint");
                AddReward(-5f);
                EndEpisode();
            };
            player.Info.onFall += (timer) => {
                Debug.Log($"{gameObject.name} fell");
                AddReward(-10f); // pénalise l'ia si elle chute du circuit
                EndEpisode();
            };
        }

        public override void OnEpisodeBegin() {
            Debug.Log($"{gameObject.name} : OnEpisodeBegin (reward is {GetCumulativeReward()})!");
            player.Respawn();
        }

        public override void CollectObservations(VectorSensor sensor) {
            sensor.AddObservation(
                player.AbstractShipController.RB.velocity.magnitude / player.AbstractShipController.max_speed_forward
            ); // 1 float
            
            var inertia_angle = Vector3.Angle(
                player.AbstractShipController.RB.velocity,
                player.AbstractShipController.transform.forward
            );
            sensor.AddObservation(AlgebraUtils.normalizeAngle(inertia_angle)); // 1 float

            var angle_to_next_cp = AlgebraUtils.angleFromObjectToObject(
                player.AbstractShipController.gameObject,
                RaceStateManager.Instance.circuit.NthNextRewardCheckpoint(player.Info.lastCheckpoint, 1).gameObject
            );

            var angle_to_second_next_cp = AlgebraUtils.angleFromObjectToObject(
                player.AbstractShipController.gameObject,
                RaceStateManager.Instance.circuit.NthNextRewardCheckpoint(player.Info.lastCheckpoint, 2).gameObject
            );
            sensor.AddObservation(AlgebraUtils.normalizeAngle(angle_to_next_cp)); // 1 float
            sensor.AddObservation(AlgebraUtils.normalizeAngle(angle_to_second_next_cp)); // 1 float

            sensor.AddObservation(player.AbstractShipController.RB.transform) // 1 vector3 = 3 floats

            // total = 7 floats
        }

        public override void OnActionReceived(ActionBuffers actions) {
            if (player.AbstractShipController == null) return;
            
            player.AbstractShipController.Accelerate(actions.ContinuousActions[0]);
            player.AbstractShipController.Steer(actions.ContinuousActions[1]);

            player.AbstractShipController.Brake(actions.DiscreteActions[1]);
            if (Convert.ToBoolean(actions.DiscreteActions[2])) player.AbstractShipController.ActivateBlitz();
            
            if (!IsHuman) return;
            
            if (Convert.ToBoolean(actions.DiscreteActions[0]))
                if (respawn_timer == 0) {
                    AddReward(-5f);
                    player.RespawnToLastCheckpoint();
                    respawn_timer = base_respawn_timer;
                }

            if (Convert.ToBoolean(actions.DiscreteActions[3]))
                if (respawn_timer == 0) { 
                    AddReward(-1f);
                    EndEpisode();
                    // player.Respawn();
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