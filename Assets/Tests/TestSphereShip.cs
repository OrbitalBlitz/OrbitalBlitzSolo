using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using OrbitalBlitz.Game.Features.Ship.Controllers;
using UnityEngine.TestTools;

public class NewSphereControllerTests
{
    [Test]
    public void UpdateAccelerationParticles_Test()
    {
        // Créer un GameObject pour représenter le vaisseau
        GameObject shipObject = new GameObject("TestShip");
        NewSphereController shipController = shipObject.AddComponent<NewSphereController>();

        // Créer une instance de ParticleSystem pour les particules d'accélération
        GameObject particlesObject = new GameObject("TestParticles");
        ParticleSystem accelerationParticles = particlesObject.AddComponent<ParticleSystem>();
        shipController.acceleration_particles = accelerationParticles;

        // Fixer la vitesse du vaisseau à une valeur arbitraire
        shipController.max_speed_forward = 100f;
        shipController.RB = new Rigidbody();

        // Appeler la méthode UpdateAccelerationParticles
        shipController.UpdateAccelerationParticles();

        // Obtenir le module d'émission des particules
        ParticleSystem.EmissionModule emissionModule = accelerationParticles.emission;

        // Vérifier que le taux d'émission des particules est correctement défini en fonction de la vitesse du vaisseau
        if (shipController.blitzTimer > 0)
        {
            Assert.AreEqual(100, emissionModule.rateOverTime.constant);
        }
        else if (shipController.RB.velocity.magnitude >= shipController.max_speed_forward / 10 * 4)
        {
            Assert.AreEqual(20, emissionModule.rateOverTime.constant);
        }
        else if (shipController.RB.velocity.magnitude >= shipController.max_speed_forward / 10 * 2)
        {
            Assert.AreEqual(5, emissionModule.rateOverTime.constant);
        }
        else
        {
            Assert.AreEqual(0, emissionModule.rateOverTime.constant);
        }
    }
}
