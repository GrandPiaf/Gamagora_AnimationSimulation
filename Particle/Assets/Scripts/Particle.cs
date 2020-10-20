using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    // Gravity constant
    private static readonly float g = 9.8f;
    private static readonly Vector3 gravity = new Vector3(0, 1.0f, 0);

    public float mass;
    public Vector3 velocity;


    void Update() {

        //Compute acceleration
        Vector3 acceleration = (-mass * g * gravity - velocity * velocity.magnitude) / mass;

        //Compute next velocity
        Vector3 nextVelocity = velocity + Time.deltaTime * acceleration;

        //Compute next position
        Vector3 nextPosition = transform.position + Time.deltaTime * velocity;

        if (transform.position.y <= -4f) {
            nextPosition.y = -4f;
        }
        
        if (transform.position.x <= -8f) {
            nextPosition.x = -8f;
        }

        if (transform.position.x >= 8f) {
            nextPosition.x = 8f;
        }

        velocity = nextVelocity;
        transform.position = nextPosition;

    }

}
