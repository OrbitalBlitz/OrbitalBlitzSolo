using UnityEngine;

namespace OrbitalBlitz.Game.Scenes.Race.Scripts {
    public class RaceCountDownState : RaceBaseState {
        private float _timeRemaining;
        private float _acc;
        private int _countDown;

        public override void UpdateState(RaceStateManager context) {
            if (isTimeOut(context)) {
                context.SwitchState(RaceStateManager.RaceState.RacePlaying);
                Player.Singleton.RaceInfo.Reset();
            }
        }

        public override void EnterState(RaceStateManager context) {
            _timeRemaining = context.CountDownSeconds;
        }

        private bool isTimeOut(RaceStateManager context) {
            _timeRemaining -= Time.deltaTime;
            Debug.Log($"{_timeRemaining} seconds left before race starts...", context);
            if (_timeRemaining < 0)
                return true;
            return false;
        }
    }
}