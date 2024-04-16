using System;
using OrbitalBlitz.Game.Scenes.Race.UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace OrbitalBlitz.Game.Scenes.MainMenu.UI {
    public class MainMenuView : MonoBehaviour, IHideableView {
        public Button PlayButton;
        public Button loginButton;
        public Button signinButton;
        public Button disconnectButton;

        private Label txt_message;

        public event Action OnPlayClicked;
        public event Action OnDisconnectClicked;
        public event Action OnLogInClicked;
        public event Action OnSignInClicked;

        private void Awake() {
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;
            
            PlayButton = root.Q<Button>("btn_play");
            PlayButton.clicked += () => { OnPlayClicked?.Invoke(); };
            
            disconnectButton = root.Q<Button>("btn_disconnect");
            disconnectButton.clicked += () => { OnDisconnectClicked?.Invoke(); };
            
            loginButton = root.Q<Button>("btn_login");
            loginButton.clicked += () => {
                OnLogInClicked?.Invoke();
                Hide();
                FindObjectOfType<LogInView>().Show();
            };
            
            signinButton = root.Q<Button>("btn_signin");
            signinButton.clicked += () => {
                OnSignInClicked?.Invoke();
                Hide();
                FindObjectOfType<SignInView>().Show();
            };

            txt_message = root.Q<Label>("text_message");
        }

        public void SetMessage(string msg) {
            if (String.IsNullOrEmpty(msg)) {
                txt_message.style.display = DisplayStyle.None;
                return;
            }
            txt_message.text = msg;
            txt_message.style.display = DisplayStyle.Flex;
        }
        public void Show() {
            GetComponent<UIDocument>()
                .rootVisualElement
                .style
                .display = DisplayStyle.Flex;
        }

        public void Hide() {
            GetComponent<UIDocument>()
                .rootVisualElement
                .style
                .display = DisplayStyle.None;
        }
    }
}