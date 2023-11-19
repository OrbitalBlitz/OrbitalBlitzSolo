using System;
using System.Collections;
using System.Collections.Generic;
using OrbitalBlitz.Game.Features.Ship.Controllers;
using UnityEngine;

public class Player : MonoBehaviour {
    public static Player Singleton;
    public PlayerInputActions Input;
    public IShipController ShipController;

    private void Awake() {
        Singleton = this;
        Input = new PlayerInputActions();
        Input.defaultMap.Enable();
    }
}