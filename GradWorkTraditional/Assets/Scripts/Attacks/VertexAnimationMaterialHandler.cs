using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class VertexAnimationMaterialHandler
{
    private List<Material> _vertexAnimationMats;
    public List<Material> VertexAnimationMaterials
    {
        get { return _vertexAnimationMats; }
    }

    private int _amountOfFrames = 43;
    private float _clipLength = 1.433f;
    private int offset;
    private float currentTime;
    private Random _random;
    private string VariableToChange = "_Frames";

    public VertexAnimationMaterialHandler(Material baseMaterial, int amountOfMaterials, int amountOfFrames, float clipLength)
    {
        _amountOfFrames = amountOfFrames;
        _clipLength = clipLength;
        offset = amountOfFrames/amountOfMaterials ;
        currentTime = 0;
        _random = new Random();

        _vertexAnimationMats = new List<Material>();
        for (int i = 0; i < amountOfMaterials; i++)
        {
            var mat = new Material(baseMaterial);
            _vertexAnimationMats.Add(mat);
        }
    }

    public void Update(float TimeDeltaTime)
    {
        currentTime += TimeDeltaTime;
        var current = (currentTime % _clipLength) / _clipLength;
        var frame = _amountOfFrames * current;

        for (int i = 0; i < VertexAnimationMaterials.Count; i++)
        {
            var mat = VertexAnimationMaterials[i];
            mat.SetFloat(VariableToChange, ((frame + (offset*i)) % _amountOfFrames) / _amountOfFrames);
        }
    }

    public Material GetRandomMaterial()
    {
        return VertexAnimationMaterials[_random.Next(0, VertexAnimationMaterials.Count)];
    }
}
