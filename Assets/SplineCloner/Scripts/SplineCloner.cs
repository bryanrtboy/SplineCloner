using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JPBotelho;

[ExecuteInEditMode]
public class SplineCloner : MonoBehaviour
{
    public CatmullRom m_spline;
    public Transform[] m_controlPoints;
    [Range(2, 25)]
    public int m_resolution = 2;
    public bool m_closedLoop;
    [Range(0, 20)]
    public float m_normalExtrusion;
    [Range(0, 20)]
    public float m_tangentExtrusion;
    public bool m_drawNormal, m_drawTangent;
    public GameObject m_objectToPlace;
    public float m_distanceBetweenObjects = .1f;

    public List<GameObject> clones;

    void Start()
    {
        if (m_spline == null)
        {
            m_spline = new CatmullRom(m_controlPoints, m_resolution, m_closedLoop);
        }
    }

    void Update()
    {
        if (m_spline != null)
        {
            m_spline.Update(m_controlPoints);
            m_spline.Update(m_resolution, m_closedLoop);
            m_spline.DrawSpline(Color.white);

            if (m_drawNormal)
                m_spline.DrawNormals(m_normalExtrusion, Color.red);

            if (m_drawTangent)
                m_spline.DrawTangents(m_tangentExtrusion, Color.cyan);
        }
        else
        {
            m_spline = new CatmullRom(m_controlPoints, m_resolution, m_closedLoop);
        }
    }

    public void PlaceObjects()
    {
#if UNITY_EDITOR
        foreach (GameObject go in clones)
            DestroyImmediate(go);
#else
        foreach (GameObject go in clones)
            Destroy(go);

#endif

        if (m_objectToPlace == null || m_spline == null)
        {
            Debug.LogError("Nothing to Place!");
            return;
        }

        var points = m_spline.GetPoints();

        clones = new List<GameObject>();

        //distanceToMove represents how much farther
        //we need to progress down the spline before
        //we place the next object
        int nextSplinePointIndex = 1;
        float distanceToMove = m_distanceBetweenObjects;

        //our current position on the spline
        Vector3 positionIterator = points[0].position;

        ////our algo skips the first control point, so 
        ////we need to manually place the first object
        GameObject b = Instantiate(m_objectToPlace, positionIterator, Quaternion.identity);
        clones.Add(b);
        Vector3 direction = (points[nextSplinePointIndex].position - positionIterator);
        b.transform.rotation = Quaternion.LookRotation(direction);


        for (int i = 0; i < points.Length - 1; i++)
        {
            while (nextSplinePointIndex < points.Length)
            {
                direction = (points[nextSplinePointIndex].position - positionIterator);
                direction = direction.normalized;
                float distanceToNextPoint = Vector3.Distance(positionIterator, points[nextSplinePointIndex].position);
                if (distanceToNextPoint >= distanceToMove)
                {
                    positionIterator += direction * distanceToMove;

                    GameObject go = Instantiate(m_objectToPlace, positionIterator, Quaternion.identity);
                    clones.Add(go);
                    go.transform.rotation = Quaternion.LookRotation(direction);
                    distanceToMove = m_distanceBetweenObjects;
                }
                else
                {
                    distanceToMove -= distanceToNextPoint;
                    positionIterator = points[nextSplinePointIndex++].position;
                }
            }

        }

    }
}
