using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OrbitalBlitz.Game.Features.Ship;
using OrbitalBlitz.Game.Features.Ship.Controllers;
using Tests.BaseShipControllerTests;

public class BaseShipControllerTests : MonoBehaviour
{
    public void ApplyAccelerationToWheel_Test()
    {
        // Créer un GameObject pour représenter le vaisseau
        GameObject shipObject = new GameObject("TestShip");
        Controllers shipController = shipObject.AddComponent<Controllers>();

        // Créer une instance de Rigidbody pour le vaisseau
        Rigidbody rb = shipObject.AddComponent<Rigidbody>();
        shipController._rb = rb;

        // Créer une roue de test
        GameObject wheelObject = new GameObject("TestWheel");
        Transform testWheel = wheelObject.transform;

        // Positionner la roue de test
        testWheel.position = Vector3.zero;

        // Appeler la méthode ApplyAccelerationToWheel avec une valeur d'accélération
        float acceleration = 1.0f;
        shipController.ApplyAccelerationToWheel(testWheel, acceleration);

        // Vérifier que la force d'accélération a été correctement appliquée à la roue
        float expectedForce = acceleration * shipController.AccelerationForce;
        Vector3 expectedForceVector = testWheel.forward * expectedForce;

        Assert.AreEqual(expectedForceVector, rb.GetPointVelocity(testWheel.position));
    }
}
