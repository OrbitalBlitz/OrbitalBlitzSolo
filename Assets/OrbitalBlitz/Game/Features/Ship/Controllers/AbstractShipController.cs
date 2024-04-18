using System;
using UnityEngine;

namespace OrbitalBlitz.Game.Features.Ship.Controllers {
    public abstract class AbstractShipController : MonoBehaviour {
        public Rigidbody RB;
        public float max_speed_forward;
        public bool is_drifting;
        public bool is_braking;
        public bool IsHuman = true;
        public abstract float GetSpeed();
        public abstract void Accelerate(float input);
        public abstract void Steer(float input);
        public abstract void Brake(int input);
        public abstract void Reset();
        public abstract void ActivateBlitz();

        public abstract event Action<Collider> onTriggerEnter;

        public abstract void SetIsKinematic(bool toggle);
    }
}
