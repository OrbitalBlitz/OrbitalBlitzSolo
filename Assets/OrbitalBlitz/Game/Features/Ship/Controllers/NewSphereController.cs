using System;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace OrbitalBlitz.Game.Features.Ship.Controllers {
    public class NewSphereController : MonoBehaviour, IShipController
    {
        [Header("Objects")]
        [SerializeField] private Rigidbody sphere;
        [SerializeField] private ParticleSystem acceleration_particles;
        [SerializeField] private LayerMask layerMask;

        [Header("Stats")]
        [SerializeField] private float max_speed_forward = 50f;
        [SerializeField] private float max_speed_backward = 30f;
        [SerializeField] private float acceleration_stat = 20f;
        [SerializeField] private float deceleration_stat = 70f;    
        [SerializeField] private float brake_force_stat = 100f;
        [SerializeField] private float boost_power = 100f;
        [SerializeField] private float boost_duration = 10f;
        [SerializeField] private float steering = 5f;

        [Header("Data visualisation")]
        [SerializeField] private float currentSpeed = 0f; // current speed of the player
        [SerializeField] private float targetSpeed = 0f;
        [SerializeField] private Vector3 sphere_velocity;
        [SerializeField] private float blitzTimer = 0;

        [Header("Lerp & delay")]
        [SerializeField] private float steeringLerpFactor = 2f;
        [SerializeField] private float stopSteeringLerpFactor = 5f;
        [SerializeField] private float delay_normal = 0.1f;


        string acceleration_mod = "mario";
        float acceleration_input = 0f; 
        bool isBraking = false;
        float velocity;
        float rotate, currentRotate;
        private PlayerInputActions playerInputActions;
        private Vector3 initialPosition;
        private Quaternion initialRotation;

        private ShipPhysicsState initialPhysicsState;
        private ShipPhysicsState lastCheckpointPhysicsState;    

        bool isForward;
        bool timerOn = false;
        int timer = 0;

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

            // // dev input
            // float inputV = Input.GetAxis("Vertical");
            // Accelerate(inputV);
            
            // float inputH = Input.GetAxis("Horizontal");
            // Steer(inputH);
            
            // bool inputB = Input.GetKey(KeyCode.Space);
            // Brake(inputB);
            
            // if (Input.GetKeyUp(KeyCode.R))
            // {
            //     Respawn();
            // }
            
            // if (Input.GetKeyUp(KeyCode.B)) {
            //     ActivateBlitz();
            // }
        }

        void FixedUpdate() {
            //Acceleration
            UpdateSpeed();

            //Steering
            UpdateRotate();

            UpdateTilt();

            UpdateBlitzTimer();

            UpdateParticles();

            sphere_velocity = sphere.velocity;
        }

        public void Accelerate(float input)
        {
            acceleration_input = input;
        }

        private void UpdateSpeed() {
            if (!isBraking) {
                // Calculate the desired target speed based on input
                switch (acceleration_input) {
                    case > 0:
                        targetSpeed = max_speed_forward;
                        break;
                    case < 0:
                        targetSpeed = max_speed_backward;
                        break;
                    case 0:
                        targetSpeed = 0;
                        break;
                }
        
                // Adjust the current speed toward the target
                if (targetSpeed > currentSpeed)
                {
                    currentSpeed += acceleration_stat * Time.fixedDeltaTime;
                }
                else if (targetSpeed < currentSpeed)
                {
                    currentSpeed -= deceleration_stat * Time.fixedDeltaTime;
                }
            } else {
                currentSpeed -= brake_force_stat * Time.fixedDeltaTime;
            }

            currentSpeed = Mathf.Clamp(currentSpeed, 0, Mathf.Infinity);

            if (blitzTimer > 0) {
                // AddForce if the blitz is active
                currentSpeed = boost_power;
                sphere.AddForce(transform.forward * currentSpeed, ForceMode.Acceleration);
            } else if (currentSpeed > 0) {
                if (!isBraking) {
                    // Accelerate the vehicule
                    sphere.AddForce(transform.forward * currentSpeed * acceleration_input, ForceMode.Acceleration);
                } else {
                    // decelerate the vehicule if breaking
                    sphere.AddForce(-sphere.velocity.normalized * currentSpeed, ForceMode.Acceleration);
                }
            } else if (isBraking) {
                // pass the vehicule velocity to 0 if currentSpeed is null and the user is breaking, the goal is to completely stop the movement of the vehicule
                sphere.velocity = Vector3.Lerp(sphere.velocity, Vector3.zero, delay_normal);
            }
        }

        public void Steer(float input)
        {
            rotate = steering * input;
        }

        private void UpdateRotate()
        {
            float adjustedLerpingFactor = (rotate != 0f) ? steeringLerpFactor : stopSteeringLerpFactor;
            currentRotate = Mathf.Lerp(currentRotate, rotate, adjustedLerpingFactor * Time.fixedDeltaTime);
        
            transform.rotation = transform.rotation * Quaternion.Euler(0, currentRotate, 0);
        }

        private void UpdateTilt()
        {
            RaycastHit hit;
            Physics.Raycast(transform.position, Vector3.down, out hit, 2.0f, layerMask);
        
            Quaternion groundNormalRotation = Quaternion.FromToRotation(transform.up, hit.normal);

            transform.rotation = Quaternion.Lerp(transform.rotation, groundNormalRotation * transform.rotation, delay_normal);
        }

        public void Brake(int input) {
            isBraking = input > 0;
        }

        public void ActivateBlitz() {
            blitzTimer = boost_duration * 10;
        }

        private void UpdateBlitzTimer() {
            if (blitzTimer > 0) {
                blitzTimer --;
            }
        }

        public void Respawn()
        {
            ResetShipToPhysicsState(initialPhysicsState);
            setLastCheckpointPhysicsState(initialPhysicsState);
        }

        public void RespawnToLastCheckpoint() {
            ResetShipToPhysicsState(lastCheckpointPhysicsState);
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
    
        private void ResetShipToPhysicsState(ShipPhysicsState state) {
            sphere.transform.position = state.Position;
            transform.rotation  = state.Rotation;
            sphere.velocity = state.Velocity;
            sphere.angularVelocity = state.AngularVelocity;
            currentSpeed = 0;
            currentRotate = 0;
        }

        public void SetIsKinematic(bool toggle) {
            sphere.isKinematic = toggle;;
        }

        public void UpdateParticles()
        {
            ParticleSystem.EmissionModule acceleration_emission = acceleration_particles.emission;

            if (blitzTimer > 0) {
                acceleration_emission.rateOverTime = 100;
            } else if (currentSpeed > max_speed_forward - 10) {
                acceleration_emission.rateOverTime = 30;
            } else {
                acceleration_emission.rateOverTime = 0;
            }
        }
    }
}