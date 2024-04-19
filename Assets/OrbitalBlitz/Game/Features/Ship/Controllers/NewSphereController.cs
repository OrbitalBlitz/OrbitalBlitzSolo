using System;
using UnityEngine;

#if UNITY_EDITOR
#endif

namespace OrbitalBlitz.Game.Features.Ship.Controllers {
    public class NewSphereController : AbstractShipController {
        // [FormerlySerializedAs("sphere")] [Header("Objects")] [SerializeField] public Rigidbody RB;
        [SerializeField] public ParticleSystem acceleration_particles;
        [SerializeField] private LayerMask layerMask;

        [Header("Stats")] 
        [SerializeField] private float max_speed_backward = 10f;
        [SerializeField] private float acceleration_stat = 20f;
        [SerializeField] private float deceleration_stat = 100f;
        [SerializeField] private float brake_force_stat = 100f;
        [SerializeField] private float boost_power = 60f;
        [SerializeField] private float boost_duration = 5f;
        [SerializeField] private float steering = 5f;
        [SerializeField] private float brake_steer_factor = 1.2f;


        [Header("Data visualisation")] [SerializeField]
        private float currentSpeed = 0f; // current speed of the player

        [SerializeField] private float targetSpeed = 0f;
        [SerializeField] private Vector3 sphere_velocity;
        [SerializeField] public float blitzTimer = 0;

        [Header("Lerp & delay")] [SerializeField]
        private float steeringLerpFactor = 2f;

        [SerializeField] private float stopSteeringLerpFactor = 5f;
        [SerializeField] private float delay_normal = 0.1f;
        
        [Header("Drift")] [SerializeField] private float drift_force = 1f;
        [SerializeField] private float min_angle_to_toggle_drift = 10f;
        [SerializeField] private float max_angle_to_toggle_drift = 80f;
        [SerializeField] private float min_angle_to_keep_drifting = 10f;
        [SerializeField] private float max_angle_to_keep_drifting = 45f;
        
        [Header("Particles")]
        [SerializeField] private ExhaustParticlesController exhaust_particles;
        [SerializeField] private int full_acceleration_emission_over_time;
        [SerializeField] private int zero_acceleration_emission_over_time;

        [Header("Debug")] [SerializeField] private GameObject skin;
        public ParticleSystem active_particle_system; 

        float acceleration_input = 0f;
        bool braking_input = false;
        float velocity;
        float rotate, currentRotate;
        private PlayerInputActions playerInputActions;

        bool isForward;
        bool timerOn = false;
        int timer = 0;

        public override event Action<Collider> onTriggerEnter;

        private void Awake() {
            if (exhaust_particles != null) {
                exhaust_particles._drifting_particles.Stop();
                exhaust_particles._base_particles.Play();
                exhaust_particles._can_drift_particles.Stop();
    
                active_particle_system = exhaust_particles._base_particles;
            }
        }

        private void switchActiveParticleSystem(ParticleSystem system) {
            active_particle_system.Stop();
            active_particle_system = system;
            active_particle_system.Play();
        }

        void Update() {
            //Follow Collider
            transform.position = RB.transform.position;
        }

        void FixedUpdate() {
            playAppropriateDriftParticles();
            updateActiveDriftParticles();
            //Acceleration
            UpdateSpeed();

            //Steering
            UpdateRotate();

            // if (!is_drifting) {
            //     // Project velocity onto forward and vertical directions
            //     Vector3 forward = transform.forward;
            //     Vector3 vertical = Vector3.up;
            //     Vector3 projectedVelocity = Vector3.Project(sphere.velocity, forward) +
            //                                 Vector3.Project(sphere.velocity, vertical);
            //
            //     // Apply the projected velocity
            //     sphere.velocity = projectedVelocity;
            // }

            UpdateTilt();

            UpdateBlitzTimer();

            UpdateAccelerationParticles();

            sphere_velocity = RB.velocity;
        }

