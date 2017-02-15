using System;
using System.Collections.Generic;
using UnityEngine;

internal class WireframeEdge
{
    public int Index1;
    public int Index2;
    public Vector3 FaceNormal1;
    public Vector3 FaceNormal2;
    public bool Duplicate;

    public WireframeEdge(int i1, int i2, Vector3 f1 = new Vector3(), Vector3 f2 = new Vector3(), bool duplicate = false)
    {
        Index1 = i1;
        Index2 = i2;
        FaceNormal1 = f1;
        FaceNormal2 = f2;
        Duplicate = duplicate;
    }
}

public class Wireframe
{


// ----------------------------------------------------------------------------------------------------
// Make indices
//
    public static int[] MakeIndices(int[] triangles)
    {
        int[] indices = new int[2 * triangles.Length];

        var j = 0;
        var len = triangles.Length;
        for (int i = 0; i < len; i += 3)
        {
            indices[j + 5] = indices[j] = triangles[i];
            indices[j + 1] = indices[j + 2] = triangles[i + 1];
            indices[j + 3] = indices[j + 4] = triangles[i + 2];
            j += 6;
        }
        return indices;
    }



    public static int[] MakeIndicesNoDuplicates(int[] triangles, Vector3[] vertices)
    {
        bool[,] frags = new bool[vertices.Length, vertices.Length];
        List<int> list = new List<int>(2 * triangles.Length);

        int len = triangles.Length / 3;

        for (int i = 0; i < len; i++)
        {
            int k = i * 3;
            for (int j = 0; j < 3; j++)
            {
                int index1 = triangles[k + j];
                int index2 = triangles[k + (j + 1) % 3];

                if (index1 > index2)
                {
                    int tmp = index1;
                    index1 = index2;
                    index2 = tmp;
                }

                if (!frags[index1, index2])
                {
                    frags[index1, index2] = true;
                    list.Add(index1);
                    list.Add(index2);
                }
            }
        }

        len = list.Count;
        int[] indices = new int[len];
        for (int i = 0; i < len; i++)
        {
            indices[i] = list[i];
        }

        return indices;
    }



    public static int[] MakeIndicesOnlyEdges(int[] triangles, Vector3[] vertices, float thresholdAngle)
    {
        double thresholdDot = Math.Cos(Math.PI / 180.0 * thresholdAngle);

        Vector3[] faceNormals = _FaceNormals(triangles, vertices);

        WireframeEdge[,] weArray = new WireframeEdge[vertices.Length, vertices.Length];
        List<WireframeEdge> weListAll = new List<WireframeEdge>(2 * triangles.Length);
        List<WireframeEdge> weListRest = new List<WireframeEdge>(2 * triangles.Length);


        int i, j;


        // ------------------------------
        int faceNum = faceNormals.Length;
        for (i = 0; i < faceNum; i++)
        {
            int k = i * 3;
            Vector3 faceNormal = faceNormals[i];
            for (j = 0; j < 3; j++)
            {
                int index1 = triangles[k + j];
                int index2 = triangles[k + (j + 1) % 3];

                if (index1 > index2)
                {
                    int tmp = index1;
                    index1 = index2;
                    index2 = tmp;
                }

                WireframeEdge we = weArray[index1, index2];
                if (we == null)
                {
                    we = new WireframeEdge(index1, index2, faceNormal);
                    weListAll.Add(we);
                    weArray[index1, index2] = we;
                }
                else
                {
                    we.Duplicate = true;
                    we.FaceNormal2 = faceNormal;
                }
            }
        }



        // ------------------------------
        int weNum = weListAll.Count;
        for (i = 0; i < weNum; i++)
        {
            WireframeEdge we = weListAll[i];
            if (!we.Duplicate || Vector3.Dot(we.FaceNormal1, we.FaceNormal2) <= thresholdDot)
            {
                weListRest.Add(we);
            }
        }



        weNum = weListRest.Count;
        int[] indices = new int[weNum * 2];
        j = 0;
        for (i = 0; i < weNum; i++)
        {
            WireframeEdge we = weListRest[i];
            indices[j] = we.Index1;
            indices[j+1] = we.Index2;
            j += 2;
        }

        return indices;
    }



// ----------------------------------------------------------------------------------------------------
// Utils
//
    private static Vector3 _FaceNormal(Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 cb = c - b;
        Vector3 ab = a - b;

        Vector3 normal = Vector3.Cross(cb, ab);
        normal.Normalize();

        return normal;
    }


    private static Vector3[] _FaceNormals(int[] triangles, Vector3[] vertices)
    {
        int faceNum = triangles.Length / 3;

        Vector3[] faceNormals = new Vector3[faceNum];

        // ------------------------------
        for (int i = 0; i < faceNum; i++)
        {
            int j = i * 3;
            faceNormals[i] = _FaceNormal(
                vertices[triangles[j]],
                vertices[triangles[j+1]],
                vertices[triangles[j+2]]
            );
        }

        return faceNormals;
    }

}
