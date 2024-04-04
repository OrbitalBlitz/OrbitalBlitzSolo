using System;
using System.Collections;
using System.Collections.Generic;
using OrbitalBlitz.Game.Features.Ship.Controllers;
using UnityEngine;

public class ExhaustParticlesController : MonoBehaviour {
    [SerializeField] public ParticleSystem _base_particles;
    [SerializeField] public ParticleSystem _can_drift_particles;
    [SerializeField] public ParticleSystem _drifting_particles;
}
