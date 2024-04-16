using System;
using System.Collections;
using System.Collections.Generic;
using OrbitalBlitz.Game.Scenes.Race.Scripts;
using UnityEngine;

public class RewardCheckpoint : MonoBehaviour {
    [SerializeField] public float Reward = 2.5f;
    [SerializeField] public float PenaltyForWrongOrder = 1f;

    private void Start() {
        gameObject.GetComponent<MeshRenderer>().enabled = false;
    }
}