using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Represents a joint of a skeleton
 * Can have constraints (angles)
 */
public class Joint : MonoBehaviour
{

    void OnMouseDrag() {
        Vector3 move = transform.position;
        move.x = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        move.y = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;
        transform.position = move;
    }

}
