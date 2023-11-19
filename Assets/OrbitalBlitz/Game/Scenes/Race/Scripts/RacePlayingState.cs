using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OrbitalBlitz.Game.Scenes.Race.Scripts {
    public class RacePlayingState : RaceBaseState {
        public bool isRaceOver;
        private RaceStateManager _context;

        private List<Action<RaceStateManager, RacePlayingState>> RaceEndConditions = new() {
            ((context, state) => { state.isRaceOver = false; })
        };

        public override void UpdateState(RaceStateManager context) {
            foreach (Action<RaceStateManager, RacePlayingState> condition in RaceEndConditions) {
                condition(context, this);
            }
            if (isRaceOver) {
                context.SwitchState(RaceStateManager.RaceState.RaceEnded);
            }
        }

        public override void EnterState(RaceStateManager context) {
            base.EnterState(context);
            _context = context;
            Player.Singleton.Input.defaultMap.ToggleEscapeMenu.started += toggleEscapeMenuCallback;
        }

        public override void ExitState(RaceStateManager context) {
            base.ExitState(context);
            Player.Singleton.Input.defaultMap.ToggleEscapeMenu.started -= toggleEscapeMenuCallback;
            context.EscapeMenuController.Hide();
        }

        public void toggleEscapeMenuCallback(InputAction.CallbackContext callbackContext) {
                _context.EscapeMenuController.Toggle();
        }
    }
}