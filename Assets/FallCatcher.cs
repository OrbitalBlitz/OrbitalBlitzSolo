using System.Collections;
using System.Collections.Generic;
using OrbitalBlitz.Game.Features.Ship.Controllers;
using UnityEngine;

public class FallCatcher : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        Debug.Log("Touched plane");
        IShipController shipController = other.transform.parent.GetComponentInChildren<IShipController>();
        if (shipController != null) {
            Debug.Log("found controller");
            shipController.RespawnToLastCheckpoint();
        }
    }
}