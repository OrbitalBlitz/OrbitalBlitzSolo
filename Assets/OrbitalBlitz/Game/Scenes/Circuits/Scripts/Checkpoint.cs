using OrbitalBlitz.Game.Features.Ship.Controllers;
using UnityEngine;
using UnityEngine.Serialization;

namespace OrbitalBlitz.Game.Scenes.Circuits.Scripts {
    public class Checkpoint : MonoBehaviour {
        public delegate void OnShipEnter(Checkpoint checkpoint, GameObject go);

        public event OnShipEnter onShipEnter;
        [FormerlySerializedAs("circuitData")] [FormerlySerializedAs("circuitManager")] [SerializeField] Circuit circuit;

        private void OnTriggerEnter(Collider other) {
            BaseShipController base_ship_controller = other.transform.parent.GetComponentInChildren<BaseShipController>();
            if (base_ship_controller == null) {
                return;
            }
            onShipEnter?.Invoke(this, other.transform.parent.gameObject);
        }
    }
}