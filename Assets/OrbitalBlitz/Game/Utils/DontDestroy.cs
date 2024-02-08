using UnityEngine;

namespace OrbitalBlitz.Game.Utils {
    public class DontDestroy : MonoBehaviour {
        void Awake() {
            DontDestroyOnLoad(gameObject);
        }
    }
}