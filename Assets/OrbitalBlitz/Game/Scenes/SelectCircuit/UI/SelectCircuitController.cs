using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OrbitalBlitz.Game.Features.API.Models;
using OrbitalBlitz.Game.Scenes.Circuits.Scripts;
using OrbitalBlitz.Game.Scenes.Race.Scripts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace OrbitalBlitz.Game.Scenes.SelectCircuit.UI {
    public class SelectCircuitController : MonoBehaviour {
        [FormerlySerializedAs("_view")] [SerializeField]
        private SelectCircuitView view;

        private Loader.CircuitInfo _selectedCircuit;

        private void Start() {
            view.setCircuitList(getBasicCircuitInfo());
            StartCoroutine(getCircuitsWithRecords(list => {
                view.setCircuitList(list);
            }));
            view.OnPlayClicked += loadSelectedCircuit;
            view.OnCircuitClicked += setSelectedCircuit;
            view.OnBackClicked += () => { Loader.LoadScene(Loader.Scene.MainMenu); };

            view.PlayClassicButton.style.display = DisplayStyle.None;
            view.PlayClockButton.style.display = DisplayStyle.None;
            
        }

        private void setSelectedCircuit(Loader.CircuitInfo circuit) {
            Loader.UnloadScene(_selectedCircuit.Scene);
            Loader.LoadScene(circuit.Scene, LoadSceneMode.Additive);
            _selectedCircuit = circuit;
            view.PlayClassicButton.style.display = DisplayStyle.Flex;
            view.PlayClockButton.style.display = DisplayStyle.Flex;
        }

        private void loadSelectedCircuit(RaceStateManager.RaceMode mode) {
            PlayerPrefs.SetString("RaceMode", mode.ToString());
            Loader.LoadScene(_selectedCircuit.Scene);
            Loader.LoadScene(Loader.Scene.Race, LoadSceneMode.Additive);
        }

        private List<Loader.CircuitInfo> getBasicCircuitInfo() {
            var circuits = Loader.Circuits;
            var circuitsInfo = new List<Loader.CircuitInfo>();

            foreach (var circuit in circuits) {
                List<Record> personalBests = new();
                Circuit.MedalType medal = Circuit.MedalType.NoMedal;
                List<Record> worldBests = new();

                // Now that all data is available, construct the CircuitInfo
                var circuitInfo = new Loader.CircuitInfo {
                    Name = circuit.Name,
                    Id = circuit.Id,
                    Scene = circuit.Scene,
                    PersonalBests = personalBests,
                    Medal = medal,
                    WorldBests = worldBests
                };

                circuitsInfo.Add(circuitInfo);
            }

            return circuitsInfo;
        }
        private IEnumerator getCircuitsWithRecords(Action<List<Loader.CircuitInfo>> onComplete) {
            var circuits = Loader.Circuits;
            var circuitsWithRecords = new List<Loader.CircuitInfo>();
            
            foreach (var circuit in circuits) {
                List<Record> personalBests = new();
                Circuit.MedalType medal = Circuit.MedalType.NoMedal;
                List<Record> worldBests = new();

                // Use a counter to track completed coroutine calls
                int completedCoroutines = 0;

                // Fetch personal bests
                CoroutineRunner.instance.StartCoroutine(
                    UserSession.Instance.GetPlayerRecords(
                        circuit.Id.ToString(),
                        records => {
                            personalBests = records;
                            completedCoroutines++;
                        },
                        error => {
                            Debug.Log(error);
                            completedCoroutines++;
                        }));

                // Fetch medal
                CoroutineRunner.instance.StartCoroutine(
                    UserSession.Instance.GetPlayerMedal(
                        circuit.Id.ToString(),
                        m => {
                            medal = m;
                            completedCoroutines++;
                        },
                        e => {
                            completedCoroutines++;
                            Debug.Log($"Error fetching medal: {e}");
                        }));

                // Fetch world records
                CoroutineRunner.instance.StartCoroutine(
                    UserSession.Instance.GetWorldRecords(
                        circuit.Id.ToString(),
                        records => {
                            Debug.Log($"Received world records for circuit {circuit.Id} ({records.Count})");
                            worldBests = records;
                            completedCoroutines++;
                        },
                        e => {
                            completedCoroutines++;
                            Debug.Log(e);
                        }));

                // Wait until all data is fetched
                yield return new WaitUntil(() => completedCoroutines == 3);

                // Now that all data is available, construct the CircuitInfo
                var circuitInfo = new Loader.CircuitInfo {
                    Name = circuit.Name,
                    Id = circuit.Id,
                    Scene = circuit.Scene,
                    PersonalBests = personalBests,
                    Medal = medal,
                    WorldBests = worldBests
                };

                circuitsWithRecords.Add(circuitInfo);
            }
            Debug.Log($"getCircuitsWithRecords: onComplete?.Invoke({circuitsWithRecords.Count} circuits)");
            onComplete?.Invoke(circuitsWithRecords);
        }
    }
}