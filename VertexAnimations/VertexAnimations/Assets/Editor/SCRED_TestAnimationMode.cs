using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class SCRED_TestAnimationMode : EditorWindow
{
    protected GameObject go;
    protected AnimationClip animationClip;
    protected float time = 0.0f;
    protected bool lockSelection = false;
    protected bool animationMode = false;

    [MenuItem("Examples/AnimationMode demo", false, 2000)]
    public static void DoWindow()
    {
        var window = GetWindowWithRect<SCRED_TestAnimationMode>(new Rect(0, 0, 300, 80));
        window.Show();
    }

    // Has a GameObject been selection?
    public void OnSelectionChange()
    {
        if (!lockSelection)
        {
            go = Selection.activeGameObject;
            Repaint();
        }
    }

    // Main editor window
    public void OnGUI()
    {
        // Wait for user to select a GameObject
        if (go == null)
        {
            EditorGUILayout.HelpBox("Please select a GameObject", MessageType.Info);
            return;
        }

        // Animate and Lock buttons.  Check if Animate has changed
        GUILayout.BeginHorizontal();
        EditorGUI.BeginChangeCheck();
        GUILayout.Toggle(AnimationMode.InAnimationMode(), "Animate");
        if (EditorGUI.EndChangeCheck())
            ToggleAnimationMode();

        GUILayout.FlexibleSpace();
        lockSelection = GUILayout.Toggle(lockSelection, "Lock");
        GUILayout.EndHorizontal();

        // Slider to use when Animate has been ticked
        EditorGUILayout.BeginVertical();
        animationClip = EditorGUILayout.ObjectField(animationClip, typeof(AnimationClip), false) as AnimationClip;
        if (animationClip != null)
        {
            float startTime = 0.0f;
            float stopTime  = animationClip.length;
            time = EditorGUILayout.Slider(time, startTime, stopTime);
        }
        else if (AnimationMode.InAnimationMode())
            AnimationMode.StopAnimationMode();
        EditorGUILayout.EndVertical();
    }
    
    Vector3[] lastVertices = null;
    void Update()
    {
        if (go == null)
            return;

        if (animationClip == null)
            return;

        // Animate the GameObject
        if (!EditorApplication.isPlaying && AnimationMode.InAnimationMode())
        {
            time+=Time.deltaTime;
            if(time > animationClip.length)
                time = 0.0f;

            AnimationMode.BeginSampling();
            AnimationMode.SampleAnimationClip(go, animationClip, time);
            AnimationMode.EndSampling();

            /*var renderer = go.GetComponentInChildren<SkinnedMeshRenderer>();
            var target = renderer.sharedMesh;
            var bones = renderer.bones;
            Vector3[] vertices = new Vector3[target.vertexCount];
            Matrix4x4[] bindposes = target.bindposes;

            for (int i = 0; i < target.vertexCount; i++)
            {
                Matrix4x4 boneMatrix = bones[target.boneWeights[i].boneIndex0].localToWorldMatrix;
                Matrix4x4 bindPoseMatrix = bindposes[target.boneWeights[i].boneIndex0];
                vertices[i] = boneMatrix * bindPoseMatrix.MultiplyPoint(target.vertices[i]);
            }

            try
            {
                for (int i = 0; i < vertices.Length; i++)
                {
                    if (vertices[i] != lastVertices[i])
                    {
                        Debug.Log("Vertex " + i + " has changed from " + lastVertices[i] + " to " + vertices[i]);
                        lastVertices[i] = vertices[i];
                    }
                }

            }
            catch (Exception e)
            {
                lastVertices = vertices;
            }*/


            //go.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh.GetBlendShapeFrameVertices(0, frameIndex, go.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh.vertices, null, null);

            SceneView.RepaintAll();
        }
    }

    void ToggleAnimationMode()
    {
        if (AnimationMode.InAnimationMode())
            AnimationMode.StopAnimationMode();
        else
            AnimationMode.StartAnimationMode();
    }
}
