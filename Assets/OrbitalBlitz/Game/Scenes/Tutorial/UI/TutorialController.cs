using System;
using OrbitalBlitz.Game.Scenes.Circuits.Scripts;
using OrbitalBlitz.Game.Scenes.Race.Scripts;
using Unity.Mathematics;
using UnityEngine;

namespace OrbitalBlitz.Game.Scenes.Tutorial.UI {
    public class TutorialController : MonoBehaviour
    {
        [SerializeField] private TutorialView view;
        [SerializeField] private float seconds_to_drift = 5f;

        [Header("Debug")] 
        public float drift_time;

        private void Start() {
            RaceStateManager.Instance.OnRaceStateChanged += (previous, current) => {
                if (current == RaceStateManager.RaceState.RacePlaying) SubscribeCallbacks();
            };
        }

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
