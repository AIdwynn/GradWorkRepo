using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class SCRED_AnimationWindow : EditorWindow
{
    public string pathToSave = "Assets/ArtAssets/Animations/VertexAnimations";

    public static bool animationMode = false;

    private int frame = 0;
    private InPogressVertexAnimation[] inPogressVertexAnimations;
    private bool firstTime = true;

    public static void DoWindow()
    {
        var window = GetWindowWithRect<SCRED_AnimationWindow>(new Rect(0, 0, 300, 80));

        var clips = SCRED_WorkingEncodeToVertexAnimation.animations;
        var renderer = SCRED_WorkingEncodeToVertexAnimation.skinnedMeshRenderer;
        var gameObject = SCRED_WorkingEncodeToVertexAnimation.gameObject;

        if (clips != null && renderer != null)
        {
            window.inPogressVertexAnimations = new InPogressVertexAnimation[clips.Length];
            for (int i = 0; i < clips.Length; i++)
            {
                var amountOfFrames = (int)(clips[i].length * clips[i].frameRate);
                window.inPogressVertexAnimations[i] = new InPogressVertexAnimation()
                {
                    texture = new Texture2D(renderer.sharedMesh.vertexCount, amountOfFrames),
                    amountOfFrames = amountOfFrames,
                    amountOfVertexes = renderer.sharedMesh.vertexCount,
                    clip = clips[i],
                    GO = gameObject,

                    minBounds = new Vector3(0, 0, 0),
                    maxBounds = new Vector3(0, 0, 0),
                };
            }

            window.frame = 0;
            window.firstTime = true;
            animationMode = true;
            AnimationMode.StartAnimationMode();
        }

        window.Show();
    }

    public void OnGUI()
    {
        EditorGUILayout.HelpBox("In Progress. \n Currently on frame: " + frame, MessageType.Info);
        if(GUILayout.Button("Cancel"))
        {
            animationMode = false;
            AnimationMode.StopAnimationMode();
            frame = 0;
            firstTime = true;
            inPogressVertexAnimations = null;
            Close();
        }

    }

    private void Update()
    {
        if (firstTime)
        {
            var allDone = false;

            for (int i = 0; i < inPogressVertexAnimations.Length; i++)
            {
                var animation = inPogressVertexAnimations[i];
                allDone = animation.isComplete(frame);
                if (allDone) continue;
                var movedVerts = SampleVertecesInAnimationTime(animation, frame / animation.clip.frameRate);

                for (int j = 0; j < movedVerts.Length; j++)
                {
                    var vertex = movedVerts[j];

                    if (animation.minBounds.x > vertex.x) animation.minBounds.x = vertex.x;
                    if (animation.minBounds.y > vertex.y) animation.minBounds.y = vertex.y;
                    if (animation.minBounds.z > vertex.z) animation.minBounds.z = vertex.z;
                    if (animation.maxBounds.x < vertex.x) animation.maxBounds.x = vertex.x;
                    if (animation.maxBounds.y < vertex.y) animation.maxBounds.y = vertex.y;
                    if (animation.maxBounds.z < vertex.z) animation.maxBounds.z = vertex.z;
                }

                inPogressVertexAnimations[i] = animation;
            }
            
            frame += 1;
            
            if (allDone)
            {
                firstTime = false;
                frame = 0;
            }

            SceneView.RepaintAll();
        }
        else
        {
            var allDone = false;
            for (int i = 0; i < inPogressVertexAnimations.Length; i++)
            {
                var animation = inPogressVertexAnimations[i];
                allDone = animation.isComplete(frame);
                if (allDone) continue;
                var movedVerts = SampleVertecesInAnimationTime(animation, frame / animation.clip.frameRate);
 

                for (int j = 0; j < movedVerts.Length; j++)
                {
                    var vertex = movedVerts[j];
                    var color = SCR_VertexAnimationHelper.EncodeVertexPositionToRGB(animation.minBounds, animation.maxBounds, vertex);

                    var texture = animation.texture;
                    texture.SetPixel(j, frame, color);
                    animation.texture = texture;
                }

                inPogressVertexAnimations[i] = animation;
            }
            if (allDone)
            {
                animationMode = false;
                AnimationMode.StopAnimationMode();
                TrySaveVertexAnimations(inPogressVertexAnimations);

                this.Close();
            }

            frame += 1;

            SceneView.RepaintAll();
        }
    }

    private Vector3[] SampleVertecesInAnimationTime(InPogressVertexAnimation animation, float time)
    {
        AnimationMode.BeginSampling();
        AnimationMode.SampleAnimationClip(animation.GO, animation.clip, time);
        AnimationMode.EndSampling();

        animation.TryGetVerteces(out var verteces);

        return verteces;
    }

    private bool TrySaveVertexAnimations(InPogressVertexAnimation[] animations)
    {
        try
        {
            foreach (var vertexAnimation in animations)
            {
                var path = pathToSave + "/" + vertexAnimation.name + ".asset";

                Debug.Log($"Saving {vertexAnimation.name} to {path} with minBounds: {vertexAnimation.minBounds} and maxBounds: {vertexAnimation.maxBounds}");
                AssetDatabase.CreateAsset(vertexAnimation.texture, path);
            }

            var renderer = animations[0].GO.GetComponentInChildren<SkinnedMeshRenderer>();
           renderer.sharedMesh.uv2 = SCR_VertexAnimationHelper.EncodeVertexIdToUv(renderer.sharedMesh);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return false;
        }
    }
}

