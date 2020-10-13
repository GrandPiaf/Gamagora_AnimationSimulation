using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Represents a bone between 2 joints for a skeleton
 * Can have constraints (distances)
 */
public class Segment : MonoBehaviour
{
    // Joints
    public Joint left;
    public Joint right;

    // Distance to respect between both joints
    private float distanceSquared;
}
