using System.Collections;
using System.Collections.Generic;
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

    public float h;
    public float k;
    public float d0;
    public float kNear;

    public float deltaTime;

    void Start() {

        // Seeding random
        Random.InitState(247943128);

        particles = new List<Particle>(nbParticles);

        for (int i = 0; i < nbParticles; i++) {

            //Create particle i
            Vector3 particlePos = new Vector3(
                Random.Range(-xRange, xRange),
                Random.Range(-yRange, yRange)
            );
            particles.Add( Instantiate(particle, particlePos, Quaternion.identity).GetComponent<Particle>() );
            
            // Settings velocity & random mass from range [1f, 10f]
            float mass = Random.Range(lowerMass, upperMass);
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

        /* line 16 */
        doubleDensityRelaxation();

        /* lines 18 - 20 */
        computeNextVelocity();
    }

    private void doubleDensityRelaxation() {

        foreach (Particle particle in particles) {

            List<Particle> neighbourhood = getNeighbours(particle);


            // Compute density and near density
            float d = 0;
            float dNear = 0;

            foreach (Particle neighbour in neighbourhood) {

                float q = Vector3.Distance(particle.transform.position, neighbour.transform.position) / h;

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

                float q = Vector3.Distance(particle.transform.position, neighbour.transform.position) / h;

                if (q < 1) {
                    // Apply displacements
                    Vector3 D = Mathf.Pow(deltaTime, 2) * ( p * (1 - q) + pNear * Mathf.Pow(1 - q, 2) ) * (neighbour.transform.position - particle.transform.position);
                    neighbour.transform.position += (D / 2);
                    dx -= (D / 2);
                }

            }

            particle.transform.position += dx;

        }

    }

    // Return list of neighbours of 'particle' (except itself)
    private List<Particle> getNeighbours(Particle particle) {

        List<Particle> neighbours = new List<Particle>();

        foreach (Particle other in particles) {

            // Avoiding current particle
            if (other == particle) {
                continue;
            }

            if (Vector3.Distance(particle.transform.position, other.transform.position) <= h) {
                neighbours.Add(other);
            }

        }

        return neighbours;
    }

    private void computeNextVelocity() {

        foreach (Particle particle in particles) {

            Vector3 pos = particle.transform.position;

            if (pos.y <= -yRange) {
                pos.y = -yRange;
            }

            if (pos.x <= -xRange) {
                pos.x = -xRange;
            }

            if (pos.x >= xRange) {
                pos.x = xRange;
            }

            particle.transform.position = pos;

            particle.velocity = (particle.transform.position - particle.previousPositon) / deltaTime;
        }

    }

    private void applyGravity() {

        foreach (Particle particle in particles) {

            /* lines 1 - 3 */
            //Compute acceleration
            Vector3 acceleration = (-particle.mass * g * gravity - particle.velocity * particle.velocity.magnitude) / particle.mass;

            //Compute next velocity
            particle.velocity = particle.velocity + deltaTime * acceleration;


            /* lines 6 - 10 */
            particle.previousPositon = particle.transform.position;

            //Compute next position
            particle.transform.position = particle.transform.position + deltaTime * particle.velocity;

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
