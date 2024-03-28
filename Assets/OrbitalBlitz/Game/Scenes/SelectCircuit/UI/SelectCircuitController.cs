using OrbitalBlitz.Game.Scenes.Race.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OrbitalBlitz.Game.Scenes.SelectCircuit.UI {
    public class SelectCircuitController : MonoBehaviour {
        [SerializeField] private SelectCircuitView _view;
        private Loader.CircuitInfo _selectedCircuit;

        private void Start() {
            _view.setCircuitList(Loader.Circuits);
            _view.OnPlayClicked += loadSelectedCircuit;
            _view.OnCircuitClicked += setSelectedCircuit;
        }

        private void OnDestroy() {
            _view.OnPlayClicked += loadSelectedCircuit;
        }

        private void setSelectedCircuit(Loader.CircuitInfo circuit) {
            Loader.UnloadScene(_selectedCircuit.Scene);
            Loader.LoadScene(circuit.Scene, LoadSceneMode.Additive);
            _selectedCircuit = circuit;
        }

        private void loadSelectedCircuit(RaceStateManager.RaceMode mode) {
            PlayerPrefs.SetString("RaceMode", mode.ToString());
            Loader.LoadScene(_selectedCircuit.Scene);
            Loader.LoadScene(Loader.Scene.Race, LoadSceneMode.Additive);
        }
    }
}