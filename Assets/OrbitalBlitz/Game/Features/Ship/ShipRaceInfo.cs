#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace OrbitalBlitz.Game.Features.Ship {
    public class ShipRaceInfo : MonoBehaviour {
        public int lastCheckpoint;
        public float timer;
        public int lap;
        public bool hasFinished = false;

        public void Reset() {
            timer = 0f;
            lastCheckpoint = 0;
            lap = 1;
            hasFinished = false;
        }

        private void FixedUpdate() {
            timer += Time.deltaTime;
        }

        #if UNITY_EDITOR
        private void OnDrawGizmos() {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = hasFinished ? UnityEngine.Color.green : UnityEngine.Color.red;
            Handles.Label(transform.position + Vector3.up * 2,
                "cp " + lastCheckpoint.ToString() + ",lap " + lap.ToString(), style);
        }
        #endif
    }
}