using UnityEngine;
using UnityEngine.UIElements;
using System;

namespace OrbitalBlitz.Game.Scenes.Race.UI.EndMenu {
    public class EndMenuView : MonoBehaviour, IHideableView {
        private Button restart_Button;
        private Button quit_Button;

        public event Action OnRestartClicked;
        public event Action OnQuitClicked;
        private void Awake() {
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;
            restart_Button = root.Q<Button>("btn_restart");
            quit_Button = root.Q<Button>("btn_quit");
            
            restart_Button.clicked += () => { OnRestartClicked?.Invoke(); };
            quit_Button.clicked += () => { OnQuitClicked?.Invoke(); };
        }
        
        public void Show() {
            GetComponent<UIDocument>().rootVisualElement.style.display = DisplayStyle.Flex;
        }

        public void Hide() {
            GetComponent<UIDocument>().rootVisualElement.style.display = DisplayStyle.None;
        }
    }
}