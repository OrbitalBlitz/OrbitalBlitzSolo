using UnityEngine;
using UnityEngine.UIElements;

namespace OrbitalBlitz.Game.Scenes.Race.UI.EndMenu {
    public class EndMenuView : MonoBehaviour, IHideableView {
        private void Awake() {
        }
        
        public void Show() {
            GetComponent<UIDocument>().rootVisualElement.style.display = DisplayStyle.Flex;
        }

        public void Hide() {
            GetComponent<UIDocument>().rootVisualElement.style.display = DisplayStyle.None;
        }
    }
}