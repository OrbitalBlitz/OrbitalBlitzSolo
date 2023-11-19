using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace OrbitalBlitz.Game.Scenes {
    public static class Loader {
        public struct Record {
            public string PlayerName;
            public int TimeInMilliseconds;
        }

        public struct CircuitInfo {
            public string Name;
            public Scene Scene;
            public List<Record> PersonalBests;
            public List<Record> WorldBests;
        }

        public static List<CircuitInfo> Circuits = new() {
            new CircuitInfo {
                Name = "TestCircuit",
                Scene = Scene.TestCircuit_1,
                PersonalBests = new List<Record>() {
                    new Record { PlayerName = "PlayerMe", TimeInMilliseconds = 400000000 }
                },
                WorldBests = new List<Record>() {
                    new Record { PlayerName = "PlayerX", TimeInMilliseconds = 100000000 },
                    new Record { PlayerName = "PlayerY", TimeInMilliseconds = 200000000 },
                    new Record { PlayerName = "PlayerZ", TimeInMilliseconds = 300000000 },
                }
            },
            new CircuitInfo {
                Name = "TestCircuit2",
                Scene = Scene.TestCircuit_1,
                PersonalBests = new List<Record>() {
                    new Record { PlayerName = "PlayerMe", TimeInMilliseconds = 400000000 }
                },
                WorldBests = new List<Record>() {
                    new Record { PlayerName = "PlayerX", TimeInMilliseconds = 100000000 },
                    new Record { PlayerName = "PlayerY", TimeInMilliseconds = 200000000 },
                    new Record { PlayerName = "PlayerZ", TimeInMilliseconds = 300000000 },
                }
            }
        };

        public enum Scene {
            MainMenu,
            SelectCircuit,
            Race,
            TestCircuit_1,
        }

        public static void LoadScene(Scene scene, LoadSceneMode mode = LoadSceneMode.Single) {
            SceneManager.LoadScene(scene.ToString(), mode);
        }
    }
}