        public void OnDrawGizmos() {
            
            // // Drift Toggle Zone
            // Quaternion left_min_base_rotation =
            //     Quaternion.AngleAxis(min_angle_to_toggle_drift, Vector3.up); // Rotate around the up axis
            // Debug.DrawLine(transform.position, transform.position + left_min_base_rotation * transform.forward,
            //     Color.blue);
            // Quaternion left_max_base_rotation =
            //     Quaternion.AngleAxis(max_angle_to_toggle_drift, Vector3.up); // Rotate around the up axis
            // Debug.DrawLine(transform.position, transform.position + left_max_base_rotation * transform.forward,
            //     Color.blue);
            // Quaternion right_min_base_rotation =
            //     Quaternion.AngleAxis(-min_angle_to_toggle_drift, Vector3.up); // Rotate around the up axis
            // Debug.DrawLine(transform.position, transform.position + right_min_base_rotation * transform.forward,
            //     Color.blue);
            // Quaternion right_max_base_rotation =
            //     Quaternion.AngleAxis(-max_angle_to_toggle_drift, Vector3.up); // Rotate around the up axis
            // Debug.DrawLine(transform.position, transform.position + right_max_base_rotation * transform.forward,
            //     Color.blue);
            //
            // // Keep Drifting Zone
            // Quaternion left_min_drift_rotation =
            //     Quaternion.AngleAxis(min_angle_to_keep_drifting, Vector3.up); // Rotate around the up axis
            // Debug.DrawLine(transform.position, transform.position + left_min_drift_rotation * transform.forward,
            //     Color.magenta);
            // Quaternion left_max_drift_rotation =
            //     Quaternion.AngleAxis(max_angle_to_keep_drifting, Vector3.up); // Rotate around the up axis
            // Debug.DrawLine(transform.position, transform.position + left_max_drift_rotation * transform.forward,
            //     Color.magenta);
            // Quaternion right_min_drift_rotation =
            //     Quaternion.AngleAxis(-min_angle_to_keep_drifting, Vector3.up); // Rotate around the up axis
            // Debug.DrawLine(transform.position, transform.position + right_min_drift_rotation * transform.forward,
            //     Color.magenta);
            // Quaternion right_max_drift_rotation =
            //     Quaternion.AngleAxis(-max_angle_to_keep_drifting, Vector3.up); // Rotate around the up axis
            // Debug.DrawLine(transform.position, transform.position + right_max_drift_rotation * transform.forward,
            //     Color.magenta);
            
            // Sphere velocity
            Debug.DrawLine(transform.position, transform.position + RB.velocity, Color.black);

            float base_angle = Vector3.Angle(RB.velocity, transform.forward);

            var drift_angle = getDriftAngleFromBaseAngle(base_angle);
            
            // Calculate the new velocity direction by rotating the current velocity vector
            Quaternion rotation = Quaternion.AngleAxis(drift_angle, Vector3.up); // Rotate around the up axis
            var cant_start_drift = !is_drifting && !inDriftToggleZone(drift_angle);
            var cant_continue_drift = is_drifting && !inDriftZone(drift_angle);

            var color = canDrift() ? Color.blue : Color.red;
            color = is_drifting ? Color.yellow : color;
            Debug.DrawLine(
                RB.position,
                RB.position + rotation * RB.velocity,
                color
            );
        }

        public override void Accelerate(float input) {
            acceleration_input = input;
        }

        private void UpdateSpeed() {
            if (blitzTimer > 0) RB.AddForce(transform.forward * acceleration_input * boost_power, ForceMode.Acceleration);
            else if (acceleration_input > 0 ) RB.AddForce(transform.forward * acceleration_input * acceleration_stat, ForceMode.Acceleration);
            
            applyBraking();
            
            var velocity_magnitude = Mathf.Clamp(RB.velocity.magnitude, 0, max_speed_forward);
            RB.velocity = RB.velocity.normalized * velocity_magnitude;
        }

        

        private void applyBraking() {
            if (!braking_input) {
                is_drifting = false;
                is_braking = false;
                return;
            }
            
            float base_angle = Vector3.Angle(RB.velocity, transform.forward);
            float normalized_base_angle = Math.Abs(base_angle);

            if (!canDrift()) {
                is_drifting = false;
                is_braking = true;
                RB.AddForce(-RB.velocity.normalized * brake_force_stat, ForceMode.Acceleration);
                return;
            }
            is_braking = false;
            is_drifting = true;
            drift(base_angle);
            return;
        }

        private bool canDrift() {
            if (is_braking) return false;
            
            float angle  = Math.Abs(Vector3.Angle(RB.velocity, transform.forward));
            if (!is_drifting && !inDriftToggleZone(angle)) {
                return false;
            }

            if (is_drifting && !inDriftZone(angle)) {
                return false;
            }

            return true;
        }
        private bool inDriftToggleZone(float angle) {
            return min_angle_to_toggle_drift <= angle && angle <= max_angle_to_toggle_drift;
        }
        private bool inDriftZone(float angle) {
            return min_angle_to_keep_drifting <= angle && angle <= max_angle_to_keep_drifting;
        }

