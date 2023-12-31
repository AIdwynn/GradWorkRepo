using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Animator))]
public class SCRED_CheckBoneWeights : SCRED_WorkingEncodeToVertexAnimation
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Check Bone Weights"))
        {
            var GO = (target as Animator).gameObject;
            Mesh mesh = GO.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh;

            for (int i = 0; i < mesh.vertexCount; i++)
            {
                BoneWeight boneWeight = mesh.boneWeights[i];

                // Check how many bones influence this vertex
                int numInfluences = 0;
                if (boneWeight.weight0 > 0) numInfluences++;
                if (boneWeight.weight1 > 0) numInfluences++;
                if (boneWeight.weight2 > 0) numInfluences++;
                if (boneWeight.weight3 > 0) numInfluences++;

                    Debug.Log($"Vertex {i} has {numInfluences} bone weights.");

            }

            
        }
        
        base.OnInspectorGUI();
    }
}
