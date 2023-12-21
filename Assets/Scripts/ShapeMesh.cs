using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeMesh : MonoBehaviour
{

    public static Mesh RectangleMesh(float width, float length)
    {
        Mesh mesh = new Mesh();
        length = Mathf.Abs(length);
        width = Mathf.Abs(width);

        var polygonPoints = GetRectPoints(width, length);
        var polygonTriangles = DrawFilledRect();
        mesh.vertices = polygonPoints;
        mesh.triangles = polygonTriangles;
        return mesh;
        Vector3[] GetRectPoints(float width, float length)
        {
            List<Vector3> points = new List<Vector3>();

            points.Add(new Vector3(width / 2f, length / 2f, 0f));
            points.Add(new Vector3(-width / 2f, length / 2f, 0f));
            points.Add(new Vector3(-width / 2f, -length / 2f, 0f));
            points.Add(new Vector3(width / 2f, -length / 2f, 0f));

            return points.ToArray();
        }


        int[] DrawFilledRect()
        {
            List<int> newtriangles = new List<int>();
            for (int i = 0; i < 2; i++)
            {
                newtriangles.Add(0);
                newtriangles.Add(i + 2);
                newtriangles.Add(i + 1);
            }
            return newtriangles.ToArray();
        }
    }



    public static Mesh CircleMesh(float size, bool filled)
    {
        Mesh mesh = new Mesh();
        if (filled)
        {
            DrawFIlled(100, size, mesh);
            return mesh;
        }
        return mesh;


        void DrawFIlled(int sides, float radius, Mesh mesh)
        {
            var polygonPoints = GetCircumferencePoints(sides, radius);
            var polygonTriangles = DrawFilledTriangles(polygonPoints);
            mesh.vertices = polygonPoints;
            mesh.triangles = polygonTriangles;




            // Gets Points on circumference to add vertices
            Vector3[] GetCircumferencePoints(int sides, float radius)
            {
                List<Vector3> points = new List<Vector3>();
                float TAU = 2 * Mathf.PI;
                float radianProgressPerStep = TAU / sides;

                for (int i = 0; i < sides; i++)
                {
                    float currentRadian = radianProgressPerStep * i;
                    points.Add(new Vector3(Mathf.Cos(currentRadian) * radius, Mathf.Sin(currentRadian) * radius, 0));
                }

                return points.ToArray();
            }

            // Uses vertices and describes how the triangles are drawn from the points
            int[] DrawFilledTriangles(Vector3[] points)
            {
                int triangleAmount = points.Length - 2;
                List<int> newtriangles = new List<int>();
                for (int i = 0; i < triangleAmount; i++)
                {
                    newtriangles.Add(0);
                    newtriangles.Add(i + 2);
                    newtriangles.Add(i + 1);
                }
                return newtriangles.ToArray();
            }
        }

    }

}
