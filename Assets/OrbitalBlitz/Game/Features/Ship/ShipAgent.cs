using System;
using OrbitalBlitz.Game.Features.Ship.Controllers;
using Unity.Barracuda;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace OrbitalBlitz.Game.Features.Ship {
    public class ShipAgent : Agent {
        [SerializeField] private PlayerInputActions _playerInputActions;
        [SerializeField] private IShipController _shipController;

        private void Awake() {
            _playerInputActions = new PlayerInputActions();
            _playerInputActions.defaultMap.Enable();
            _shipController = gameObject.GetComponentInChildren<IShipController>();
        }

        public override void OnEpisodeBegin() {
            //Logger.Info("In Episode begin", gameObject);
        }

        public override void CollectObservations(VectorSensor sensor) {
            //Debug.Log("In collect Observations");
        }

        public override void OnActionReceived(ActionBuffers actions) {
            // Debug.Log($"continuous actions == [{string.Join(", ", actions.ContinuousActions)}]");
            // Debug.Log($"discrete actions == [{string.Join(", ", actions.DiscreteActions)}]");
            
            _shipController.Accelerate(actions.ContinuousActions[0]);
            _shipController.Steer(actions.ContinuousActions[1]);

            if (Convert.ToBoolean(actions.DiscreteActions[0])) _shipController.Respawn();
            _shipController.Brake(actions.DiscreteActions[1]);
            if (Convert.ToBoolean(actions.DiscreteActions[2])) _shipController.ActivateBlitz();
        }

        private void LateUpdate() {
            //// Move and rotate the player to match the ship's position and rotation
        }

        public override void Heuristic(in ActionBuffers actionsOut) {
            Vector2 inputVector = _playerInputActions.defaultMap.Move.ReadValue<Vector2>();
            float brakeInput = _playerInputActions.defaultMap.Brake.ReadValue<float>();
            float blitzInput = _playerInputActions.defaultMap.UseBlitz.ReadValue<float>();
            float respawnInput = _playerInputActions.defaultMap.Respawn.ReadValue<float>();

            var continuousActionsOut = actionsOut.ContinuousActions;
            continuousActionsOut[0] = inputVector.y;
            continuousActionsOut[1] = inputVector.x;

            var discreteActionsOut = actionsOut.DiscreteActions;
            discreteActionsOut[0] = respawnInput > 0 ? 1 : 0;
            discreteActionsOut[1] = brakeInput > 0 ? 1 : 0;
            discreteActionsOut[2] = blitzInput > 0 ? 1 : 0;
        }

        private void OnTriggerEnter(Collider other) {
            // TODO: handle reward logic based onn collisions   
        }

        private void OnCollisionEnter(Collision collision) {
            // TODO: handle reward logic based on triggers 
        }
    }
}