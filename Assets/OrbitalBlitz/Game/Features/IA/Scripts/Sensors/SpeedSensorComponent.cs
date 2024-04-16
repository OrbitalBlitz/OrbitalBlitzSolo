using OrbitalBlitz.Game.Features.Ship.Controllers;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace OrbitalBlitz.Game.Features.IA.Scripts.Sensors {
    public class SpeedSensorComponent : SensorComponent {

        [SerializeField] private string name = "speed_sensor";
        [SerializeField] private AbstractShipController ship_controller;
        [SerializeField] private bool normalize;
    
        public override ISensor[] CreateSensors() {
            return new[] { new SpeedSensor() {
                name = name,
                ship_controller = ship_controller,
                normalize = normalize
            } };
        }
    }
}