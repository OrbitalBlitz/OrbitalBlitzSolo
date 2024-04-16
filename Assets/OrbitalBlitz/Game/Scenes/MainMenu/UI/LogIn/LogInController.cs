using System;
using OrbitalBlitz.Game.Scenes.Race.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace OrbitalBlitz.Game.Scenes.MainMenu.UI {
    public class LogInController : MonoBehaviour {
        [SerializeField] private LogInView view;
    
        private void Start() {
            view.OnLogInClicked += handleLogInClicked;
            UserSession.Instance.OnUserConnected += handleOnUserConnected;
            UserSession.Instance.onLogInError += handleOnLogInError;
        }

        private void OnDestroy() {
            UserSession.Instance.OnUserConnected -= handleOnUserConnected;
            UserSession.Instance.onLogInError -= handleOnLogInError;
        }

        private void handleOnUserConnected() {
            view.Hide();
            FindObjectOfType<MainMenuView>().Show();
        }

        private void handleOnLogInError(string error) {
            view.SetError(error);
            view.ResetFields();
        }

        private void handleLogInClicked(string username, string password) {
            StartCoroutine(UserSession.Instance.LogIn(username, password));
            view.SetError(null);
        }
    }
}