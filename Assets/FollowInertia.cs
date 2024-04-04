using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowInertia : MonoBehaviour {
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float sensitivity = 0.01f;
    private void Update() {
        if (rb.velocity != Vector3.zero) {
            transform.forward = rb.velocity.normalized;
        }
    }
}