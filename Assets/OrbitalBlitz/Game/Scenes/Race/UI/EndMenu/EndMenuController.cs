using UnityEngine;
using UnityEngine.UIElements;

namespace OrbitalBlitz.Game.Scenes.Race.UI.EndMenu {
    public class EndMenuController : MonoBehaviour, IHideableView {
        [SerializeField] private EndMenuView _view;
    
        private void Start() {
            _view.OnQuitClicked += () => { Loader.LoadScene(Loader.Scene.MainMenu); };
            _view.OnRestartClicked += () => {
                Hide();
                Player.Singleton.RaceInfo.Reset();
                Player.Singleton.ShipController.Respawn();                
            };
            
            Hide();
        }
        
        public void Show() {
            _view.Show();
        }

        public void Hide() {
            _view.Hide();
        }
    }
}