using System;
using System.Collections.Generic;
using OrbitalBlitz.Game.Features.Player;
using Unity.VisualScripting;
using UnityEngine;


namespace OrbitalBlitz.Game.Scenes.Race.Scripts {
    public class RacePlayingState : RaceBaseState {
        public bool isRaceOver;
        private RaceStateManager _context;

        private List<Action<RaceStateManager, RacePlayingState>> RaceEndConditions = new() {
            ((context, state) => {
                if (RaceStateManager.Instance.HumanPlayer.Info.hasFinished)
                    state.isRaceOver = true;
            })
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
            isRaceOver = false;
            _context = context;
            // PlayerSingleton.Singleton.ShipController.SetIsKinematic(false);
        }

        public override void ExitState(RaceStateManager context) {
            base.ExitState(context);
            context.EscapeMenuController.Hide();
        }

        
    }
}