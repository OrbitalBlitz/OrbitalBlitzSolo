using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace OrbitalBlitz.Game.Scenes.MainMenu.UI {
    public class MainMenuView : MonoBehaviour {
        private Button playButton;

        public event Action OnPlayClicked;

        private void Awake() {
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;
            playButton = root.Q<Button>("btn_play");
            playButton.clicked += () => { OnPlayClicked?.Invoke(); };
        }
    }
}