using System.Collections.Generic;
using UnityEngine;

namespace OrbitalBlitz.Game.Utils {
    public class DontDestroyOnLoadTagged : MonoBehaviour {
        private static List<string> dontDestroyTags = new List<string>();

        void Awake() {
            if (!dontDestroyTags.Contains(gameObject.tag)) {
                DontDestroyOnLoad(gameObject);
                dontDestroyTags.Add(gameObject.tag);
            } else {
                Destroy(gameObject);
            }
        }
    }
}