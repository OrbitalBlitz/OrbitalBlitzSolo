using System.Collections;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;

namespace OrbitalBlitz.Game.Features.Ship.Controllers {
    public class InertiaShipController : MonoBehaviour, IShipController {
        [SerializeField] private Rigidbody sphere;

        /* SPEED CONTROL*/
        private float speed = 0f; // player current speed
        [SerializeField] private float maxSpeed = 50f;
        [SerializeField] private float acceleration = 20f;

        [SerializeField] private float deceleration = 30f;

        [SerializeField] private float brakeForce = 50f;

        public bool isAccelerating = false; // True when the player is accelerating

        /* BOOST */
        [SerializeField] private bool canBoost = false;
        [SerializeField] private float boostMultiplier = 4f; // Multiplier to increase speed during boost
        [SerializeField] private float boostDuration = 1f; // Duration of the speed boost in seconds
        public bool isBoosting = false; // Flag to check if boost is active

        [SerializeField] private float postBoostDrag = 1f;
        [SerializeField] private float postBoostDragDuration = 1f;

        /* Steering (turning) */

        [SerializeField] private float steering = 10f; // Lower value for more steerability at low speeds.
        [SerializeField] private float minSteering = 70f; // Higher value for less steerability at high speeds.
        [SerializeField] private float steeringDelay = 0.5f;


        /** */
        [SerializeField] private LayerMask layerMask;

        [SerializeField] private float delay_normal = 8f;

        public float currentSpeed, velocity;
        public float inputSteering;
        public bool inputBraking;
        public float rotate, currentRotate;
        

        private ShipPhysicsState initialPhysicsState;
        private ShipPhysicsState lastCheckpointPhysicsState;

        /* Chronomètre ?? */
        private int timer = 0;
        private bool timerOn = false;

        private const float ZeroSpeedThreshold = 0.01f; // avoid having a direct comparison of float numbers to zero

        void Start() {
            initialPhysicsState = new() {
                Position = sphere.transform.position,
                Rotation = transform.rotation,
                Velocity = new(0f,0f,0f),
                AngularVelocity = new(0f,0f,0f),
            };
        }

        void Update() {
            //Follow Collider
            transform.position = sphere.transform.position;
            // HandleUserInputs();
        }

        void FixedUpdate() {
            UpdateSpeed();
            ApplyBrake();
            ApplySteering();
            AlignWithGround(); 
        }

        public void ActivateBlitz() {
            if (canBoost) StartCoroutine(Turbo());
        }

        public ShipPhysicsState GetCurrentPhysicsState() {
            return new() {
                Position = sphere.transform.position,
                Rotation = transform.rotation,
                Velocity = sphere.velocity,
                AngularVelocity = sphere.angularVelocity,
            };
        }

        public void setLastCheckpointPhysicsState(ShipPhysicsState state) {
            lastCheckpointPhysicsState = state;
        }

        public void SetIsKinematic(bool toggle) {
            sphere.isKinematic = toggle;
        }

        private void UpdateSpeed() {
            if (isAccelerating && speed < maxSpeed - ZeroSpeedThreshold)
                speed += acceleration * Time.fixedDeltaTime;
            else if (!isAccelerating && speed > ZeroSpeedThreshold)
                speed -= deceleration * Time.fixedDeltaTime;

            speed = Mathf.Clamp(speed, 0, maxSpeed); // Ensuring the speed is within the 0 to maxSpeed range

            if (isBoosting)
                speed = maxSpeed; // If boosting, set the speed to maxSpeed

            sphere.AddForce(transform.forward * speed, ForceMode.Acceleration);
        }

        void AlignWithGround() {
            currentRotate = Mathf.Lerp(currentRotate, rotate, Time.deltaTime);
            rotate = 0f;

            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 2.0f, layerMask)) {
                Quaternion groundNormalRotation = Quaternion.FromToRotation(transform.up, hit.normal);
                Quaternion steeringRotation = Quaternion.Euler(0, currentRotate, 0);

                transform.rotation = Quaternion.Slerp(transform.rotation, transform.rotation * steeringRotation,
                    steeringDelay);
                transform.rotation = Quaternion.Slerp(transform.rotation, groundNormalRotation * transform.rotation,
                    delay_normal);

                Debug.DrawRay(hit.point, hit.normal * 5, Color.green);
            }
        }

        public void Accelerate(float input) {
            if (input > 0)
                isAccelerating = true; // speed increases only when there is positive input
            else
                isAccelerating = false;
        }

        public void Steer(float input) {
            inputSteering = input;
        }

        private void ApplySteering() {
            if (speed > 0) // The kart can only be steered if it is moving.
            {
                // float steerFactor = Mathf.Lerp(minSteering, steering, speed / maxSpeed);
                // rotate = steerFactor * input;
                
                // a reverifier (adapter minSteering & steering)
                float steerFactor = Mathf.Lerp(steering, minSteering, (maxSpeed - speed) / maxSpeed);
                rotate = Mathf.Lerp(rotate, steerFactor * inputSteering, Time.deltaTime * 5f); // Added smoothing
            }
            else {
                rotate = 0; // The kart is not steerable if the speed is zero.
            }
            //Debug.Log("STEER : " + input);
            //rotate = steering * input;
        }

        public void Brake(int input) {
            inputBraking = input > 0;
        }

        private void ApplyBrake() {
            if (inputBraking) {
                speed -= brakeForce * Time.deltaTime; // Applying brake force
                if (speed < 0) speed = 0; // Ensure speed doesn't go below 0
            }
        }

        public void Respawn() {
            ResetShipToPhysicsState(initialPhysicsState);
            setLastCheckpointPhysicsState(initialPhysicsState);
        }
        
        public void RespawnToLastCheckpoint() {
            ResetShipToPhysicsState(lastCheckpointPhysicsState);
        }

        private void ResetShipToPhysicsState(ShipPhysicsState state) {
            sphere.transform.position = state.Position;
            transform.rotation  = state.Rotation;
            sphere.velocity = state.Velocity;
            sphere.angularVelocity = state.AngularVelocity;
            currentSpeed = 0;
            currentRotate = 0;
        }

        /// <summary>
        /// (BLITZ) Activate the turbo for the player for a duraction
        /// </summary>
        public IEnumerator Turbo() {
            canBoost = false;

            Debug.Log("starting turbo");
            isBoosting = true;
            float originalMaxSpeed = maxSpeed;
            float originalDrag = sphere.drag;

            maxSpeed *= boostMultiplier;

            yield return new WaitForSeconds(boostDuration);

            maxSpeed = originalMaxSpeed;

            // Increase drag to decelerate faster
            sphere.drag = postBoostDrag;

            yield return new WaitForSeconds(postBoostDragDuration);

            // Restore original drag
            sphere.drag = originalDrag;
            isBoosting = false;

            Debug.Log("stop turbo");
        }
    }

    [CustomEditor(typeof(InertiaShipController))]
    [CanEditMultipleObjects]
    class InertiaShipControllerEditor : Editor {

        void OnEnable() {
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            serializedObject.Update();
        }
    }
}
