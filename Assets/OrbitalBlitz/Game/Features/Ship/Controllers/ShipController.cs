using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace OrbitalBlitz.Game.Features.Ship.Controllers {
    public class BaseShipController : AbstractShipController {

        [Header("Debug")]
        [SerializeField] public Logger Logger;
        [SerializeField] private bool toggleDebugControls   = false;
        [SerializeField] private float GuizmosScale         = 1;
        [SerializeField] private float GuizmosMicroScale    = 1;
        [SerializeField] private bool DrawWheelAxes         = true;
        [SerializeField] private bool DrawSteeringForces    = false;
        [SerializeField] private bool DrawSuspensionForces  = false;
        [SerializeField] private bool DrawAccelerationForces  = false;

        [Header("General")]
        [SerializeField] private float RaycastMaxDist       = 1;

        [Header("Acceleration")]
        [SerializeField] private bool UseAcceleration       = true;
        [SerializeField] private bool Traction              = true;
        [SerializeField] private bool Propulsion            = true;
        [SerializeField] private int maxSpeed               = 1;
        [SerializeField] private float AccelerationForce    = 1;

        [Header("Steering")]
        [SerializeField] private bool UseSteering           = true;
        [SerializeField] private float SteeringGrip         = 1;
        [SerializeField] private float tireMass             = 1;
        [SerializeField] private float degPerSec            = 90;
        [SerializeField] private float maxAngle             = 45;
        [SerializeField] private float steeringRelaxRate    = 0.1f;

        [Header("Brakes")]
        [SerializeField] private bool UseBrakes             = true;

        [Header("Suspensions")]
        [SerializeField] private bool UseSuspension         = true;
        [SerializeField] private bool UseDamping            = true;
        [SerializeField] private float SuspensionStrength       = 1;
        [SerializeField] private float SuspensionRestDistance   = 1f;
        [SerializeField] private float DampingForce             = 5;

        [Header("Wheels")]
        [SerializeField] private Transform LFWheel;
        [SerializeField] private Transform RFWheel;
        [SerializeField] private Transform RBWheel;
        [SerializeField] private Transform LBWheel;
        private Dictionary<Transform, float> previousOffsets = new Dictionary<Transform, float>();

        Rigidbody _rb;
        float _steering, _acceleration;
        bool _isBreaking;
        private PlayerInputActions playerInputActions;

        // Public methods
        public void Brake( bool isBraking ) {
            _isBreaking = isBraking;

        }
        public override void Steer( float input ) {
            _steering = input;
        }
        public override void Reset() {
            _steering = _acceleration = 0;
        }
        public override void Brake(int input) {
            // throw new NotImplementedException();
        }

        public override void Accelerate( float input ) {
            _acceleration = input;
        }

        public override float GetSpeed() {
            return _rb.velocity.magnitude;
        }

        // Start is called before the first frame update
        void Start() {
            _rb = transform.GetComponent<Rigidbody>();
            playerInputActions = new PlayerInputActions();
            playerInputActions.defaultMap.Enable();
        }

        // Update is called once per frame
        void FixedUpdate() {
            if( toggleDebugControls ) DebugControls();
            if( UseSuspension ) ApplySuspensions();
            if( UseAcceleration ) ApplyAcceleration();
            if( UseSteering ) ApplySteering();
            if( UseBrakes ) ApplyBrakes();
        }
        void DebugControls() {
            Vector2 inputVector = playerInputActions.defaultMap.Move.ReadValue<Vector2>();
            Accelerate( inputVector.y );
            Steer( inputVector.x );
        }

        void ApplyAcceleration() {
            if( Propulsion ) {
                ApplyAccelerationToWheel( RBWheel );
                ApplyAccelerationToWheel( LBWheel );
            }
            if( Traction ) {
                ApplyAccelerationToWheel( RFWheel );
                ApplyAccelerationToWheel( LFWheel );
            }
        }
        private void ApplyAccelerationToWheel( Transform wheel ) {
            RaycastHit hit;
            if( Physics.Raycast( wheel.position, -Vector3.up, out hit, RaycastMaxDist ) ) {
                Vector3 forwardDirection   = wheel.forward;
                float force = _acceleration * AccelerationForce;

                #if UNITY_EDITOR
                if( DrawWheelAxes ) {
                    Debug.DrawRay( wheel.position, forwardDirection * GuizmosScale, Color.blue );
                }
                if( DrawAccelerationForces ) {
                    Debug.DrawRay( wheel.position, forwardDirection * force * GuizmosScale, Color.blue );
                }
                #endif

                _rb.AddForceAtPosition( forwardDirection * force, wheel.position );
            }
        }

        void ApplySteering() {
            //float force =  _steering * AccelerationForce;
            //Debug.DrawRay( transform.position, transform.right * force, Color.blue );
            //_rb.AddForce( transform.right * force );
            SteerWheel( LFWheel );
            SteerWheel( RFWheel );
            ApplySteeringToWheel( LFWheel );
            ApplySteeringToWheel( RFWheel );
            ApplySteeringToWheel( LBWheel );
            ApplySteeringToWheel( RBWheel );
        }
        private void SteerWheel( Transform wheel ) {
            float targetRotation = _steering * maxAngle; // target is either -45 or 45
            float currentRotation = wheel.localEulerAngles.y;
            float newRotation = Mathf.MoveTowardsAngle(currentRotation, targetRotation, degPerSec * Time.deltaTime);
            wheel.localEulerAngles = new Vector3( wheel.localEulerAngles.x, newRotation, wheel.localEulerAngles.z );
        }

        private void ApplySteeringToWheel( Transform wheel ) {
            RaycastHit hit;
            if( Physics.Raycast( wheel.position, -Vector3.up, out hit, RaycastMaxDist ) ) {
                Vector3 driftingDirection   = wheel.right;
                Vector3 wheelWorldVelocity  = _rb.GetPointVelocity(wheel.position);
                float driftingForce         = Vector3.Dot(driftingDirection, wheelWorldVelocity);
                float desiredVelocityChange = - ( driftingForce * SteeringGrip);
                float desiredAccel          = desiredVelocityChange / Time.fixedDeltaTime;
                Vector3 force               = driftingDirection * tireMass * desiredAccel;

                #if UNITY_EDITOR
                if( DrawWheelAxes ) {
                    Debug.DrawRay( wheel.position, driftingDirection * GuizmosScale, Color.red );
                }
                if( DrawSteeringForces ) {
                    Debug.DrawRay( wheel.position, force * GuizmosScale, Color.red );
                }
                #endif
                //print( "Found an object at distance: " + hit.distance + ", applying force " + suspension );
                _rb.AddForceAtPosition( driftingDirection * tireMass * desiredAccel * Time.deltaTime, wheel.position );
            }
        }
        void ApplySuspensions() {
            ApplySuspensionToWheel( LFWheel );
            ApplySuspensionToWheel( RFWheel );
            ApplySuspensionToWheel( LBWheel );
            ApplySuspensionToWheel( RBWheel );
        }
        private void ApplySuspensionToWheel( Transform wheel ) {
            RaycastHit hit;
            if( Physics.Raycast( wheel.position, -Vector3.up, out hit, RaycastMaxDist ) ) {
                Vector3 springDir               = wheel.up;
                Vector3 wheelWorldVelocity      = _rb.GetPointVelocity(wheel.position);
                float wheelVelocity             = Vector3.Dot(springDir, wheelWorldVelocity);
                float damping                   = UseDamping ? (wheelVelocity * DampingForce) : 0f;

                float offset                    = SuspensionRestDistance - hit.distance;

                float suspension = (( offset * _rb.mass * SuspensionStrength ) - damping ) * Time.deltaTime;

                #if UNITY_EDITOR
                if( DrawWheelAxes ) {
                    Debug.DrawRay( wheel.position, springDir * GuizmosScale, Color.green );
                }
                if( DrawSuspensionForces ) {
                    Debug.DrawRay( wheel.position, springDir * suspension * GuizmosScale, Color.green );
                    Debug.DrawRay( wheel.transform.position, -wheel.up * hit.distance, Color.red ); ;
                }
                #endif
                //print( "Found an object at distance: " + hit.distance + ", applying force " + suspension );
                _rb.AddForceAtPosition( springDir * suspension, wheel.position );
            }
        }
        void ApplyBrakes() {

        }

        public override void ActivateBlitz() {
            throw new NotImplementedException();
        }

        public override event Action<Collider> onTriggerEnter;

        public override void SetIsKinematic(bool toggle) {
            _rb.isKinematic = toggle;
        }
    }
}
