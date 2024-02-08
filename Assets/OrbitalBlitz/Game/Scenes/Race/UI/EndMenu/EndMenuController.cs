using System;
using OrbitalBlitz.Game.Scenes.Race.Scripts;
using UnityEngine;
using UnityEngine.UIElements;

namespace OrbitalBlitz.Game.Scenes.Race.UI.EndMenu {
    public class EndMenuController : MonoBehaviour, IHideableView {
        [SerializeField] private EndMenuView _view;
    
        private void Start() {
            _view.OnQuitClicked += () => { Loader.LoadScene(Loader.Scene.MainMenu); };
            _view.OnRestartClicked += () => {
                Hide();
                Player.Singleton.ShipController.Respawn(); 
                RaceStateManager.Instance.SwitchState(RaceStateManager.RaceState.RacePlaying);
            };
            
            Hide();
        }
        
        public void Show() {
            FindObjectOfType<HudView>().Hide();
            var timespan = TimeSpan.FromSeconds(Player.Singleton.RaceInfo.timer);            
            _view.time.text = timespan.ToString(@"mm\'ss\'\'ff\'\'\'");
            _view.Show();
        }

        public void Hide() {
            FindObjectOfType<HudView>().Show();
            _view.Hide();
        }
    }
}