using System;
using JetBrains.Annotations;
using OrbitalBlitz.Game.Features.Player;
using OrbitalBlitz.Game.Features.Ship.Controllers;
using OrbitalBlitz.Game.Scenes.Race.Scripts;
using Unity.Barracuda;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.Serialization;

namespace OrbitalBlitz.Game.Features.Ship {
    public class PlayerAgent : Agent {
        [SerializeField] private OrbitalBlitzPlayer player;

        private bool is_asking_for_respawn = false;
        private bool is_asking_for_restart = false;
        
        private float base_respawn_timer = 1f;
        private float respawn_timer = 1f;

        private void Update() {
            respawn_timer = Math.Max(0, respawn_timer - Time.deltaTime);
        }

        private void Awake() {
            // collision handling examples :
            player.Info.onHasFinished += timer => { AddReward(0f); };
            player.Info.onCorrectCheckpointCrossed += (cp, timer) => { AddReward(1f); };
            player.Info.onWrongCheckpointCrossed += (cp, timer) => { AddReward(-10f); };
            player.Info.onFall += ( timer) => { AddReward(-10f); };
        }

        public override void OnEpisodeBegin() {
            player.Respawn();
        }

        public override void CollectObservations(VectorSensor sensor) {
            //Debug.Log("In collect Observations");
        }

        public override void OnActionReceived(ActionBuffers actions) {
            if (player.AbstractShipController == null) return;

            player.AbstractShipController.Accelerate(actions.ContinuousActions[0]);
            player.AbstractShipController.Steer(actions.ContinuousActions[1]);

            if (Convert.ToBoolean(actions.DiscreteActions[0]))
                if (respawn_timer == 0) {
                    player.RespawnToLastCheckpoint();
                    respawn_timer = base_respawn_timer;
                }


            if (Convert.ToBoolean(actions.DiscreteActions[3]))
                if (respawn_timer == 0) { 
                    player.Respawn();
                    respawn_timer = base_respawn_timer;
                }

            player.AbstractShipController.Brake(actions.DiscreteActions[1]);
            if (Convert.ToBoolean(actions.DiscreteActions[2])) player.AbstractShipController.ActivateBlitz();
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