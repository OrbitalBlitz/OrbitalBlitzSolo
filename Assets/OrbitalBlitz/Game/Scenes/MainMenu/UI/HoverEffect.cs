using UnityEngine;

namespace OrbitalBlitz.Game.Scenes.MainMenu.UI {
    public class HoverEffect : MonoBehaviour {
        public float hoverHeight = 0.5f; // The maximum height the object will hover.
        public float hoverSpeed = 2f; // The speed of the hover effect.

        private Vector3 startPosition;

        void Start() {
            startPosition = transform.localPosition;
        }

        void LateUpdate() {
            float newY = Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;
            transform.localPosition = new Vector3(startPosition.x, startPosition.y + newY, startPosition.z);
        }
    }
}