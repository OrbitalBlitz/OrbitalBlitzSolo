using System.Collections.Generic;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace OrbitalBlitz.Game.Features.IA.Scripts.Sensors {
    public class InertiaAngleSensorComponent : SensorComponent {
        [SerializeField] private string name = "inertia_angle_sensor";
        [SerializeField] private Transform ship_model;
        [SerializeField] private Rigidbody ship_rigidbody;
        [SerializeField] private bool normalize;

        public override ISensor[] CreateSensors() {
            return new[] {
                new InertiaAngleSensor() {
                    name = name,
                    ship_model = ship_model,
                    ship_rigidbody = ship_rigidbody,
                    normalize = normalize
                }
            };
        }
    }
}