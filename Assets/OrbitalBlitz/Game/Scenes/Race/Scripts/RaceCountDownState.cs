using UnityEngine;

namespace OrbitalBlitz.Game.Scenes.Race.Scripts {
    public class RaceCountDownState : RaceBaseState {
        private float _initialTime = 3f;
        private float _timeRemaining;
        private float _acc;
        private int _countDown;

        public override void UpdateState(RaceStateManager context) {
            if (isTimeOut(context))
                context.SwitchState(RaceStateManager.RaceState.RaceCountDown);
        }

        public override void EnterState(RaceStateManager context) {
            _timeRemaining = _initialTime;
        }

        private bool isTimeOut(RaceStateManager context) {
            _timeRemaining -= Time.deltaTime;
            _acc += Time.deltaTime;
            if (_acc >= 1) {
                Debug.Log($"{_initialTime - (_countDown += 1)} seconds left before race starts...", context);
                _acc = 0;
            }
            if (_timeRemaining < 0)
                return true;
            return false;
        }
    }
}