using OrbitalBlitz.Game.Features.Ship.Controllers;
using OrbitalBlitz.Game.Scenes.Race.Scripts;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace OrbitalBlitz.Game.Scenes.Circuits.Scripts {
    public class Checkpoint : MonoBehaviour {
        public delegate void OnShipEnter(Checkpoint checkpoint, GameObject go);

        public event OnShipEnter onShipEnter;
        [FormerlySerializedAs("circuitManager")] [SerializeField] CircuitData circuitData;


        // #if UNITY_EDITOR
        // private void OnDrawGizmos() {
        //     if (Application.isPlaying) {
        //         GUIStyle style = new GUIStyle();
        //         style.normal.textColor = UnityEngine.Color.green;
        //         Handles.Label(transform.position + Vector3.up * 2, circuitData.Checkpoints.IndexOf(this).ToString(), style);
        //     }
        // }
        // #endif
        private void OnTriggerEnter(Collider other) {
            IShipController shipController = other.transform.parent.GetComponentInChildren<IShipController>();
            if (shipController == null) {
                return;
            }
            onShipEnter?.Invoke(this, other.transform.parent.gameObject);
        }
    }
}