using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class EffectLineRenderer : MonoBehaviour {

    public GameObject end;
    public LineRenderer lineRender;
    public int segments = 20;

	void Update ()
    {
        if (end == null)
        {
            end = new GameObject();
            end.name = "End";
            end.transform.parent = transform;
        }

        if (lineRender == null)
        {
            lineRender = gameObject.GetComponent<LineRenderer>();
            if (lineRender == null)
                lineRender = gameObject.AddComponent<LineRenderer>();
        }

        Vector3 direction = end.transform.position - transform.position;
        direction.Normalize();
        float dist = Vector3.Distance(end.transform.position, transform.position);
        List<Vector3> positions = new List<Vector3>();
        positions.Add(transform.position);

        for (int i=0; i<segments-2; i++)
        {
            positions.Add(transform.position + direction * (dist * ((float)i / ((float)segments - 2f))));
        }

        positions.Add(end.transform.position);

        lineRender.positionCount = positions.Count;
        lineRender.SetPositions(positions.ToArray());


	}
}