public struct InPogressVertexAnimation
{
    public string name { get { return clip.name; } }

    public Texture2D texture;
    public int amountOfFrames;
    public int amountOfVertexes;
    public AnimationClip clip;
    public GameObject GO;


    public Vector3 minBounds;
    public Vector3 maxBounds;

    public bool isComplete(int frame)
    {
        return frame >= amountOfFrames;
    }

    public bool TryGetVerteces(out Vector3[] verteces)
    {
        var rend = GO.GetComponentInChildren<SkinnedMeshRenderer>();
        var target = rend.sharedMesh;
        var bones = rend.bones;
        Vector3[] vertices = new Vector3[target.vertexCount];
        Matrix4x4[] bindposes = target.bindposes;

        for (int i = 0; i < target.vertexCount; i++)
        {
            Matrix4x4 boneMatrix = bones[target.boneWeights[i].boneIndex0].localToWorldMatrix;
            Matrix4x4 bindPoseMatrix = bindposes[target.boneWeights[i].boneIndex0];
            vertices[i] = boneMatrix.MultiplyPoint3x4(bindPoseMatrix.MultiplyPoint3x4(target.vertices[i]));
        }
        
        verteces = vertices.ToArray();
        if (verteces.Length == 0) { throw new Exception("Failed to get verteces"); }
        return true;
    }
}


[CustomEditor(typeof(Animator))]
public class SCRED_WorkingEncodeToVertexAnimation : Editor
{
    public static AnimationClip[] animations;
    public static SkinnedMeshRenderer skinnedMeshRenderer;
    public static GameObject gameObject;

    public override void OnInspectorGUI()
    {
        if (SCRED_AnimationWindow.animationMode) return;
        if (GUILayout.Button("Encode to Vertex Animation"))
        {
            try
            {
                if (!TryGetAnimations(target, out animations)) throw new Exception("Failed to get animations");
                skinnedMeshRenderer = TryGetSkinnedMeshRenderer(target);
                gameObject = (target as Animator).gameObject;

                SCRED_AnimationWindow.DoWindow();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                Debug.Log("Fail Encode to Vertex Animation");
            }

        }
        base.OnInspectorGUI();

    }

   
    private bool TryGetAnimations(Object target, out AnimationClip[] animations)
    {
        var animator = TryGetAnimator(target);
        animations = animator.runtimeAnimatorController.animationClips;
        return true;
    }

    private Animator TryGetAnimator(Object target)
    {
        if (target is Animator) return target as Animator;

        var animator = target.GetComponent<Animator>();
        if (animator == null)
        {
            animator = target.GetComponentInParent<Animator>();
            if (animator == null)
            {
                animator = target.GetComponentInParent<Transform>().GetComponentInParent<Animator>();
                if (animator == null)
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
                if (skinnedMeshRenderer == null)
                    throw new Exception("Failed to get skinned mesh renderer");
            }
        }

        return skinnedMeshRenderer;
    }
}
