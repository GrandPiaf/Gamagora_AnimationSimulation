using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{

    public GameObject particle;

    public int nbParticles;

    private List<Particle> particles;


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


    [Range(0, 1)]
    public float deltaTime;


    private Dictionary<Tuple<int, int>, List<Particle>> grid;

    [Range(0, 1000)]
    public float outsideForce;


    void Start() {

        // Seeding random
        UnityEngine.Random.InitState(247943128);

        particles = new List<Particle>(nbParticles);

        for (int i = 0; i < nbParticles; i++) {

            //Create particle i
            Vector3 particlePos = new Vector3(
                UnityEngine.Random.Range(-xRange, xRange),
                UnityEngine.Random.Range(-yRange, yRange)
            );
            particles.Add( Instantiate(particle, particlePos, Quaternion.identity).GetComponent<Particle>() );
            
            // Settings velocity & random mass from range [1f, 10f]
            float mass = UnityEngine.Random.Range(lowerMass, upperMass);
            particles[i].mass = mass;
            particles[i].velocity = (Vector3.up) * 10f;
            //particles[i].GetComponent<Particle>().velocity = (Vector3.up + Vector3.right) * 10f;

            // Setting color depending on mass
            // RED = high mass
            // BLUE = low mass
            float rangedMass = (((mass - lowerMass)) / (upperMass - lowerMass));
            //Color current = rangedMass * high + (1 - rangedMass) * low;
            Color current = Color.Lerp(low, high, rangedMass); //Using lerp is as efficient

            particles[i].GetComponent<SpriteRenderer>().material.color = current;
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
        grid = new Dictionary<Tuple<int, int>, List<Particle>>();

        // For each particle, sort it in the correct cell
        foreach (Particle particle in particles) {

            Tuple<int, int> particleCell = getCellFromParticle(particle);

            if (!grid.ContainsKey(particleCell)) {
                grid.Add(particleCell, new List<Particle>());
            }

            grid[particleCell].Add(particle);

        }

    }

    private Tuple<int, int> getCellFromParticle(Particle particle) {

        return Tuple.Create(
            Convert.ToInt32( Mathf.Floor( (particle.positionCache.x + xRange) / h) ),
            Convert.ToInt32( Mathf.Floor( (particle.positionCache.y + yRange) / h) )
        );
    }

    private void doubleDensityRelaxation() {

        foreach (Particle particle in particles) {

            List<Particle> neighbourhood = getNeighboursPerf(particle);


            // Compute density and near density
            float d = 0;
            float dNear = 0;

            foreach (Particle neighbour in neighbourhood) {

                float q = Vector3.Distance(particle.positionCache, neighbour.positionCache) / h;

                if (q < 1) {
                    d += Mathf.Pow(1 - q, 2);
                    dNear += Mathf.Pow(1 - q, 3);
                }

            }

            // Compute pressure and near-pressure
            float p = k * (d - d0);
            float pNear = kNear * dNear;

            Vector3 dx = Vector3.zero;

            foreach (Particle neighbour in neighbourhood) {

                float q = Vector3.Distance(particle.positionCache, neighbour.positionCache) / h;

                if (q < 1) {
                    // Apply displacements
                    Vector3 D = Mathf.Pow(deltaTime, 2) / particle.mass * ( p * (1 - q) + pNear * Mathf.Pow(1 - q, 2) ) * (neighbour.positionCache - particle.positionCache);
                    neighbour.positionCache += (D / 2);
                    dx -= (D / 2);
                }

            }

            particle.positionCache += dx;

        }

    }

    private List<Particle> getNeighboursPerf(Particle particle) {
        List<Particle> neighbours = new List<Particle>();

        // Get EVERY CELL AROUND THE CURRENT particle
        Tuple<int, int> particleCell = getCellFromParticle(particle);

        for (int i = particleCell.Item1 - 1; i < particleCell.Item1 + 2; i++) {
            for (int j = particleCell.Item2 - 1; j < particleCell.Item2 + 2; j++) {

                Tuple<int, int> tempCell = Tuple.Create(i, j);

                List<Particle> tempCellParticles;

                if (grid.TryGetValue(tempCell, out tempCellParticles)) {

                    foreach (Particle other in tempCellParticles) {
                        if (other == particle) {
                            continue;
                        }

                        if (Vector3.Distance(particle.positionCache, other.positionCache) <= h) {
                            neighbours.Add(other);
                        }
                    }

                }
                

            }
        }


        return neighbours;
    }

    // Return list of neighbours of 'particle' (except itself)
    private List<Particle> getNeighbours(Particle particle) {

        List<Particle> neighbours = new List<Particle>();

        foreach (Particle other in particles) {

            // Avoiding current particle
            if (other == particle) {
                continue;
            }

            if (Vector3.Distance(particle.positionCache, other.positionCache) <= h) {
                neighbours.Add(other);
            }

        }

        return neighbours;
    }

    private void computeNextVelocity() {

        foreach (Particle particle in particles) {

            // Add force proportionnal to distance between current position & border

            if (particle.positionCache.y <= -yRange) {
                float distY = - particle.positionCache.y;
                float force = distY * outsideForce * deltaTime * deltaTime / particle.mass;
                particle.positionCache.y += force;
            }

            if (particle.positionCache.x <= -xRange) {
                float distX = -particle.positionCache.x;
                float force = distX * outsideForce * deltaTime * deltaTime / particle.mass;
                particle.positionCache.x += force;
            }

            if (particle.positionCache.x >= xRange) {
                float distX = -particle.positionCache.x;
                float force = distX * outsideForce * deltaTime * deltaTime / particle.mass;
                particle.positionCache.x += force;
            }

            particle.velocity = (particle.positionCache - particle.previousPositon) / deltaTime;

            particle.transform.position = particle.positionCache;
        }

    }

    private void applyGravity() {

        foreach (Particle particle in particles) {

            particle.positionCache = particle.transform.position;

            /* lines 1 - 3 */
            //Compute acceleration
            Vector3 acceleration = (-particle.mass * g * gravity - particle.velocity * particle.velocity.magnitude) / particle.mass;

            //Compute next velocity
            particle.velocity = particle.velocity + deltaTime * acceleration;


            /* lines 6 - 10 */
            particle.previousPositon = particle.positionCache;

            //Compute next position
            particle.positionCache = particle.transform.position + deltaTime * particle.velocity;

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
