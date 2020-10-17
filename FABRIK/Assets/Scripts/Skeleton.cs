using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : MonoBehaviour
{
    // Entry data
    public List<Joint> joints;
    public Joint target;

    private List<Segment> segments;

    // Renderer
    private LineRenderer lr;

    void Start() {

        lr = GetComponent<LineRenderer>();

        // Compute segments
        segments = new List<Segment>();
        for (int i = 0; i < joints.Count - 1; i++) {
            segments.Add(new Segment(joints[i], joints[i + 1]));
        }

        // Compute inverse kinematics
        inverseKinematics(5);
    }

    private void inverseKinematics(int nbIterations) {

        // Place Joint on target
        // Resolve constraints as we rewind the list

        Vector3 initialPos = joints[0].transform.position;
        Vector3 targetPos = target.transform.position;

        for (int ite = 0; ite < nbIterations; ite++) {

            joints.Reverse();
            segments.Reverse();

            joints[0].transform.position = targetPos;
            for (int i = 0; i < segments.Count; i++) {
                segments[i].left.transform.position = segments[i].right.transform.position + segments[i].distance * (segments[i].left.transform.position - segments[i].right.transform.position).normalized;
            }

            joints.Reverse();
            segments.Reverse();

            joints[0].transform.position = initialPos;
            for (int i = 0; i < segments.Count; i++) {
                segments[i].right.transform.position = segments[i].left.transform.position + segments[i].distance * (segments[i].right.transform.position - segments[i].left.transform.position).normalized;
            }

        }

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
        for (int i = 0; i < joints.Count - 1; i++) {
            Gizmos.DrawLine(joints[i].transform.position, joints[i+1].transform.position);
        }
    }

}
