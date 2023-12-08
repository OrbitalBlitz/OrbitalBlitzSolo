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
        [SerializeField] private float boosterTimer = 0;

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
            
            // playerInputActions = new PlayerInputActions();
            // playerInputActions.defaultMap.Enable();
            // initialPosition = sphere.transform.position;
            // initialRotation = transform.rotation;
        }

        void Update() {
            //Follow Collider
            transform.position = sphere.transform.position;

            // // dev input
            // float inputV = Input.GetAxis("Vertical");
            // Accelerate(inputV);
            //
            // float inputH = Input.GetAxis("Horizontal");
            // Steer(inputH);
            //
            // bool inputB = Input.GetKey(KeyCode.Space);
            // Brake(inputB);
            //
            // if (Input.GetKeyUp(KeyCode.R))
            // {
            //     Respawn();
            // }
            //
            // if (Input.GetKeyUp(KeyCode.B)) {
            //     ActivateBlitz();
            // }
        }

        void FixedUpdate() {
            //Acceleration
            switch(acceleration_mod) {
                case "smooth":
                    UpdateSpeedSmooth();
                    break;
                case "linear":
                    UpdateSpeedLinear();
                    break;
                case "mario":
                    UpdateSpeedMario();
                    break;
            }        

            //Steering
            UpdateRotate();

            UpdateTilt();

            // UpdateBrake();

            UpdateBoosterTimer();

            UpdateParticles();

            sphere_velocity = sphere.velocity;
        }

        public void Accelerate(float input)
        {
            acceleration_input = input;
        }

        private void UpdateSpeedSmooth() {
            switch (acceleration_input) {
                case > 0:
                    currentSpeed = Mathf.SmoothDamp(currentSpeed, max_speed_forward, ref velocity, 1, acceleration_stat);
                    break;
                case < 0:
                    currentSpeed = Mathf.SmoothDamp(currentSpeed, -max_speed_backward, ref velocity, 1, acceleration_stat);
                    break;
                case 0:
                    currentSpeed = Mathf.SmoothDamp(currentSpeed, 0, ref velocity, 1, deceleration_stat);
                    break;
            }

            currentSpeed = (currentSpeed > -0.1f && currentSpeed < 0.1f) ? 0 : currentSpeed;

            sphere.AddForce(transform.forward * currentSpeed, ForceMode.Acceleration);
            Debug.DrawRay(transform.position, transform.forward * currentSpeed, Color.blue);
        }

        private void UpdateSpeedLinear() {
            currentSpeed = acceleration_input * acceleration_stat;
            sphere.AddForce(transform.forward * currentSpeed, ForceMode.Acceleration);
            sphere.velocity = Vector3.ClampMagnitude(sphere.velocity, max_speed_forward);
        }

        private void UpdateSpeedMario() {
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

            if (boosterTimer > 0) {
                currentSpeed = boost_power;
                sphere.AddForce(transform.forward * currentSpeed, ForceMode.Acceleration);
            } else if (currentSpeed > 0) {
                if (!isBraking) {
                    sphere.AddForce(transform.forward * currentSpeed * acceleration_input, ForceMode.Acceleration);
                } else {
                    sphere.AddForce(-sphere.velocity.normalized * currentSpeed, ForceMode.Acceleration);
                }
            } else if (isBraking) {
                sphere.velocity = Vector3.Lerp(sphere.velocity, Vector3.zero, delay_normal);
            }
        }

        public void Steer(float input)
        {
            rotate = steering * input;
            // isSteering = input != 0;
        }

        public void Brake(int input) {
            throw new NotImplementedException();
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

        public void Brake(bool input)
        {
            isBraking = input;
        }

        // private void UpdateBrake() {
        //     if (isBraking) {
        //         float current_speed_opposit_sign = -1 * Mathf.Sign(currentSpeed);

        //         currentSpeed += current_speed_opposit_sign * brake_force_stat;
        //         currentSpeed = Mathf.Sign(currentSpeed) == current_speed_opposit_sign ? 0 : currentSpeed; // le freinage ne peut pas faire avancer
        //         currentSpeed = (currentSpeed > -0.1f && currentSpeed < 0.1f) ? 0 : currentSpeed;

        //         sphere.AddForce(transform.forward * currentSpeed, ForceMode.Acceleration);
        //     }
        // }

        private void UpdateBoosterTimer() {
            if (boosterTimer > 0) {
                boosterTimer --;
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

        public void ActivateBlitz() {
            boosterTimer = boost_duration * 10;
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

            if (boosterTimer > 0) {
                acceleration_emission.rateOverTime = 100;
            } else if (currentSpeed > max_speed_forward - 10) {
                acceleration_emission.rateOverTime = 30;
            } else {
                acceleration_emission.rateOverTime = 0;
            }
        }
    }
}