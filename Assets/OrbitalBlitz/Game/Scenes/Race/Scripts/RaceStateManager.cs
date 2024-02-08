using System.Collections.Generic;
using OrbitalBlitz.Game.Scenes.Circuits.Scripts;
using OrbitalBlitz.Game.Scenes.Race.UI.EndMenu;
using OrbitalBlitz.Game.Scenes.Race.UI.EscapeMenu;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace OrbitalBlitz.Game.Scenes.Race.Scripts {
    public class RaceStateManager : MonoBehaviour {
        public static RaceStateManager Instance { get; private set; }

        public enum RaceState {
            RaceSetup,
            RaceCountDown,
            RacePlaying,
            RaceEnded,
        }

        public Dictionary<RaceState, RaceBaseState> States;
        public RaceState currentState;
        public RaceBaseState currentRaceState;
        
        [SerializeField] public EndMenuController EndMenuController;
        [SerializeField] public EscapeMenuController EscapeMenuController;
        [SerializeField] public Transform PlayerPrefab;
        [SerializeField] public Transform ShipPrefab;
        
        [SerializeField] public int CountDownSeconds = 0;
        
        public CircuitData m_circuit_data;

        void Start() {
            Instance = this;
            States =
                new() {
                    { RaceState.RaceSetup, new RaceSetupState() },
                    { RaceState.RaceCountDown, new RaceCountDownState() },
                    { RaceState.RacePlaying, new RacePlayingState() },
                    { RaceState.RaceEnded, new RaceEndedState() },
                };
            m_circuit_data = FindObjectOfType<CircuitData>();
            SwitchState(RaceState.RaceSetup);
        }

        void FixedUpdate() {
            currentRaceState.UpdateState(this);
            currentRaceState.Update(this);
        }

        public void SwitchState(RaceState newState) {
            currentState = newState;
            currentRaceState?.ExitState(this);
            currentRaceState = States[newState];
            currentRaceState.EnterState(this);
        }

        
    }
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(RaceStateManager))]
    public class RaceStateManagerEditor : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            RaceStateManager stateManager = (RaceStateManager)target;

            EditorGUILayout.Space(10f);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Current state");
            EditorGUILayout.LabelField(stateManager.currentRaceState?.GetType().Name);
            EditorGUILayout.EndHorizontal();
        }
    }
    #endif
}