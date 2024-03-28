using System;
using UnityEngine;

namespace OrbitalBlitz.Game.Features.Ship.Controllers {
    public abstract class AbstractShipController : MonoBehaviour {
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
