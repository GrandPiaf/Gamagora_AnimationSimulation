using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{

    public GameObject particlePrefab;

    public int nbParticles;

    Particles particles;


    private static float lowerMass = 1f;
    private static float upperMass = 10f;

    // setting color FROM mass
    private static Color low = Color.blue;
    private static Color high = Color.red;


    // Gravity constant
    private static readonly float g = 9.8f;
    private static readonly Vector3 gravity = new Vector3(0, 1.0f, 0);

    public float xRange;
    public float yRange;

    // Distance de recherche du voisinnage
    // 1 = 1 cube
    [Range(0, 1000)]
    public float h;

    // Quantifie la force de répulsion du voisinnage
    [Range(0, 1000)]
    public float k;
    
    // La densité de voisinnage d'une particule
    // Si = 1, elle va attirer les voisins jusqu'à que la densité moyenne autour soit suffisante
    [Range(0, 1000)]
    public float d0;
    
    // "Unpack" les particle qui sont paqués
    [Range(0, 1000)]
    public float kNear;


    public float deltaTime;


    private Dictionary<Tuple<int, int>, List<int>> grid;

    [Range(0, 1000)]
    public float outsideForce;


    void Start() {

        // Seeding random
        UnityEngine.Random.InitState(247943128);

        particles = new Particles(nbParticles);

        for (int i = 0; i < nbParticles; i++) {

            //Create particle i
            Vector3 particlePos = new Vector3(
                UnityEngine.Random.Range(-xRange, xRange),
                UnityEngine.Random.Range(-yRange, yRange)
            );

            GameObject go = Instantiate(particlePrefab, particlePos, Quaternion.identity);
            
            // Setting random mass from range [1f, 10f]
            float mass = UnityEngine.Random.Range(lowerMass, upperMass);

            // Setting velocity
            Vector3 velocity = (Vector3.up) * 10f;
            //particles[i].GetComponent<Particle>().velocity = (Vector3.up + Vector3.right) * 10f;

            // Setting color depending on mass / RED = high mass / BLUE = low mass
            float rangedMass = (((mass - lowerMass)) / (upperMass - lowerMass));
            Color current = Color.Lerp(low, high, rangedMass);

            go.GetComponent<SpriteRenderer>().material.color = current;

            particles.Add(i, mass, velocity, Vector3.zero, particlePos, go);
        }
    }

    void Update() {

        // Apply gravity
        /* lines 1 - 3  &  lines 6 - 10 */
        applyGravity();

        sortParticles();

        /* line 16 */
        doubleDensityRelaxation();

        /* lines 18 - 20 */
        computeNextVelocity();
    }


    private void sortParticles() {

        // Create Dictionary
        grid = new Dictionary<Tuple<int, int>, List<int>>();

        // For each particle, sort it in the correct cell
        for (int i = 0; i < nbParticles; i++) {

            Tuple<int, int> particleCell = getCellFromParticle(i);

            if (!grid.ContainsKey(particleCell)) {
                grid.Add(particleCell, new List<int>());
            }

            grid[particleCell].Add(i);

        }

    }

    private Tuple<int, int> getCellFromParticle(int i) {

        int x = Convert.ToInt32(Mathf.Floor((particles.positionCaches[i].x + xRange) / h));
        int y = Convert.ToInt32(Mathf.Floor((particles.positionCaches[i].y + yRange) / h));

        return Tuple.Create(x, y);
    }

    private void doubleDensityRelaxation() {

        for (int i = 0; i < nbParticles; i++) {

            List<int> neighbourhood = getNeighboursPerf(i);


            // Compute density and near density
            float d = 0;
            float dNear = 0;

            foreach (int neighbour in neighbourhood) {

                float q = Vector3.Distance(particles.positionCaches[i], particles.positionCaches[neighbour]) / h;

                if (q < 1) {
                    d += Mathf.Pow(1 - q, 2);
                    dNear += Mathf.Pow(1 - q, 3);
                }

            }

            // Compute pressure and near-pressure
            float p = k * (d - d0);
            float pNear = kNear * dNear;

            Vector3 dx = Vector3.zero;

            foreach (int neighbour in neighbourhood) {

                float q = Vector3.Distance(particles.positionCaches[i], particles.positionCaches[neighbour]) / h;

                if (q < 1) {
                    // Apply displacements
                    Vector3 D = Mathf.Pow(deltaTime, 2) / particles.masses[i] * ( p * (1 - q) + pNear * Mathf.Pow(1 - q, 2) ) * (particles.positionCaches[neighbour] - particles.positionCaches[i]);
                    particles.positionCaches[neighbour] += (D / 2);
                    dx -= (D / 2);
                }

            }

            particles.positionCaches[i] += dx;

        }

    }

    private List<int> getNeighboursPerf(int i) {
        List<int> neighbours = new List<int>();

        // Get EVERY CELL AROUND THE CURRENT particle
        Tuple<int, int> particleCell = getCellFromParticle(i);

        for (int k = particleCell.Item1 - 1; k < particleCell.Item1 + 2; k++) {
            for (int j = particleCell.Item2 - 1; j < particleCell.Item2 + 2; j++) {

                Tuple<int, int> tempCell = Tuple.Create(k, j);

                List<int> tempCellParticles;

                if (grid.TryGetValue(tempCell, out tempCellParticles)) {

                    foreach (int other in tempCellParticles) {
                        if (other == i) {
                            continue;
                        }

                        if (Vector3.Distance(particles.positionCaches[i], particles.positionCaches[other]) <= h) {
                            neighbours.Add(other);
                        }
                    }

                }
                

            }
        }


        return neighbours;
    }

    private void computeNextVelocity() {

        for (int i = 0; i < nbParticles; i++) {

            // Add force proportionnal to distance between current position & border

            if (particles.positionCaches[i].y <= -yRange) {
                float distY = -particles.positionCaches[i].y;
                float force = distY * outsideForce * deltaTime * deltaTime / particles.masses[i];
                particles.positionCaches[i].y += force;
            }

            if (particles.positionCaches[i].x <= -xRange) {
                float distX = -particles.positionCaches[i].x;
                float force = distX * outsideForce * deltaTime * deltaTime / particles.masses[i];
                particles.positionCaches[i].x += force;
            }

            if (particles.positionCaches[i].x >= xRange) {
                float distX = -particles.positionCaches[i].x;
                float force = distX * outsideForce * deltaTime * deltaTime / particles.masses[i];
                particles.positionCaches[i].x += force;
            }

            particles.velocities[i] = (particles.positionCaches[i] - particles.previousPositions[i]) / deltaTime;

            particles.particles[i].transform.position = particles.positionCaches[i];
        }

    }

    private void applyGravity() {

        for (int i = 0; i < nbParticles; i++) {

            /* lines 1 - 3 */
            //Compute acceleration
            Vector3 acceleration = (-particles.masses[i] * g * gravity - particles.velocities[i] * particles.velocities[i].magnitude) / particles.masses[i];

            //Compute next velocity
            particles.velocities[i] = particles.velocities[i] + deltaTime * acceleration;


            /* lines 6 - 10 */
            particles.previousPositions[i] = particles.positionCaches[i];

            //Compute next position
            particles.positionCaches[i] += + deltaTime * particles.velocities[i];

        }

    }

    void OnDrawGizmos() {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(new Vector3(xRange, yRange, 0), new Vector3(xRange, -yRange, 0));
        Gizmos.DrawLine(new Vector3(xRange, yRange, 0), new Vector3(-xRange, yRange, 0));
        Gizmos.DrawLine(new Vector3(-xRange, -yRange, 0), new Vector3(xRange, -yRange, 0));
        Gizmos.DrawLine(new Vector3(-xRange, -yRange, 0), new Vector3(-xRange, yRange, 0));
    }


}
