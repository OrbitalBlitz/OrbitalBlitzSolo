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
            _selectedCircuit = circuit;
        }
        
        private void loadSelectedCircuit() {
            Loader.LoadScene(_selectedCircuit.Scene);
            Loader.LoadScene(Loader.Scene.Race, LoadSceneMode.Additive);
        }
    }
}