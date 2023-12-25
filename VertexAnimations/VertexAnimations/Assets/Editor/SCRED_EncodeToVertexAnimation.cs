using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using Object = UnityEngine.Object;

//[CustomEditor(typeof(Animator))]
public class SCRED_EncodeToVertexAnimation :Editor
{
    public string pathToSave = "Assets/ArtAssets/Animations/VertexAnimations";

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Encode to Vertex Animation"))
        {
            try
            {
                Debug.Log("Start Encode to Vertex Animation");

                AnimationMode.StartAnimationMode();
                
                if(!TryGetAnimations(target, out var animations)) throw new Exception("Failed to get animations");
                if(!TryEncodeToVertexAnimation(TryGetSkinnedMeshRenderer(target), animations, out var vertexAnimations)) throw new Exception("Failed to encode to vertex animation");
                if(!TrySaveVertexAnimations(animations, vertexAnimations)) throw new Exception("Failed to save vertex animation");
                
                AnimationMode.StopAnimationMode();

                Debug.Log("Complete Encode to Vertex Animation");
            }
            catch(Exception e)
            {
                Debug.LogError(e);
                Debug.Log("Fail Encode to Vertex Animation");
            }
            
        }
        base.OnInspectorGUI();

    }

    private bool TrySaveVertexAnimations(AnimationClip[] animations, Texture2D[] vertexAnimations)
    {
        try
        {
            foreach (var vertexAnimation in vertexAnimations)
            {
                var animation = animations[Array.IndexOf(vertexAnimations, vertexAnimation)];
                var path = pathToSave + "/" + animation.name + ".asset";
                AssetDatabase.CreateAsset(vertexAnimation, path);
            }
            return true;
        }
        catch(Exception e)
        {
            Debug.LogError(e);
            return false;
        }
    }

    private bool TryEncodeToVertexAnimation(SkinnedMeshRenderer target, AnimationClip[] animations, out Texture2D[] vertexAnimation)
    {
        vertexAnimation = new Texture2D[animations.Length];

        TryGetAmountOfVertexes(target.sharedMesh, out var amountOfVertexes);
        var animator = TryGetAnimator(target);
        for (int n = 0; n < animations.Length; n++)
        {
            var animation = animations[n];

            var frames = (int)(animation.length * animation.frameRate);
            var frameTime = animation.length / frames;

            var texture = new Texture2D(amountOfVertexes, frames, TextureFormat.RGBAFloat, false);

            Vector3 minBounds = new Vector3();
            Vector3 maxBounds = new Vector3();
            for (int i = 0; i < frames; i++)
            {
                var verteces = SampleVertecesInAnimationTime(target, animation, frameTime * i);
                for (int j = 0; j < amountOfVertexes; j++)
                {
                    var vertex = verteces[j];
                    if(minBounds.x > vertex.x) minBounds.x = vertex.x;
                    if(minBounds.y > vertex.y) minBounds.y = vertex.y;
                    if(minBounds.z > vertex.z) minBounds.z = vertex.z;
                    if(maxBounds.x < vertex.x) maxBounds.x = vertex.x;
                    if(maxBounds.y < vertex.y) maxBounds.y = vertex.y;
                    if(maxBounds.z < vertex.z) maxBounds.z = vertex.z;
                }
            }

            Debug.Log("minBounds: " + minBounds);
            Debug.Log("maxBounds: " + maxBounds);
            for (int i = 0; i < frames; i++)
            {
                var verteces = SampleVertecesInAnimationTime(target, animation, frameTime * i);
                for (int j = 0; j < amountOfVertexes; j++)
                {
                    var vertex = verteces[j];
                    var color = SCR_VertexAnimationHelper.EncodeVertexPositionToRGB(minBounds, maxBounds, vertex);
                    texture.SetPixel(j, i, color);
                }
            }


            vertexAnimation[n] = texture;
        }

        target.sharedMesh.uv2 = SCR_VertexAnimationHelper.EncodeVertexIdToUv(target.sharedMesh);
        return true;
    }

    private Vector3[] SampleVertecesInAnimationTime(SkinnedMeshRenderer target, AnimationClip animation, float time)
    {
        AnimationMode.BeginSampling();
        AnimationMode.SampleAnimationClip(target.gameObject, animation, time);
        AnimationMode.EndSampling();
        
        TryGetVerteces(target, out var verteces);

        return verteces;
    }
    
    /*
     private Vector3[] SampleVertecesInAnimationTime(SkinnedMeshRenderer target, Animator animator, AnimationClipPlayable animation, float time)
    {
        var playableOutput = AnimationPlayableOutput.Create(playableGraph, "Animation", animator);
        playableOutput.SetSourcePlayable(animation);
        playableGraph.Play();
        animation.Pause();
        animation.SetTime(time);
        

        TryGetVerteces(target.sharedMesh, out var verteces);
        return verteces;
        
    }
    */
    private bool TryGetAnimations(Object target, out AnimationClip[] animations)
    {
        var animator = TryGetAnimator(target);
        animations = animator.runtimeAnimatorController.animationClips;
        return true;
    }

    private Animator TryGetAnimator(Object target)
    {
        if(target is Animator) return target as Animator;
        
        var animator = target.GetComponent<Animator>();
        if (animator == null)
        {
            animator = target.GetComponentInParent<Animator>();
            if (animator == null)
            {
                animator = target.GetComponentInParent<Transform>().GetComponentInParent<Animator>();
                if(animator == null)
                    throw new Exception("Failed to get animator");
            }
        }

        return animator;
    }
    
    private SkinnedMeshRenderer TryGetSkinnedMeshRenderer(Object target)
    {
        var skinnedMeshRenderer = target as SkinnedMeshRenderer;
        if (skinnedMeshRenderer == null)
        {
            skinnedMeshRenderer = target.GetComponentInChildren<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer == null)
            {
                skinnedMeshRenderer = target.GetComponentInChildren<Transform>().GetComponentInChildren<SkinnedMeshRenderer>();
                if(skinnedMeshRenderer == null)
                    throw new Exception("Failed to get skinned mesh renderer");
            }
        }

        return skinnedMeshRenderer;
    }

    private bool TryGetAmountOfVertexes(Mesh target, out int amountOfVertexes)
    {
        amountOfVertexes = target.vertexCount;
        if (amountOfVertexes == 0) { throw new Exception("Failed to get amount verteces"); }
        return true;
    }
    private bool TryGetVerteces(SkinnedMeshRenderer renderer, out Vector3[] verteces)
    {
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
        
        
        verteces = vertices;
        if(verteces.Length == 0 ) { throw new Exception("Failed to get verteces"); }
        return true;
    }
}
