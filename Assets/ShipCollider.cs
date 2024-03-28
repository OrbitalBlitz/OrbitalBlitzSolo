using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCollider : MonoBehaviour {
    public event Action<Collider> onTrigger;
    private void OnTriggerEnter(Collider other) {
        onTrigger?.Invoke(other);
    }
}