using OrbitalBlitz.Game.Features.Ship;
using OrbitalBlitz.Game.Scenes.Circuits.Scripts;
using UnityEngine;

namespace OrbitalBlitz.Game.Scenes.Race.Scripts {
    public class RaceSetupState : RaceBaseState {
        private float _countdown;
        private int _lastUsedSpawnPoint;
        private bool _hasCountdownStarted;
        private bool _setupFinished;
        private RaceStateManager _stateManager;

        [SerializeField] private const float CountdownLength = 3f;

        public override void UpdateState(RaceStateManager context) {
            if (_setupFinished) context.SwitchState(RaceStateManager.RaceState.RacePlaying);
            // if (_hasCountdownStarted && (_countdown += Time.deltaTime) > CountdownLength)
            //     context.SwitchState(RaceStateManager.RaceState.RacePlaying);
        }

        public override void EnterState(RaceStateManager context) {
            base.EnterState(context);
            _stateManager = context;
            RaceSetup();
        }

        private void RaceSetup() {
            Debug.Log("RaceManager RaceSetup");
            AddCallbacksToCheckpoints();
            SpawnPlayer();
            // SpawnBots();
            // StartRaceCountdown();
            _setupFinished = true;
        }

        public void SpawnPlayer() {
            int spawnpointsCount = CircuitManager.Instance.Spawnpoints.Count;
            int i = (_lastUsedSpawnPoint + 1) % spawnpointsCount;
            Transform spTransform = CircuitManager.Instance.Spawnpoints[i].gameObject.transform;

            var spPosition = spTransform.position;
            var spRotation = spTransform.rotation;
            // Transform player = Object.Instantiate(_stateManager.PlayerPrefab, spPosition, spRotation);
            // player.gameObject.name = $"player";

            Transform ship = Object.Instantiate(_stateManager.ShipPrefab, spPosition, spRotation);
            ship.gameObject.name = $"ship";

            // player.gameObject
            //     .GetComponent<PlayerAgent>()
            //     .LinkToShip(
            //         ship.gameObject
            //     );
            // Debug.Log($"Linked PlayerAgent to its Ship.");
            _lastUsedSpawnPoint = i;
        }

        public void AddCallbacksToCheckpoints() {
            CircuitManager.Instance.Checkpoints.ForEach(delegate(Checkpoint cp) {
                Debug.Log($"Added OnShipEnter callback on {cp.name}");
                cp.onShipEnter += Checkpoint_OnShipEnter;
            });
        }

        public void Checkpoint_OnShipEnter(Checkpoint checkpoint, GameObject ship) {
            Debug.Log($"{ship.name} passed {checkpoint.name}");
            UpdateShipCheckpointAndLap(checkpoint, ship);
            UpdateShipHasFinished(ship);
        }

        private void UpdateShipCheckpointAndLap(Checkpoint crossed_checkpoint, GameObject ship) {
            //Debug.Log("updatePlayerCheckpointAndLap called.");
            int numberOfCheckpoints = CircuitManager.Instance.Checkpoints.Count;
            int passed_cp = CircuitManager.Instance.Checkpoints.IndexOf(crossed_checkpoint);

            int playerLastCp = ship.GetComponent<ShipRaceInfo>().lastCheckpoint;
            int playerLap = ship.GetComponent<ShipRaceInfo>().lap;

            // special cases 
            bool passedLastCp = passed_cp == numberOfCheckpoints - 1;
            if (passedLastCp) {
                //Debug.Log("going through last checkpoint...");

                if (playerLastCp == 0) { // ... backwards
                    //Debug.Log("... backwards");

                    ship.GetComponent<ShipRaceInfo>().lastCheckpoint = passed_cp;
                    ship.GetComponent<ShipRaceInfo>().lap = playerLap - 1;
                    return;
                }

                if (playerLastCp == numberOfCheckpoints - 2) { // ... forwards
                    //Debug.Log("... forwards");

                    ship.GetComponent<ShipRaceInfo>().lastCheckpoint = passed_cp;
                    return;
                }
            }
            else if (passed_cp == 0) { // going through first checkpoint
                //Debug.Log("going through first checkpoint...");
                if (playerLastCp == numberOfCheckpoints - 1) { // ... forwards
                    //Debug.Log("... forwards");

                    ship.GetComponent<ShipRaceInfo>().lastCheckpoint = passed_cp;
                    ship.GetComponent<ShipRaceInfo>().lap = playerLap + 1;
                    return;
                }

                if (playerLastCp == 1) // ... backwards
                {
                    //Debug.Log("... backwards");
                    ship.GetComponent<ShipRaceInfo>().lastCheckpoint = passed_cp;
                }
            }
            else { // General case
                //Debug.Log("General case.");
                if (passed_cp == playerLastCp + 1 || passed_cp == playerLastCp - 1) {
                    ship.GetComponent<ShipRaceInfo>().lastCheckpoint = passed_cp;
                    return;
                }
            }
            //Debug.Log("Ship " + player.name + " induly passed checkpoint " + crossed_checkpoint.gameObject.name);
        }

        private void UpdateShipHasFinished(GameObject player) {
            //Debug.Log("updatePlayerHasFinished called.");

            if (player.GetComponent<ShipRaceInfo>().lap == CircuitManager.Instance.Laps + 1) {
                player.GetComponent<ShipRaceInfo>().hasFinished = true;
            }
        }

        private void StartRaceCountdown() {
            _hasCountdownStarted = true;
        }
    }
}