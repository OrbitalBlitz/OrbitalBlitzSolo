using System;
using JetBrains.Annotations;
using OrbitalBlitz.Game.Scenes.Race.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace OrbitalBlitz.Game.Scenes.MainMenu.UI {
    public class LogInView : MonoBehaviour, IHideableView {
        private Button loginButton;
        private Button backButton;

        private TextField field_password;
        private TextField field_username;
        
        private Label txt_error;

        public event Action<string, string> OnLogInClicked;
        public event Action OnBackClicked;

        private void Awake() {
            Hide();
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;

            field_password = root.Q<TextField>("field_password");
            field_username = root.Q<TextField>("field_username");
            
            loginButton = root.Q<Button>("btn_login");
            loginButton.clicked += () => {
                OnLogInClicked?.Invoke(field_username.text ,field_password.text);
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