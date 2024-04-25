using System;
using System.Collections.Generic;
using OrbitalBlitz.Game.Features.API.Models;
using OrbitalBlitz.Game.Scenes.Circuits.Scripts;
using UnityEngine.SceneManagement;

namespace OrbitalBlitz.Game.Scenes {
    public static class Loader {

        [Serializable]
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
                Name = "Tutorial",
                Id = 0,
                Scene = Scene.Tutorial,
                PersonalBests = new(),
                WorldBests = new()
            },
            new CircuitInfo {
                Name = "Precision - 1",
                Id = 1713687387,
                Scene = Scene.Precision1,
                PersonalBests = new(),
                WorldBests = new()
            },
            new CircuitInfo {
                Name = "Speed - 1",
                Id = 1714047001,
                Scene = Scene.Speed1,
                PersonalBests = new(),
                WorldBests = new()
            },
            new CircuitInfo {
                Name = "Endurance - 1",
                Id = 1714045896,
                Scene = Scene.Endurance1,
                PersonalBests = new(),
                WorldBests = new()
            },
            new CircuitInfo {
                Name = "Precision - 2",
                Id = 1713687402,
                Scene = Scene.Precision2,
                PersonalBests = new(),
                WorldBests = new()
            },
            new CircuitInfo {
                Name = "Speed - 2",
                Id = 1714047054,
                Scene = Scene.Speed2,
                PersonalBests = new(),
                WorldBests = new()
            },
            new CircuitInfo {
                Name = "Endurance - 2",
                Id = 1714046173,
                Scene = Scene.Endurance2,
                PersonalBests = new(),
                WorldBests = new()
            },
            new CircuitInfo {
                Name = "Precision - 3",
                Id = 1714046896,
                Scene = Scene.Precision3,
                PersonalBests = new(),
                WorldBests = new()
            },
            new CircuitInfo {
                Name = "Speed - 3",
                Id = 1714047145,
                Scene = Scene.Speed3,
                PersonalBests = new(),
                WorldBests = new()
            },
            new CircuitInfo {
                Name = "Endurance - 3",
                Id = 1714046749,
                Scene = Scene.Endurance3,
                PersonalBests = new(),
                WorldBests = new()
            }
        };

        public enum Scene {
            MainMenu,
            SelectCircuit,
            Race,
            Tutorial,
            Precision1,
            Speed1,
            Endurance1,
            Precision2,
            Speed2,
            Endurance2,
            Precision3,
            Speed3,
            Endurance3
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