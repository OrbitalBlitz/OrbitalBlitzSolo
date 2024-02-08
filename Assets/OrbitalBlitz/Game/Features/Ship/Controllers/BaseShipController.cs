using UnityEngine;

namespace OrbitalBlitz.Game.Features.Ship.Controllers {
    public class BaseShipController : MonoBehaviour, IShipController {
        private Rigidbody _rigidbody;
    
        private ShipPhysicsState _initialPhysicsState;
        private ShipPhysicsState _lastCheckpointPhysicsState;
    
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void Accelerate(float input) {
            throw new System.NotImplementedException();
        }

        public void Steer(float input) {
            throw new System.NotImplementedException();
        }
        public float GetSpeed() {
            return _rigidbody.velocity.magnitude;
        }

        public void Brake(int input) {
            throw new System.NotImplementedException();
        }

        public void Respawn() {
            ResetShipToPhysicsState(_initialPhysicsState);
            setLastCheckpointPhysicsState(_initialPhysicsState);
        }
        
        public void RespawnToLastCheckpoint() {
            ResetShipToPhysicsState(_lastCheckpointPhysicsState);
        }

        private void ResetShipToPhysicsState(ShipPhysicsState state) {
            _rigidbody.transform.position = state.Position;
            transform.rotation  = state.Rotation;
            _rigidbody.velocity = state.Velocity;
            _rigidbody.angularVelocity = state.AngularVelocity;
        }

        public void ActivateBlitz() {
            throw new System.NotImplementedException();
        }

        public ShipPhysicsState GetCurrentPhysicsState() {
            throw new System.NotImplementedException();
        }

        public void setLastCheckpointPhysicsState(ShipPhysicsState state) {
            throw new System.NotImplementedException();
        }

        public void SetIsKinematic(bool toggle) {
            _rigidbody.isKinematic = toggle;
        }
    }
}
