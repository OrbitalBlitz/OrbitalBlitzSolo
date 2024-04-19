// using System.Collections;
// using System.Collections.Generic;
// using NUnit.Framework;
// using OrbitalBlitz.Game.Features.Ship.Controllers;
// using UnityEngine;
// using UnityEngine.TestTools;

// public class NewTestScript
// {
//     // A Test behaves as an ordinary method
//     [Test]
//     public void NewTestScriptSimplePasses()
//     {
//         // Créer un GameObject pour représenter le vaisseau
//         GameObject shipObject = new GameObject("TestShip");
//         NewSphereController newSphereController = shipObject.AddComponent<NewSphereController>();

//         // Créer une instance de ParticleSystem pour les particules d'accélération
//         GameObject particlesObject = new GameObject("TestParticles");
//         ParticleSystem accelerationParticles = particlesObject.AddComponent<ParticleSystem>();
//         newSphereController.acceleration_particles = accelerationParticles;

//         // Fixer la vitesse du vaisseau à une valeur arbitraire
//         newSphereController.max_speed_forward = 100f;
//         newSphereController.RB = new Rigidbody();

//         // Appeler la méthode UpdateAccelerationParticles
//         newSphereController.UpdateAccelerationParticles();

//         // Obtenir le module d'émission des particules
//         ParticleSystem.EmissionModule emissionModule = accelerationParticles.emission;

//         // Vérifier que le taux d'émission des particules est correctement défini en fonction de la vitesse du vaisseau
//         if (newSphereController.blitzTimer > 0)
//         {
//             Assert.AreEqual(100, emissionModule.rateOverTime.constant);
//         }
//         else if (newSphereController.RB.velocity.magnitude >= newSphereController.max_speed_forward / 10 * 4)
//         {
//             Assert.AreEqual(20, emissionModule.rateOverTime.constant);
//         }
//         else if (newSphereController.RB.velocity.magnitude >= newSphereController.max_speed_forward / 10 * 2)
//         {
//             Assert.AreEqual(5, emissionModule.rateOverTime.constant);
//         }
//         else
//         {
//             Assert.AreEqual(0, emissionModule.rateOverTime.constant);
//         }
//     }

//     // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
//     // `yield return null;` to skip a frame.
//     [UnityTest]
//     public IEnumerator NewTestScriptWithEnumeratorPasses()
//     {
//         // Use the Assert class to test conditions.
//         // Use yield to skip a frame.
//         yield return null;
//     }
// }
