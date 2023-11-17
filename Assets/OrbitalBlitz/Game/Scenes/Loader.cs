using UnityEngine.SceneManagement;

namespace OrbitalBlitz.Game.Scenes {
    public static class Loader {
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