using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace Spline
{
    [ExecuteInEditMode]
    public class SplineCloner : MonoBehaviour
    {
        public CatmullRom m_spline;
        public Transform[] m_controlPoints = new Transform[3];
        public int m_resolution = 2;
        public bool m_closedLoop;
        public float m_normalExtrusion;
        public float m_tangentExtrusion;
        public bool m_drawNormal, m_drawTangent;
        public GameObject m_objectToClone;
        public float m_distanceBetweenObjects = .1f;
        public AnimationCurve m_scaleCurve = AnimationCurve.Linear(0, 0, 1, 0);
        public Vector2 m_scaleMinMax = Vector2.one;

        List<GameObject> m_clones;

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
            CleanUpScene();

            if (m_objectToClone == null || m_spline == null)
            {
                Debug.LogError("Nothing to Place!");
                return;
            }

            var points = m_spline.GetPoints();

            m_clones = new List<GameObject>();

            //distanceToMove represents how much farther
            //we need to progress down the spline before
            //we place the next object
            int nextSplinePointIndex = 1;
            float distanceToMove = m_distanceBetweenObjects;

            //our current position on the spline
            Vector3 positionIterator = points[0].position;

            ////our algo skips the first control point, so 
            ////we need to manually place the first object
            GameObject b = Instantiate(m_objectToClone, positionIterator, Quaternion.identity);
            m_clones.Add(b);
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

                        GameObject go = Instantiate(m_objectToClone, positionIterator, Quaternion.identity);
                        m_clones.Add(go);
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

            float increment = 1.0f / m_clones.Count;

            for (int i = 0; i < m_clones.Count; i++)
            {
                m_clones[i].tag = "Respawn";

                float scale = Mathf.Lerp(m_scaleMinMax.x, m_scaleMinMax.y, m_scaleCurve.Evaluate(i * increment));
                m_clones[i].transform.localScale = Vector3.one * scale;
            }

        }

        void CleanUpScene()
        {
            if (m_clones == null)
            {
                GameObject[] gos = GameObject.FindGameObjectsWithTag("Respawn");
                if (gos != null)
                {
                    m_clones = new List<GameObject>();
                    foreach (GameObject g in gos)
                        m_clones.Add(g);

                }

            }

#if UNITY_EDITOR
            if (m_clones != null)
                foreach (GameObject go in m_clones)
                    DestroyImmediate(go);
#else
            if (m_clones != null)
                foreach (GameObject go in m_clones)
                    Destroy(go);

#endif


        }
    }
}
