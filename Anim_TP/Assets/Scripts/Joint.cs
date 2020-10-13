using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Represents a joint linking 2 segments of a skeleton
 * Can have constraints (angles)
 */
public class Joint : MonoBehaviour
{
    public Skeleton skeleton;

    // The 2 segments linked
    //public Segment left;
    //public Segment right;

    // Can add constraints


    void OnMouseDrag() {
        Vector3 move = transform.position;
        move.x = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        move.y = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;
        transform.position = move;
    }

}
