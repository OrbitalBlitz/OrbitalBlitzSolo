using System.Collections.Generic;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace OrbitalBlitz.Game.Features.IA.Scripts.Sensors {
    public class InertiaAngleSensor : ISensor {

        public string name = "inertia_angle_sensor";
        public Transform ship_model;
        public Rigidbody ship_rigidbody;
        public bool normalize;
            
        private float angle = .5f;
    
        public ObservationSpec GetObservationSpec() {
            return ObservationSpec.Vector(1);
        }

        public int Write(ObservationWriter writer) {
            angle = AlgebraUtils.SignedAngleBetween(
                ship_model.forward,
                ship_rigidbody.velocity,
                ship_model.up,
                normalize
            );
            writer.AddList(new List<float>(){ angle });
            return 1;
        }

        public byte[] GetCompressedObservation() {
            return null;
        }

        void ISensor.Update() {
        }


        public void Reset() {
            angle = 0.5f;
        }

        public CompressionSpec GetCompressionSpec() {
            return CompressionSpec.Default();
        }

        public string GetName() {
            return name;
        }
    }
}