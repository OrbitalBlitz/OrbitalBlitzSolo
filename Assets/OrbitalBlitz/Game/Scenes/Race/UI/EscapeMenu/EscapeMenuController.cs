using OrbitalBlitz.Game.Scenes.Race.Scripts;
using UnityEngine;
using UnityEngine.UIElements;

namespace OrbitalBlitz.Game.Scenes.Race.UI.EscapeMenu {
    public class EscapeMenuController : MonoBehaviour {
        [SerializeField] private EscapeMenuView _view;

        private void Start() {
            _view.OnQuitClicked += () => { Loader.LoadScene(Loader.Scene.MainMenu); };
            _view.OnResumeClicked += () => { Hide(); };
            _view.OnRestartClicked += () => {
                Player.Singleton.RaceInfo.Reset();
                RaceStateManager.Instance.SwitchState(RaceStateManager.RaceState.RaceCountDown);
                Player.Singleton.ShipController.Respawn();
            };
            _view.OnRespawnClicked += () => { Player.Singleton.ShipController.RespawnToLastCheckpoint(); };
            
            Hide();
        }
        private void OnDestroy() {  
        }
        
        public void Toggle() {
            bool _isViewHidden = GetComponent<UIDocument>().rootVisualElement.style.display == DisplayStyle.None;
            Debug.Log($"_isViewHidden = {_isViewHidden}");
            if (_isViewHidden) {
                _view.Show();
                FindObjectOfType<HudView>().Hide();
                return;
            }
            _view.Hide();
            FindObjectOfType<HudView>().Show();
        }

        public void Hide() {
            _view.Hide();
        }
    }
}