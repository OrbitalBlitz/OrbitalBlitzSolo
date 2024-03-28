using System;
using OrbitalBlitz.Game.Features.Player;
using OrbitalBlitz.Game.Scenes.Race.Scripts;
using UnityEngine;
using UnityEngine.UIElements;

namespace OrbitalBlitz.Game.Scenes.Race.UI.EndMenu {
    public class EndMenuController : MonoBehaviour, IHideableView {
        [SerializeField] private EndMenuView _view;

        public void Start() {
            _view.OnQuitClicked += () => { Loader.LoadScene(Loader.Scene.MainMenu); };
            _view.OnRestartClicked += () => {
                Hide();
                RaceStateManager.Instance.HumanPlayer.Respawn(); 
                RaceStateManager.Instance.SwitchState(RaceStateManager.RaceState.RacePlaying);
            };
            
            Hide();
        }
        
        public void Show() {
            FindObjectOfType<HudView>().Hide();
            var timespan = TimeSpan.FromSeconds(RaceStateManager.Instance.HumanPlayer.Info.timer);            
            _view.time.text = timespan.ToString(@"mm\'ss\'\'ff\'\'\'");
            _view.Show();
        }

        public void Hide() {
            FindObjectOfType<HudView>().Show();
            _view.Hide();
        }
    }
}