using OrbitalBlitz.Game.Features.Ship.Controllers;
using UnityEngine;
using UnityEngine.Serialization;

namespace OrbitalBlitz.Game.Scenes.Circuits.Scripts {
    public class Checkpoint : MonoBehaviour {
        public delegate void OnShipEnter(Checkpoint checkpoint, GameObject go);

        public event OnShipEnter onShipEnter;
        [FormerlySerializedAs("circuitManager")] [SerializeField] CircuitData circuitData;

        private void OnTriggerEnter(Collider other) {
            IShipController shipController = other.transform.parent.GetComponentInChildren<IShipController>();
            if (shipController == null) {
                return;
            }
            onShipEnter?.Invoke(this, other.transform.parent.gameObject);
        }
    }
}