using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCollider : MonoBehaviour {
    public event Action<Collider> onTrigger;
    private void OnTriggerEnter(Collider other) {
        Debug.Log($"{gameObject.name} trigger enter with {other.gameObject.name}");
        onTrigger?.Invoke(other);
    }
}