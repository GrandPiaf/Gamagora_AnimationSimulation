using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lines : MonoBehaviour
{

    public List<GameObject> points;
    public List<int> lines;

    private LineRenderer lr;

    // Start is called before the first frame update
    void Update()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = lines.Count;

        for (int i = 0; i < lines.Count; i+=2) {

            lr.SetPosition(i, points[lines[i]].transform.position);
            lr.SetPosition(i+1, points[lines[i+1]].transform.position);

        }
    }

    void OnDrawGizmos() {
        for (int i = 0; i < lines.Count; i += 2) {
            Gizmos.DrawLine(points[lines[i]].transform.position, points[lines[i + 1]].transform.position);
        }
    }

}
