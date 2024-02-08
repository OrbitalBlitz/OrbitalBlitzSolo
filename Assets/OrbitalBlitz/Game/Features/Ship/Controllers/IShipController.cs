using UnityEngine;

namespace OrbitalBlitz.Game.Features.Ship.Controllers {
    public struct ShipPhysicsState {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Velocity;
        public Vector3 AngularVelocity;
    }

    public interface IShipController {
        public float GetSpeed();
        public void Accelerate(float input);
        public void Steer(float input);
        public void Brake(int input);
        public void Respawn();
        public void RespawnToLastCheckpoint();
        public void ActivateBlitz();
        
        public ShipPhysicsState GetCurrentPhysicsState();
        public void setLastCheckpointPhysicsState(ShipPhysicsState state);

        public void SetIsKinematic(bool toggle);
    }
}
