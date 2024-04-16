using System;
using JetBrains.Annotations;
using OrbitalBlitz.Game.Scenes.Race.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace OrbitalBlitz.Game.Scenes.MainMenu.UI {
    public class SignInView : MonoBehaviour, IHideableView {
        private Button signInButton;
        private Button backButton;
        
        private TextField field_username;
        private TextField field_password;
        private TextField field_password_conf;

        private Label txt_error;

        public event Action<string, string, string> OnSignInClicked;
        public event Action OnBackClicked;

        private void Awake() {
            Hide();
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;

            field_username = root.Q<TextField>("field_username");
            field_password = root.Q<TextField>("field_password");
            field_password_conf = root.Q<TextField>("field_password_conf");
            
            signInButton = root.Q<Button>("btn_signin");
            signInButton.clicked += () => {
                OnSignInClicked?.Invoke(
                    field_username.text,
                    field_password.text,
                    field_password_conf.text);
            };

            backButton = root.Q<Button>("btn_back");
            backButton.clicked += () => {
                OnBackClicked?.Invoke();
                Hide();
                FindObjectOfType<MainMenuView>().Show();
            };

            txt_error = root.Q<Label>("txt_error");
        }

        public void SetError([CanBeNull] string error) {
            if (error == null || error == "") {
                txt_error.style.display = DisplayStyle.None;
                return;
            }
            txt_error.text = error;
            txt_error.style.display = DisplayStyle.Flex;
        }

        public void ResetFields() {
            field_username.SetValueWithoutNotify(""); 
            field_password.SetValueWithoutNotify(""); 
            field_password_conf.SetValueWithoutNotify(""); 
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