using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace OrbitalBlitz.Game.Scenes.Race.UI.EscapeMenu {
    public class EscapeMenuView : MonoBehaviour, IHideableView {
        private Button restart_Button;
        private Button resume_Button;
        private Button respawn_Button;
        private Button quit_Button;

        public event Action OnRestartClicked;
        public event Action OnResumeClicked;
        public event Action OnRespawnClicked;
        public event Action OnQuitClicked;


        private void Awake() {
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;
            restart_Button = root.Q<Button>("btn_restart");
            resume_Button = root.Q<Button>("btn_resume");
            respawn_Button = root.Q<Button>("btn_respawn");
            quit_Button = root.Q<Button>("btn_quit");

            restart_Button.clicked += () => { OnRestartClicked?.Invoke(); };
            resume_Button.clicked += () => { OnResumeClicked?.Invoke(); };
            respawn_Button.clicked += () => { OnRespawnClicked?.Invoke(); };
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