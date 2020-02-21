using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(MeshCombiner))]
public class MeshCombinerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MeshCombiner combiner = (MeshCombiner)target;
        if(GUILayout.Button("Combine mesh"))
        {
            combiner.MeshCombine();
        }
    }
}
