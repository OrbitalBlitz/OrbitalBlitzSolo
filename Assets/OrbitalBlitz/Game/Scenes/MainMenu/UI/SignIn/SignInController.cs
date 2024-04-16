using System;
using OrbitalBlitz.Game.Scenes.Race.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace OrbitalBlitz.Game.Scenes.MainMenu.UI {
    public class SignInController : MonoBehaviour {
        [SerializeField] private SignInView view;

        private void Start() {
            view.OnSignInClicked += handleSignInClicked;
            UserSession.Instance.onSignInError += handleOnSignInError; 
            UserSession.Instance.OnUserSignedIn += handleOnUserSignedIn; 
        }

        private void OnDestroy() {
            UserSession.Instance.onSignInError -= handleOnSignInError; 
            UserSession.Instance.OnUserSignedIn -= handleOnUserSignedIn; 
        }

        private void handleOnSignInError(string error) {
            view.SetError(error);
            view.ResetFields();
        }

        private void handleOnUserSignedIn() {
            view.Hide();
            FindObjectOfType<MainMenuView>().Show();
        }
        private void handleSignInClicked(string username, string password, string password_conf) {
            if (password_conf != password) {
                view.SetError("Passwords do not match.");
                return;
            }

            StartCoroutine(UserSession.Instance.SignIn(username, password));
        }
    }
}