using System;
using OrbitalBlitz.Game.Scenes.Race.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace OrbitalBlitz.Game.Scenes.MainMenu.UI {
    public class MainMenuController : MonoBehaviour {
        [SerializeField] private MainMenuView view;

        private void Awake() {
            view.OnPlayClicked += goToSelectScene;
            
            view.OnDisconnectClicked += handleUserDisconnected;

        }

        private void Start() {
            UserSession.Instance.OnUserSignedIn += handleUserSignedIn;
            UserSession.Instance.OnUserConnected += handleUserConnected;
            if (UserSession.Instance.IsConnected())
                handleUserConnected();
        }

        private void OnDestroy() {
            view.OnPlayClicked += goToSelectScene;
            UserSession.Instance.OnUserSignedIn -= handleUserSignedIn;
            UserSession.Instance.OnUserConnected -= handleUserConnected;

        }

        private void goToSelectScene() {
            Loader.LoadScene(Loader.Scene.SelectCircuit);
            // Loader.LoadScene(Loader.Scene.Race, LoadSceneMode.Additive);
        }

        private void handleUserSignedIn() {
            view.SetMessage("You signed in ! You can log in.");
        }
        
        private void handleUserConnected() {
            view.PlayButton.text = $"Play as {UserSession.Instance.username}";
            view.loginButton.style.display = DisplayStyle.None;
            view.signinButton.style.display = DisplayStyle.None;
            view.disconnectButton.style.display = DisplayStyle.Flex;
        }
        
        private void handleUserDisconnected() {
            UserSession.Instance.Disconnect();
            view.PlayButton.text = $"Play as guest";
            view.loginButton.style.display = DisplayStyle.Flex;
            view.signinButton.style.display = DisplayStyle.Flex;
            view.disconnectButton.style.display = DisplayStyle.None;
        }
    }
}

