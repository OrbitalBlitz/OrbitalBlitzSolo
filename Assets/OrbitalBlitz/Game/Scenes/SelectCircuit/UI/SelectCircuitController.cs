using UnityEngine;
using UnityEngine.SceneManagement;

namespace OrbitalBlitz.Game.Scenes.SelectCircuit.UI {
    public class SelectCircuitController : MonoBehaviour {
        [SerializeField] private SelectCircuitView _mainMenuView;
    
        private void Start() {
            _mainMenuView.OnPlayClicked += goToSelectScene;
        }
        private void OnDestroy() {
            _mainMenuView.OnPlayClicked += goToSelectScene;
        }

        private void goToSelectScene() {
            Loader.LoadScene(Loader.Scene.TestCircuit_1);
            Loader.LoadScene(Loader.Scene.Race, LoadSceneMode.Additive);
        }
    }
}