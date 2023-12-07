using UnityEditor;
using UnityEngine;

namespace OrbitalBlitz.Game.Features.Ship {
    public class ShipRaceInfo : MonoBehaviour
    {
        public int lastCheckpoint = 0;
        public int lap = 1;
        public bool hasFinished = false;

        public void Reset() {
            lastCheckpoint = 0;
            lap  = 1;
            hasFinished = false;
        }

        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = hasFinished ? UnityEngine.Color.green : UnityEngine.Color.red;
            Handles.Label(transform.position + Vector3.up * 2, "cp " + lastCheckpoint.ToString() + ",lap " + lap.ToString(), style);
        }
        #endif
    }
}
