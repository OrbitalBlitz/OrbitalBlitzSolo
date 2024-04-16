using System.Collections;
using System.Collections.Generic;
using OrbitalBlitz.Game.Features.Ship.Controllers;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class SpeedSensor : ISensor {

    public string name = "speed_sensor";
    public AbstractShipController ship_controller;
    public bool normalize;
    
    private float speed = 0f;

    public ObservationSpec GetObservationSpec() {
        return ObservationSpec.Vector(1);
    }

    public int Write(ObservationWriter writer) {
        speed = ship_controller.RB.velocity.magnitude / ship_controller.max_speed_forward;
        writer.AddList(new List<float>(){ speed });
        return 1;
    }

    public byte[] GetCompressedObservation() {
        return null;
    }

    void ISensor.Update() {
    }


    public void Reset() {
        speed = 0f;
    }

    public CompressionSpec GetCompressionSpec() {
        return CompressionSpec.Default();
    }

    public string GetName() {
        return name;
    }
}