using UnityEngine;
using UnityEngine.UIElements;

namespace OrbitalBlitz.Game.Scenes.Race.UI.EndMenu {
    public class EndMenuController : MonoBehaviour, IHideableView {
        [SerializeField] private EndMenuView _view;
    
        private void Start() {
        }
        private void OnDestroy() {
        }

        public void Show() {
            _view.Show();
        }

        public void Hide() {
            _view.Hide();
        }
    }
}