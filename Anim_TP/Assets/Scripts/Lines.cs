using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lines : MonoBehaviour
{
    public List<Segment> segments;

    private LineRenderer lr;

    void Start() {
        //segments = new List<Segment>(GameObject.Find("Segments").GetComponentsInChildren<Segment>());
    }


    void Update()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = segments.Count * 2;

        for (int i = 0; i < segments.Count; i++) {

            lr.SetPosition( (i * 2)    , segments[i].left.transform.position);
            lr.SetPosition( (i * 2) + 1, segments[i].right.transform.position);

        }
    }

    void OnDrawGizmos() {
        for (int i = 0; i < segments.Count; i++) {
            if (segments[i] != null) {
                Gizmos.DrawLine(segments[i].left.transform.position, segments[i].right.transform.position);
            }
        }
    }

}
