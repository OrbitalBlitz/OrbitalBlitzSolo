using System;
using OrbitalBlitz.Game.Features.Player;
using OrbitalBlitz.Game.Features.Ship;
using OrbitalBlitz.Game.Features.Ship.Controllers;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

public class OrbitalBlitzPlayer : MonoBehaviour {
    public PlayerInputActions Input;
    public AbstractShipController AbstractShipController;
    public PlayerAgent Agent;
    public Collider ShipCollider;
    public GameObject Ship;
    private ShipStateMemento memento;
    public PlayerInfo Info;

    private void Awake() {
        Input = new PlayerInputActions();
        Input.defaultMap.Enable();
    }

    // private void Update() {
    //     if (AbstractShipController != null) {
    //         transform.position = AbstractShipController.transform.position;
    //         transform.rotation = AbstractShipController.transform.rotation;
    //     }
    // }


    public void SetShip(GameObject ship) {
        Ship = ship;
        AbstractShipController = ship.GetComponentInChildren<AbstractShipController>();
        memento = new(ship);
        Info.SetShipCollider(ship.GetComponentInChildren<ShipCollider>());
    }

    public void Respawn() {
        Debug.Log($"{gameObject.name}.OBPlayer.Respawn()");
        if (memento == null) {
            Debug.LogError("memento is null");
            return;
        }
        memento.RestoreInitialState();        
        memento.Reset();
        Info.Reset();
    }

    public void RespawnToLastCheckpoint() {
        Debug.Log("OBPlayer.RespawnToLastCheckpoint()");
        memento.Rollback(2);
    }

    public void SaveCheckpoint() {
        Debug.Log("OBPlayer.SaveCheckpoint()");
        memento.SaveState(tolerance:1f);
        // memento.SaveState();
    }
}