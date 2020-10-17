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
            particles.Add( Instantiate(particle, new Vector3((UnityEngine.Random.value - 0.5f) * 16, (UnityEngine.Random.value - 0.5f) * 8), Quaternion.identity) );
        }
    }

}
