using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : MonoBehaviour
{
    // Entry data
    public List<Segment> segments;
    public Joint target;

    // Results
    private List<Segment> segmentsResults;

    // Renderer
    private LineRenderer lr;

    void Start() {

        lr = GetComponent<LineRenderer>();

        // Create result list
        segmentsResults = new List<Segment>(segments);
    }

    void Update() {
        // Render entry data
        lr.positionCount = segments.Count * 2;
        for (int i = 0; i < segments.Count; i++) {
            lr.SetPosition((i * 2), segments[i].left.transform.position);
            lr.SetPosition((i * 2) + 1, segments[i].right.transform.position);
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
