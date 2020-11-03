using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particles
{
    internal float[] masses;
    internal Vector3[] velocities;
    internal Vector3[] previousPositions;
    internal Vector3[] positionCaches;

    internal GameObject[] particles;

    private int nbParticles;

    public Particles(int nbParticles) {
        this.nbParticles = nbParticles;
        this.masses = new float[nbParticles];
        this.velocities = new Vector3[nbParticles];
        this.previousPositions = new Vector3[nbParticles];
        this.positionCaches = new Vector3[nbParticles];
        this.particles = new GameObject[nbParticles];
    }

    internal void Add(int i, float mass, Vector3 velocity, Vector3 previousPosition, Vector3 particlePos, GameObject go) {
        masses[i] = mass;
        velocities[i] = velocity;
        previousPositions[i] = previousPosition;
        positionCaches[i] = particlePos;
        particles[i] = go;
    }
}
