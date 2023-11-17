using OrbitalBlitz.Game.Features.Ship.Controllers;
using UnityEditor;
using UnityEngine;

namespace OrbitalBlitz.Game.Scenes.Circuits.Scripts {
    public class Checkpoint : MonoBehaviour {
        public delegate void OnShipEnter(Checkpoint checkpoint, GameObject go);

        public event OnShipEnter onShipEnter;


        private void Awake() { }

        #if UNITY_EDITOR
        private void OnDrawGizmos() {
            if (Application.isPlaying) {
                CircuitManager circuitManager = CircuitManager.Instance;
                GUIStyle style = new GUIStyle();
                style.normal.textColor = UnityEngine.Color.green;
                Handles.Label(transform.position + Vector3.up * 2, circuitManager.Checkpoints.IndexOf(this).ToString(),
                    style);
            }
        }
        #endif
        private void OnTriggerEnter(Collider other) {
            IShipController shipController = other.transform.parent.GetComponentInChildren<IShipController>();
            if (shipController == null) {
                return;
            }
            onShipEnter?.Invoke(this, other.transform.parent.gameObject);
        }
    }
}