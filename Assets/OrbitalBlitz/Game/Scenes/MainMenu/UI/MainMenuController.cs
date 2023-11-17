using UnityEngine;
using UnityEngine.SceneManagement;

namespace OrbitalBlitz.Game.Scenes.MainMenu.UI {
    public class MainMenuController : MonoBehaviour {
        [SerializeField] private MainMenuView _mainMenuView;
    
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