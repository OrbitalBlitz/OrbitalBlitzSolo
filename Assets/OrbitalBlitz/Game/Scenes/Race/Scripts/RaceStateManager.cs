using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using OrbitalBlitz.Game.Scenes.Circuits.Scripts;
using OrbitalBlitz.Game.Scenes.Race.UI.EndMenu;
using OrbitalBlitz.Game.Scenes.Race.UI.EscapeMenu;
using Unity.Barracuda;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

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

        public enum RaceMode {
            Clock,
            Classic
        }

        public enum TrainingModeTypes {
            Disabled,
            Training,
            Recording,
            Testing
        }

        public event Action OnRaceRestart;
        public RaceMode raceMode = RaceMode.Classic; //TODO base this value on player input

        [Header("UI")] [SerializeField] public EndMenuController EndMenuController;
        [SerializeField] public EscapeMenuController EscapeMenuController;
        [SerializeField] public CinemachineBrain MainCamera;

        [Header("Prefabs")] [SerializeField] public Transform PlayerPrefab;
        [SerializeField] public Transform ShipPrefab;
        [SerializeField] public Transform TutorialPrefab;

        [SerializeField] public int CountDownSeconds = 0;

        [Header("Agents")] [SerializeField] public float BotAlpha = 0.3f;
        [SerializeField] private Material GoldBotMaterial;
        [SerializeField] private Material SilverBotMaterial;
        [SerializeField] private Material BronzeBotMaterial;
        [SerializeField] private Material DefaultBotMaterial;
        public Dictionary<Circuit.MedalType, Material> MedalMaterials;

        [Header("Training")] [SerializeField] public TrainingModeTypes TrainingMode = TrainingModeTypes.Disabled;
        [SerializeField] public bool RenderAgents = true;
        [SerializeField] public int NumberOfAgents = 10;

        [Header("Debug")] [FormerlySerializedAs("CircuitData")] [FormerlySerializedAs("m_circuit_data")]
        public Circuit circuit;

        public Circuit.MedalType CurrentWinnableMedal = Circuit.MedalType.Gold;

        public OrbitalBlitzPlayer HumanPlayer;
        public NNModel DefaultAI;

        public event Action<RaceState, RaceState> OnRaceStateChanged;

        void Awake() {
            Instance = this;
            States =
                new() {
                    { RaceState.RaceSetup, new RaceSetupState() },
                    { RaceState.RaceCountDown, new RaceCountDownState() },
                    { RaceState.RacePlaying, new RacePlayingState() },
                    { RaceState.RaceEnded, new RaceEndedState() },
                };
            MedalMaterials = new() {
                { Circuit.MedalType.Gold, GoldBotMaterial },
                { Circuit.MedalType.Silver, SilverBotMaterial },
                { Circuit.MedalType.Bronze, BronzeBotMaterial },
                { Circuit.MedalType.NoMedal, DefaultBotMaterial },
            };
            circuit = FindObjectOfType<Circuit>();
            MainCamera = FindObjectOfType<CinemachineBrain>();

            if (PlayerPrefs.HasKey("RaceMode"))
                raceMode = Enum.Parse<RaceMode>(PlayerPrefs.GetString("RaceMode"));

            if (PlayerPrefs.HasKey("Tutorial") && PlayerPrefs.GetInt("Tutorial") == 1)
                Instantiate(TutorialPrefab);

            SwitchState(RaceState.RaceSetup);

            if (TrainingMode != TrainingModeTypes.Disabled) {
                var reward_triggers = FindObjectsOfType<RewardCheckpoint>();
                foreach (var trigger in reward_triggers) {
                    trigger.gameObject.GetComponent<MeshRenderer>().enabled = true;
                }
            }
        }

        public void RestartRace() {
            FindObjectsOfType<OrbitalBlitzPlayer>().Aggregate("", (acc, player) => {
                DestroyImmediate(player.gameObject);
                return acc;
            });
            SwitchState(RaceState.RaceSetup);
            OnRaceRestart?.Invoke();
        }

        void FixedUpdate() {
            currentRaceState.UpdateState(this);
            currentRaceState.Update(this);
        }

        public void SwitchState(RaceState newState) {
            var old_state = currentState;
            currentState = newState;
            currentRaceState?.ExitState(this);
            currentRaceState = States[newState];
            currentRaceState.EnterState(this);
            OnRaceStateChanged?.Invoke(old_state, newState);
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