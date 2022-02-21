using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;

    public Transform edge;
    public Transform line;
    public int Radius;
    public int Length;
    Vector3[] vertices;
    Vector3[] center;
    Vector3[] points;
    Vector3[] e1;
    Vector3[] e2;
    int[] triangles;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        centerLine();
        CreateShape();
    }

    void centerLine()
    {
        e1 = new Vector3[Length];
        e2 = new Vector3[Length];
        center = new Vector3[Length];
        points = new Vector3[Length*10];
        for(int index=0;index<Length;index++)
        {
            
            center[index] = new Vector3 ((float)Math.Sin(Math.PI*index*45/180),0,index);
            e1[index] = new Vector3 ((float)(-1*Math.Cos(Math.PI*index*45/180)),0,1);
            e2[index] = new Vector3 (0,(float)(-1*Math.Cos(Math.PI*index*45/180)),1);
            Instantiate(line, center[index], Quaternion.identity);
            edgePoints(index);
        }
    }
    
    void edgePoints(int index)
    {
        for(int angle=0;angle<360;angle+=36)
        {
            double x = Radius*(e1[index][0]*Math.Cos(Math.PI*angle/180)+e2[index][0]*Math.Sin(Math.PI*angle/180));
            double y = Radius*(e1[index][1]*Math.Cos(Math.PI*angle/180)+e2[index][1]*Math.Sin(Math.PI*angle/180));
            double z = Radius*(e1[index][2]*Math.Cos(Math.PI*angle/180)+e2[index][2]*Math.Sin(Math.PI*angle/180));
            int pIndex = (angle/36)+(index*Length);
            points[pIndex] = center[index] + new Vector3 ((float)x,(float)y,(float)z);
            Instantiate(edge, points[pIndex], Quaternion.identity);
            Debug.Log("points: " + points[pIndex]+" index: " + pIndex);
        }
    }

    void CreateShape()
    {
        triangles = new int[]
        {
            0, 1, 10,
            1, 2, 11,
            2, 3, 12,
            3, 4, 13,
            4, 5, 14,
            5, 6, 15,
            6, 7, 16,
            7, 8, 17,
            8, 9, 18,
            9, 0, 19,

            11, 10, 1,
            12, 11, 2,
            13, 12, 3,
            14, 13, 4,
            15, 14, 5,
            16, 15, 6,
            17, 16, 7,
            18, 17, 8,
            19, 18, 9,
            10, 19, 0

        };
    }

    // Update is called once per frame
    void Update()
    {
        
        
        // for(int length=0;length<10;length++)
        // {
        //     Debug.Log("center: " + center[length]);
        // }
        mesh.Clear();
        mesh.vertices = points;
        mesh.triangles = triangles;
    }
}
