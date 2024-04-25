using System;
using OrbitalBlitz.Game.Scenes.Circuits.Scripts;
using OrbitalBlitz.Game.Scenes.Race.Scripts;
using Unity.Mathematics;
using UnityEngine;

namespace OrbitalBlitz.Game.Scenes.Tutorial.UI {
    
    /// <summary>
    /// This class controls the tutorial in the game.
    /// </summary>
    public class TutorialController : MonoBehaviour
    {
        [SerializeField] private TutorialView view;
        [SerializeField] private float seconds_to_drift = 5f;

        [Header("Debug")] 
        public float drift_time;

        /// <summary>
        /// Subscribe to the events that will trigger the tutorial challenges       
        /// </summary>
        private void Start() {
            RaceStateManager.Instance.OnRaceStateChanged += (previous, current) => {
                if (current == RaceStateManager.RaceState.RacePlaying) SubscribeCallbacks();
            };
        }

        /// <summary>
        /// This method subscribes to various events related to the player and the race state.
        /// </summary>
        private void SubscribeCallbacks() {
            RaceStateManager.Instance.HumanPlayer.Info.onHasFinished += (_) => {
                view.ValidateChallenge(TutorialView.TutorialChallenges.Finish);
                view.Hide();
            };
            
            RaceStateManager.Instance.HumanPlayer.Agent.OnPlayerRespawnedToLastCheckpoint += () => {
                view.ValidateChallenge(TutorialView.TutorialChallenges.Respawn);
            };
            
            RaceStateManager.Instance.HumanPlayer.Agent.OnPlayerToggledBoost += () => {
                view.ValidateChallenge(TutorialView.TutorialChallenges.Boost);
            };
            
            RaceStateManager.Instance.OnRaceRestart += () => {
                view.ValidateChallenge(TutorialView.TutorialChallenges.Restart);
            };
            
            
        }

        /// <summary>
        /// Checks if the player is drifting and updates the drift timer.
        /// </summary>
        void FixedUpdate() {
            if (RaceStateManager.Instance.HumanPlayer.AbstractShipController.is_drifting) {
                drift_time += Time.deltaTime;
                view.SetDriftTimer(drift_time);
                if (drift_time >= seconds_to_drift) {
                    view.ValidateChallenge(TutorialView.TutorialChallenges.Drift);
                }
            }

        }

        
    }
}
