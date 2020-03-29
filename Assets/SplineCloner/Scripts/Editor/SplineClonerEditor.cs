using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(SplineCloner))]
public class SplinePClonerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Place Objects"))
        {
            SplineCloner placer = (SplineCloner)target;
            placer.PlaceObjects();
        }
    }
}