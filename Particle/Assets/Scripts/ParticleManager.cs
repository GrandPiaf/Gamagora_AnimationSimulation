using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{

    public GameObject particle;

    public int nbParticles;

    private List<GameObject> particles;
    
    void Start() {

        particles = new List<GameObject>(nbParticles);

        for (int i = 0; i < nbParticles; i++) {
            particles.Add( Instantiate(particle, new Vector3((Random.value - 0.5f) * 16, (Random.value - 0.5f) * 8), Quaternion.identity) );
            particles[i].GetComponent<SpriteRenderer>().material.color = new Color(
                Random.Range(0f, 1f),
                Random.Range(0f, 1f),
                Random.Range(0f, 1f),
                1
            );
            particles[i].GetComponent<Particle>().mass = Random.Range(1f, 10f);
            particles[i].GetComponent<Particle>().velocity = (Vector3.up + Vector3.right) * 10f;
        }
    }

}
