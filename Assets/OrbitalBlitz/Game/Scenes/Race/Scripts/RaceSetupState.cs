using System;
using Cinemachine;
using JetBrains.Annotations;
using OrbitalBlitz.Game.Features.Player;
using OrbitalBlitz.Game.Features.Ship;
using OrbitalBlitz.Game.Features.Ship.Controllers;
using OrbitalBlitz.Game.Scenes.Circuits.Scripts;
using OrbitalBlitz.Game.Scenes.Race.UI.EndMenu;
using OrbitalBlitz.Game.Scenes.Race.UI.EscapeMenu;
using Unity.Barracuda;
using Unity.MLAgents.Policies;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

namespace OrbitalBlitz.Game.Scenes.Race.Scripts {
    public class RaceSetupState : RaceBaseState {
        private float _countdown;
        private int _lastUsedSpawnPoint;
        private bool _hasCountdownStarted;
        private bool _setupFinished;
        private RaceStateManager _stateManager;
        private int _spawnedPlayer = 0;

        [SerializeField] private const float CountdownLength = 3f;

        public override void UpdateState(RaceStateManager context) {
            if (_setupFinished) context.SwitchState(RaceStateManager.RaceState.RaceCountDown);
            // if (_hasCountdownStarted && (_countdown += Time.deltaTime) > CountdownLength)
            //     context.SwitchState(RaceStateManager.RaceState.RacePlaying);
        }

        public override void EnterState(RaceStateManager context) {
            base.EnterState(context);
            _stateManager = context;
            
            Debug.Log("RaceManager/Setup : RaceSetup beginning...");

            if (_stateManager.TrainingMode) {
                TrainingSetup();
                return;
            }

            PlayingSetup();
            _setupFinished = true;
            
            Debug.Log("RaceManager/Setup : RaceSetup finished !");
        }

        private void PlayingSetup() {
            OrbitalBlitzPlayer human_player = spawnPlayer();
            human_player.Input.defaultMap.ToggleEscapeMenu.started += toggleEscapeMenuCallback;
            _stateManager.HumanPlayer = human_player;

            if (_stateManager.raceMode == RaceStateManager.RaceMode.Classic) {
                foreach (var model in _stateManager.circuit.Models) {
                    // SpawnPlayer(); // TODO spawn ai
                }
            }
        }

        private void TrainingSetup() {
            for (var i = 0; i < _stateManager.NumberOfAgents; i++) {
                spawnPlayer(isHuman: false);
            }
        }

        private OrbitalBlitzPlayer spawnPlayer(
            bool isHuman = true
            ) {
            
            Debug.Log("\tRaceManager/Setup : SpawnPlayer beginning...");
            int spawnpointsCount = _stateManager.circuit.Spawnpoints.Count;
            int i = (_lastUsedSpawnPoint + 1) % spawnpointsCount;
            Transform spTransform = _stateManager.circuit.Spawnpoints[i].gameObject.transform;

            var spPosition = spTransform.position;
            var spRotation = spTransform.rotation;

            Transform player_tf = Object.Instantiate(_stateManager.PlayerPrefab);
            player_tf.gameObject.name = $"player_{_spawnedPlayer}";
            
            Transform ship_tf = Object.Instantiate(_stateManager.ShipPrefab, spPosition, spRotation);
            ship_tf.gameObject.name = $"ship_{_spawnedPlayer}";
            // ship_tf.GetComponent<IShipController>().SetIsKinematic(true);

            _spawnedPlayer++;
            _lastUsedSpawnPoint = i;

            var player = player_tf.GetComponent<OrbitalBlitzPlayer>();
            player.SetShip(ship_tf.gameObject);
            
            if (!isHuman) {
                var behaviour = player_tf.GetComponent<BehaviorParameters>();
                // behaviour.BehaviorType = BehaviorType.InferenceOnly;
            }
            
            Debug.Log("\tRaceManager/Setup : SpawnPlayer finished !");
            return player;
        }

        public void toggleEscapeMenuCallback(InputAction.CallbackContext callbackContext) {
            RaceStateManager.Instance.EscapeMenuController.Toggle();
        }
        

        private void StartRaceCountdown() {
            _hasCountdownStarted = true;
        }
    }
}