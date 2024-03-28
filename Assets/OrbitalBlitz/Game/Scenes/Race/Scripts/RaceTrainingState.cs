using Cinemachine;
using OrbitalBlitz.Game.Features.Player;
using OrbitalBlitz.Game.Features.Ship;
using OrbitalBlitz.Game.Features.Ship.Controllers;
using OrbitalBlitz.Game.Scenes.Circuits.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OrbitalBlitz.Game.Scenes.Race.Scripts {
    public class RaceTrainingState : RaceBaseState {
        private float _countdown;
        private int _lastUsedSpawnPoint;
        private bool _hasCountdownStarted;
        private bool _setupFinished;
        private RaceStateManager _stateManager;

        [SerializeField] private const float CountdownLength = 3f;

        public override void UpdateState(RaceStateManager context) {
            if (_setupFinished) context.SwitchState(RaceStateManager.RaceState.RaceCountDown);
            // if (_hasCountdownStarted && (_countdown += Time.deltaTime) > CountdownLength)
            //     context.SwitchState(RaceStateManager.RaceState.RacePlaying);
        }

        public override void EnterState(RaceStateManager context) {
            base.EnterState(context);
            _stateManager = context;
            // PlayerSingleton.Singleton.Input.defaultMap.ToggleEscapeMenu.started += toggleEscapeMenuCallback;
            // RaceSetup();
        }

        public override void ExitState(RaceStateManager context) {
            base.ExitState(context);
        }

        // private void RaceSetup() {
        //     Debug.Log("RaceManager/Setup : RaceSetup beginning...");
        //     AddCallbacksToCheckpoints();
        //     SpawnPlayer();
        //     PlayerSingleton.Singleton.ShipController.SetIsKinematic(true);
        //     _setupFinished = true;
        //     Debug.Log("RaceManager/Setup : RaceSetup finished !");
        // }
        //
        // public void SpawnPlayer() {
        //     Debug.Log("\tRaceManager/Setup : SpqwnPlayer beginning...");
        //     int spawnpointsCount = _stateManager.circuit.Spawnpoints.Count;
        //     int i = (_lastUsedSpawnPoint + 1) % spawnpointsCount;
        //     Transform spTransform = _stateManager.circuit.Spawnpoints[i].gameObject.transform;
        //
        //     var spPosition = spTransform.position;
        //     var spRotation = spTransform.rotation;
        //
        //     Transform ship = Object.Instantiate(_stateManager.ShipPrefab, spPosition, spRotation);
        //     ship.gameObject.name = $"ship";
        //     
        //     _lastUsedSpawnPoint = i;
        //     Debug.Log("\tRaceManager/Setup : SpqwnPlayer finished !");
        // }
        //
        // public void toggleEscapeMenuCallback(InputAction.CallbackContext callbackContext) {
        //     RaceStateManager.Instance.EscapeMenuController.Toggle();
        // }
        // public void AddCallbacksToCheckpoints() {
        //     Debug.Log("\tRaceManager/Setup : AddCallbacksToCheckpoint beginning...");
        //     _stateManager.circuit.Checkpoints.ForEach(delegate(Checkpoint cp) {
        //         Debug.Log($"Added OnShipEnter callback on {cp.name}");
        //         cp.onShipEnter += Checkpoint_OnShipEnter;
        //     });
        //     Debug.Log("\tRaceManager/Setup : AddCallbacksToCheckpoint finished !");
        // }
        //
        // public void Checkpoint_OnShipEnter(Checkpoint checkpoint, GameObject ship) {
        //     Debug.Log($"{ship.name} passed {checkpoint.name}");
        //     UpdateShipCheckpointAndLap(checkpoint, ship);
        //     UpdateShipHasFinished(ship);
        //     UpdateShipLastCheckpointPositionAndVelocity(ship);
        // }
        //
        // private void UpdateShipLastCheckpointPositionAndVelocity(GameObject ship) {
        //     var _controller = ship.GetComponentInChildren<IShipController>();
        //     _controller.setLastCheckpointPhysicsState(_controller.GetCurrentPhysicsState());
        // }
        //
        // private void UpdateShipCheckpointAndLap(Checkpoint crossed_checkpoint, GameObject ship) {
        //     //Debug.Log("updatePlayerCheckpointAndLap called.");
        //     int numberOfCheckpoints = _stateManager.circuit.Checkpoints.Count;
        //     int passed_cp = _stateManager.circuit.Checkpoints.IndexOf(crossed_checkpoint);
        //
        //     int playerLastCp = ship.GetComponent<PlayerInfo>().lastCheckpoint;
        //     int playerLap = ship.GetComponent<PlayerInfo>().lap;
        //
        //     // special cases 
        //     bool passedLastCp = passed_cp == numberOfCheckpoints - 1;
        //     if (passedLastCp) {
        //         //Debug.Log("going through last checkpoint...");
        //
        //         if (playerLastCp == 0) { // ... backwards
        //             //Debug.Log("... backwards");
        //
        //             ship.GetComponent<PlayerInfo>().lastCheckpoint = passed_cp;
        //             ship.GetComponent<PlayerInfo>().lap = playerLap - 1;
        //             return;
        //         }
        //
        //         if (playerLastCp == numberOfCheckpoints - 2) { // ... forwards
        //             //Debug.Log("... forwards");
        //
        //             ship.GetComponent<PlayerInfo>().lastCheckpoint = passed_cp;
        //             return;
        //         }
        //     }
        //     else if (passed_cp == 0) { // going through first checkpoint
        //         //Debug.Log("going through first checkpoint...");
        //         if (playerLastCp == numberOfCheckpoints - 1) { // ... forwards
        //             //Debug.Log("... forwards");
        //
        //             ship.GetComponent<PlayerInfo>().lastCheckpoint = passed_cp;
        //             ship.GetComponent<PlayerInfo>().lap = playerLap + 1;
        //             return;
        //         }
        //
        //         if (playerLastCp == 1) // ... backwards
        //         {
        //             //Debug.Log("... backwards");
        //             ship.GetComponent<PlayerInfo>().lastCheckpoint = passed_cp;
        //         }
        //     }
        //     else { // General case
        //         //Debug.Log("General case.");
        //         if (passed_cp == playerLastCp + 1 || passed_cp == playerLastCp - 1) {
        //             ship.GetComponent<PlayerInfo>().lastCheckpoint = passed_cp;
        //             return;
        //         }
        //     }
        //     //Debug.Log("Ship " + player.name + " induly passed checkpoint " + crossed_checkpoint.gameObject.name);
        // }
        //
        // private void UpdateShipHasFinished(GameObject player) {
        //     //Debug.Log("updatePlayerHasFinished called.");
        //
        //     if (player.GetComponent<PlayerInfo>().lap == _stateManager.circuit.Laps + 1) {
        //         player.GetComponent<PlayerInfo>().hasFinished = true;
        //     }
        // }
        //
        // private void StartRaceCountdown() {
        //     _hasCountdownStarted = true;
        // }
    }
}