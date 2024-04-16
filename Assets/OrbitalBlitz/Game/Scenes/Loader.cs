using System.Collections.Generic;
using OrbitalBlitz.Game.Features.API.Models;
using OrbitalBlitz.Game.Scenes.Circuits.Scripts;
using UnityEngine.SceneManagement;

namespace OrbitalBlitz.Game.Scenes {
    public static class Loader {

        public struct CircuitInfo {
            public string Name;
            public long Id;
            public Scene Scene;
            public List<Record> PersonalBests;
            public Circuit.MedalType Medal;
            public int Rank;
            public int RecordsCount;
            public List<Record> WorldBests;
        }
        
        
        public static List<CircuitInfo> Circuits = new() {
            new CircuitInfo {
                Name = "Level1",
                Id = 1712506995,
                Scene = Scene.Level1,
                PersonalBests = new(),
                WorldBests = new()
            },
            new CircuitInfo {
                Name = "Level2",
                Id = 2,
                Scene = Scene.Level2,
                PersonalBests = new(),
                WorldBests = new()
            },
            new CircuitInfo {
                Name = "Level3",
                Id = 3,
                Scene = Scene.Level3,
                PersonalBests = new(),
                WorldBests = new()
            }
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