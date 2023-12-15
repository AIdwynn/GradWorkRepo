using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SCR_VertexAnimationHelper
{
    public static Color EncodeVertexPositionToRGB(Vector3 minPos, Vector3 maxPos, Vector3 vertexPosition)
    {
        float r = Mathf.InverseLerp(minPos.x, maxPos.x, vertexPosition.x);
        float g = Mathf.InverseLerp(minPos.y, maxPos.y, vertexPosition.y);
        float b = Mathf.InverseLerp(minPos.z, maxPos.z, vertexPosition.z);
        return new Color(r, g, b);
    }

    public static Vector3 DecodeVertexPositionFromRGB(float minPos, float maxPos, Color rgb)
    {
        float x = Mathf.Lerp(minPos, maxPos, rgb.r);
        float y = Mathf.Lerp(minPos, maxPos, rgb.g);
        float z = Mathf.Lerp(minPos, maxPos, rgb.b);
        return new Vector3(x, y, z);
    }

    public static Vector2[] EncodeVertexIdToUv(Mesh mesh)
    {
        float offset = 1f / mesh.vertexCount * 0.5f;
        var uvs = new Vector2[mesh.vertexCount];
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            float uvx = Remap(i, mesh.vertexCount) + offset;
            uvs[i] = new Vector2(uvx, 1f);
        }
        return uvs;
    }

    private static float Remap(int i, int vertexCount) => i / (float)(vertexCount - 1);
    }

