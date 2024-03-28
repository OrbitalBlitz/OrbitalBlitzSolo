using OrbitalBlitz.Game.Features.Ship;
using OrbitalBlitz.Game.Features.Ship.Controllers;
using UnityEngine;

namespace OrbitalBlitz.Game.Features.Player {
    public class PlayerSingleton : MonoBehaviour {
        public static PlayerSingleton Singleton;
        public PlayerInputActions Input;
        public BaseShipController BaseShipController;
        public PlayerInfo RaceInfo;

        private void Awake() {
            Singleton = this;
            Input = new PlayerInputActions();
            Input.defaultMap.Enable();
        }
    }
}