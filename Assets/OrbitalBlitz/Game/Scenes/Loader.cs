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

        private static List<Record> dummyWorldRecords = new() {
            new Record { PlayerName = "PlayerX", TimeInMilliseconds = 100000000 },
            new Record { PlayerName = "PlayerY", TimeInMilliseconds = 200000000 },
            new Record { PlayerName = "PlayerZ", TimeInMilliseconds = 300000000 },
        };

        private static List<Record> dummyPersonalRecords = new() {
            new Record { PlayerName = "PlayerMe", TimeInMilliseconds = 400000000 }
        };
        
        public static List<CircuitInfo> Circuits = new() {
            new CircuitInfo {
                Name = "Level1",
                Scene = Scene.Level1,
                PersonalBests = dummyPersonalRecords,
                WorldBests = dummyWorldRecords
            },
            new CircuitInfo {
                Name = "Level2",
                Scene = Scene.Level2,
                PersonalBests = dummyPersonalRecords,
                WorldBests = dummyWorldRecords
            },
            new CircuitInfo {
                Name = "Level3",
                Scene = Scene.Level3,
                PersonalBests = dummyPersonalRecords,
                WorldBests = dummyWorldRecords
            },
            // new CircuitInfo {
            //     Name = "TestCircuitGen_Custom",
            //     Scene = Scene.TestCircuitGen_Custom,
            //     PersonalBests = dummyPersonalRecords,
            //     WorldBests = dummyWorldRecords
            // },
            // new CircuitInfo {
            //     Name = "TestCircuit",
            //     Scene = Scene.TestCircuit_1,
            //     PersonalBests = dummyPersonalRecords,
            //     WorldBests = dummyWorldRecords
            // },
            // new CircuitInfo {
            //     Name = "TestCircuit2",
            //     Scene = Scene.TestCircuit_1,
            //     PersonalBests = dummyPersonalRecords,
            //     WorldBests = dummyWorldRecords
            // },
            // new CircuitInfo {
            //     Name = "GeneratedCircuit",
            //     Scene = Scene.TestCircuitGen,
            //     PersonalBests = dummyPersonalRecords,
            //     WorldBests = dummyWorldRecords
            // }
        };

        public enum Scene {
            MainMenu,
            SelectCircuit,
            Race,
            Level1,
            Level2,
            Level3,
            // TestCircuit_1,
            // TestCircuitGen,
            // TestCircuitGen_Custom
        }

        public static void LoadScene(Scene scene, LoadSceneMode mode = LoadSceneMode.Single) {
            SceneManager.LoadScene(scene.ToString(), mode);
        }
        
        public static void UnloadScene(Scene scene) {
            if (SceneManager.GetSceneByName(scene.ToString()).isLoaded) {
                SceneManager.UnloadSceneAsync(scene.ToString());
            }
        }
    }
}