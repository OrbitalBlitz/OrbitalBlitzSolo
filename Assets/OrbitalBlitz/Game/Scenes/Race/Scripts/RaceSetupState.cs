using System;
using System.Linq;
using Cinemachine;
using JetBrains.Annotations;
using OrbitalBlitz.Game.Features.Player;
using OrbitalBlitz.Game.Features.Ship;
using OrbitalBlitz.Game.Features.Ship.Controllers;
using OrbitalBlitz.Game.Scenes.Circuits.Scripts;
using OrbitalBlitz.Game.Scenes.Race.UI.EndMenu;
using OrbitalBlitz.Game.Scenes.Race.UI.EscapeMenu;
using Unity.Barracuda;
using Unity.MLAgents.Demonstrations;
using Unity.MLAgents.Policies;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Profiling;
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

            if (_stateManager.TrainingMode != RaceStateManager.TrainingModeTypes.Disabled) {
                TrainingSetup();
                _setupFinished = true;
                return;
            }

            PlayingSetup();
            _setupFinished = true;
            
            Debug.Log("RaceManager/Setup : RaceSetup finished !");
        }

        private void PlayingSetup() {
            var ( human_player,  human_player_ship) = spawnPlayer();

            if (_stateManager.raceMode == RaceStateManager.RaceMode.Classic) {
                foreach (var phantom in _stateManager.circuit.Phantoms) {
                    var (bot_player, _ ) = spawnPlayer(
                        isHuman: false,
                        material: _stateManager.MedalMaterials[phantom.Medal],
                        alpha: 0.3f,
                        model: phantom.Model
                        );

                    bot_player.Info.onHasFinished += f => {
                        _stateManager.CurrentWinnableMedal = _stateManager.CurrentWinnableMedal > phantom.Medal
                            ? _stateManager.CurrentWinnableMedal
                            : phantom.Medal;
                    };
                }
            }
        }

        private void TrainingSetup() {
            OrbitalBlitzPlayer human_player;
            GameObject human_player_ship;
            
            switch (_stateManager.TrainingMode) {
                
                case RaceStateManager.TrainingModeTypes.Recording:
                    ( human_player,  human_player_ship) = spawnPlayer();
                    var recorder = human_player.gameObject.GetComponent<DemonstrationRecorder>();

                    recorder.DemonstrationDirectory = "Assets/OrbitalBlitz/Game/Features/IA/config/ppo/demos";
                    
                    DateTime current_time = DateTime.UtcNow;
                    long unix_time = ((DateTimeOffset)current_time).ToUnixTimeSeconds();
                    recorder.DemonstrationName = $"demo-{unix_time}";
                    
                    recorder.Record = true;
                    break;
                
                case RaceStateManager.TrainingModeTypes.Testing:
                    ( human_player,  human_player_ship) = spawnPlayer();
                    break;
                
                case RaceStateManager.TrainingModeTypes.Training:
                    for (var i = 0; i < _stateManager.NumberOfAgents; i++) {
                        spawnPlayer(
                            isHuman: false, 
                            alpha: _stateManager.BotAlpha, 
                            render: _stateManager.RenderAgents);
                    }
                    break;
            }
        }

        private (OrbitalBlitzPlayer player, GameObject ship) spawnPlayer(
            bool isHuman = true,
            float? alpha = null,
            bool render = true,
            [CanBeNull] Material material = null,
            [CanBeNull] NNModel model = null
            ) {
            
            Debug.Log("\tRaceManager/Setup : SpawnPlayer beginning...");
            int spawnpointsCount = _stateManager.circuit.Spawnpoints.Count;
            int i = (_lastUsedSpawnPoint + 1) % spawnpointsCount;
            Transform spTransform = _stateManager.circuit.Spawnpoints[i].gameObject.transform;

            var spPosition = spTransform.position;
            var spRotation = spTransform.rotation;

            Transform player_tf = Object.Instantiate(_stateManager.PlayerPrefab);
            player_tf.gameObject.name = $"player_{_spawnedPlayer}";
            
            Transform ship_tf = Object.Instantiate(_stateManager.ShipPrefab, spPosition, spRotation, player_tf);
            ship_tf.gameObject.name = $"ship_{_spawnedPlayer}";
            var skin = ship_tf.gameObject.GetComponentInChildren<ShipSkin>();
            
            var player = player_tf.GetComponent<OrbitalBlitzPlayer>();
            player.SetShip(ship_tf.gameObject);
            
            if (material != null) skin.SetMaterial(material);
            if (alpha.HasValue) skin.SetAlpha(alpha.Value);
            if (!render) skin.gameObject.GetComponent<MeshRenderer>().enabled = false;

            var agent = player_tf.GetComponent<PlayerAgent>();
            if (!isHuman) {
                var systems = ship_tf.gameObject.GetComponentsInChildren<ParticleSystem>();
                foreach (var sys in systems) {
                    sys.Stop();
                }
                var exhaust = ship_tf.gameObject.GetComponentInChildren<ExhaustParticlesController>();
                Object.DestroyImmediate(exhaust);
                var virtual_camera = ship_tf.GetComponentInChildren<CinemachineVirtualCamera>();
                Object.DestroyImmediate(virtual_camera.gameObject);
            }
            else {
                _stateManager.HumanPlayer = player;
                agent.IsHuman = true;
                player.Input.defaultMap.ToggleEscapeMenu.started += toggleEscapeMenuCallback;
                ship_tf.GetComponentInChildren<CinemachineVirtualCamera>().Priority = 100; 
            }
            
            agent.Init();

            if (!isHuman || RaceStateManager.Instance.TrainingMode == RaceStateManager.TrainingModeTypes.Recording) {
                ship_tf.GetComponentInChildren<AbstractShipController>().IsHuman = false;
            }

            _spawnedPlayer++;
            _lastUsedSpawnPoint = i;

            
            
            if (model != null) {
                var behaviour = player_tf.GetComponent<BehaviorParameters>();
                behaviour.Model = model;
                behaviour.BehaviorType = BehaviorType.InferenceOnly;
            }
            
            Debug.Log("\tRaceManager/Setup : SpawnPlayer finished !");
            return (player, ship_tf.gameObject);
        }

        public void toggleEscapeMenuCallback(InputAction.CallbackContext callbackContext) {
            RaceStateManager.Instance.EscapeMenuController.Toggle();
        }
        

        private void StartRaceCountdown() {
            _hasCountdownStarted = true;
        }
    }
}