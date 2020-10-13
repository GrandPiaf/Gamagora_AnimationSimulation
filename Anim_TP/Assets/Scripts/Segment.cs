using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Represents a bone between 2 joints for a skeleton
 * Can have constraints (distances)
 */
public class Segment
{
    // Joints
    public readonly Joint left;
    public readonly Joint right;

    // Distance to respect between both joints
    public readonly float distance;

    public Segment(Joint left, Joint right) {
        this.left = left;
        this.right = right;
        distance = CalculateDistance();
    }

    public float CalculateDistance() {
        return (right.transform.position - left.transform.position).magnitude;
    }
}
