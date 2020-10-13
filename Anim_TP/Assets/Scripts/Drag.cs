using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drag : MonoBehaviour {

    void OnMouseDrag() {
        Vector3 move = transform.position;
        move.x = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        move.y = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;
        transform.position = move;
    }

}

