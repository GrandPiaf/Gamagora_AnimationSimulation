using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lines : MonoBehaviour
{

    public List<Vector3> vertices;
    public List<int> lines;

    private LineRenderer lr;

    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = lines.Count;
        for (int i = 0; i < lines.Count; i+=2) {
            lr.SetPosition(i, vertices[lines[i]]);
            lr.SetPosition(i+1, vertices[lines[i+1]]);
        }
    }

}