        private void drift(float base_angle) {
            // Debug.Log($"Drifting to {base_angle} degree");

            var drift_angle = getDriftAngleFromBaseAngle(base_angle);
            // Calculate the new velocity direction by rotating the current velocity vector
            Quaternion rotation = Quaternion.AngleAxis(drift_angle, Vector3.up); // Rotate around the up axis
            Vector3 new_velocity_direction = rotation * RB.velocity.normalized;

            // Apply the new velocity, maintaining the original speed
            RB.velocity = new_velocity_direction * RB.velocity.magnitude; //TODO understand why currentSpeed == 0 
            Debug.DrawLine(RB.position, RB.position + (transform.forward * 2f), Color.blue);
            Debug.DrawLine(RB.position, RB.position + (new_velocity_direction * RB.velocity.magnitude), Color.red);
            // Debug.DrawLine(sphere.position, sphere.position + (drift_direction * currentSpeed), Color.red);
        }

        private float getDriftAngleFromBaseAngle(float base_angle) {
            // Calculate the drift direction - this might depend on whether v is to the left or right of the current velocity
            Vector3 cross_product = Vector3.Cross(RB.velocity, transform.forward);
            bool is_left = cross_product.y < 0;

            // Adjust the drift angle based on the direction (left or right)
            float drift_angle = is_left ? -base_angle : base_angle;
            drift_angle = drift_angle * drift_force;
            // Ensure driftAngle is within a sensible range to avoid overly sharp or reversed drifts
            drift_angle =
                Mathf.Clamp(drift_angle, -max_angle_to_keep_drifting, max_angle_to_keep_drifting); // Adjust min/max angles as needed
            
            return drift_angle;
        }
        public override void Steer(float input) {
            rotate = steering * input * (braking_input ? brake_steer_factor : 1f);
        }

        private void UpdateRotate() {
            float adjustedLerpingFactor = (rotate != 0f) ? steeringLerpFactor : stopSteeringLerpFactor;
            currentRotate = Mathf.Lerp(currentRotate, rotate, adjustedLerpingFactor * Time.fixedDeltaTime);

            transform.rotation = transform.rotation * Quaternion.Euler(0, currentRotate, 0);
        }

        private void UpdateTilt() {
            RaycastHit hit;
            Physics.Raycast(transform.position, Vector3.down, out hit, 2.0f, layerMask);

            Quaternion groundNormalRotation = Quaternion.FromToRotation(transform.up, hit.normal);

            transform.rotation =
                Quaternion.Lerp(transform.rotation, groundNormalRotation * transform.rotation, delay_normal);
        }

        public override void Brake(int input) {
            braking_input = input > 0;
        }

        public override void ActivateBlitz() {
            blitzTimer = boost_duration * 10;
        }

        private void UpdateBlitzTimer() {
            if (blitzTimer > 0) {
                blitzTimer--;
            }
        }

        public override void Reset() {
            currentSpeed = 0;
            currentRotate = 0;
        }

        public override void SetIsKinematic(bool toggle) {
            RB.isKinematic = toggle;
            ;
        }

        public override float GetSpeed() {
            return RB.velocity.magnitude;
        }

        public void UpdateAccelerationParticles() {
            ParticleSystem.EmissionModule acceleration_emission = acceleration_particles.emission;
            float max_speed_division = max_speed_forward / 10;

            if (blitzTimer > 0) {
                acceleration_emission.rateOverTime = 100;
            }
            else if (RB.velocity.magnitude >= max_speed_division*4) {
                acceleration_emission.rateOverTime = 20;
            }
            else if (RB.velocity.magnitude >= max_speed_division*2) {
                acceleration_emission.rateOverTime = 5;
            }
            else {
                acceleration_emission.rateOverTime = 0;
            }
        }

        private void playAppropriateDriftParticles() {
            if (exhaust_particles == null) return;
            
            if (is_drifting) {
                if (!exhaust_particles._drifting_particles.isStopped) return;
                Debug.Log("is_drifting");
                switchActiveParticleSystem(exhaust_particles._drifting_particles);
                return;
            }

            if (canDrift()) {
                if (!exhaust_particles._can_drift_particles.isStopped) return;
                Debug.Log("can_drifting");
                switchActiveParticleSystem(exhaust_particles._can_drift_particles);
                return;  
            }

            if (!exhaust_particles._base_particles.isStopped) return;
            Debug.Log("base particles");
            switchActiveParticleSystem(exhaust_particles._base_particles);
        }

        private void updateActiveDriftParticles() {
            if (exhaust_particles == null) return;

            var emission = active_particle_system.emission;
            if (acceleration_input > 0) {
                emission.rateOverTime = full_acceleration_emission_over_time * acceleration_input;
                return;
            }
            emission.rateOverTime = zero_acceleration_emission_over_time * acceleration_input;            
        }

        private void OnTriggerEnter(Collider other) {
            Debug.Log("Controller: {gameObject.name} triggered with {other.gameObject.name}");

            onTriggerEnter?.Invoke(other);
        }

        private void OnCollisionEnter(Collision collision) {
            Debug.Log("Controller: {gameObject.name} collided with {other.gameObject.name}");
        }
    }
}