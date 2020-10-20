using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{

    public GameObject particle;

    public int nbParticles;

    private List<GameObject> particles;


    private static float lowerMass = 1f;
    private static float upperMass = 10f;

    // setting color FROM mass
    private static Color low = Color.blue;
    private static Color high = Color.red;



    void Start() {

        particles = new List<GameObject>(nbParticles);

        for (int i = 0; i < nbParticles; i++) {

            //Create particle i
            particles.Add( Instantiate(particle, new Vector3((Random.value - 0.5f) * 16, (Random.value - 0.5f) * 8), Quaternion.identity) );
            
            // Settings velocity & random mass from range [1f, 10f]
            float mass = Random.Range(lowerMass, upperMass);
            particles[i].GetComponent<Particle>().mass = mass;
            particles[i].GetComponent<Particle>().velocity = (Vector3.up + Vector3.right) * 10f;

            // Setting color depending on mass
            // RED = high mass
            // BLUE = low mass
            float rangedMass = (((mass - lowerMass)) / (upperMass - lowerMass));
            Color current = rangedMass * high + (1 - rangedMass) * low;

            particles[i].GetComponent<SpriteRenderer>().material.color = current;
        }
    }

}
