using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;

    public GameObject edge;
    public GameObject line;
    public double Radius;
    
    public int Length;
    Vector3[] vertices;
    Vector3[] center;
    Vector3[] points;
    Vector3[] e0;
    Vector3[] e1;
    Vector3[] e2;
    Vector3 ab;
    Vector3 bc;
    Vector3 average;
    Vector3[] oldc;
    int[] triangles;
    float direction = 0.0f;
    float wave;
    float time = 0.0f;
    float intervals = 0.0f;
    

    // Start is called before the first frame update
    void Start()
    {
        center = new Vector3[Length];
        oldc = new Vector3[Length];
        //Radius=Radius/10;
        
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        centerLine(time);
        CreateShape();

        
    }

    void centerLine(float time)
    {
        e0 = new Vector3[Length];
        e1 = new Vector3[Length];
        e2 = new Vector3[Length];

        points = new Vector3[Length*10];
        Debug.Log("time: " + time);

        wave = (float)Math.Sin(Math.PI*(time)*45/180);
        
        center[0] = new Vector3 (wave*(direction)+(time)*(1-direction),0,wave*(1-direction)+(time)*(direction));

        for(int index=1;index<Length;index++)
        {
            center[index] = oldc[index-1];
            if(index>0&&index<Length-1)
            {

                ab = center[index]-center[index-1];
                bc = center[index+1]-center[index];
                average = ab+bc;
                average=new Vector3 (average[0]/2,average[1]/2,average[2]/2);
            }
            else
            {
                average = center[index];
            }



            e0[index] = new Vector3 (average[0],average[1],average[2]);
            e1[index] = new Vector3 ((float)(-1*Math.Cos(Math.PI*index*45/180)),0,1);
            e2[index] = new Vector3 (e0[index][0],1,e0[index][2]);
            Instantiate(line, center[index], Quaternion.identity);
            //Debug.Log("points: " + center[index]);
            edgePoints(index);
        }
        oldc = center;
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
            //Instantiate(edge, points[pIndex], Quaternion.identity);
            //Debug.Log("points: " + points[pIndex]+" index: " + pIndex);
        }
    }

    void CreateShape()
    {
        triangles = new int[(Length-1)*60];
        for(int tIndex=0;tIndex<2-1;tIndex++)
        {
            int count = 0;
            for(int index=0;index<30;index+=3)
            {
                if(index+3>=30)
                {
                    triangles[index+(tIndex*10)]=(tIndex*10);
                }
                else
                {
                    triangles[index+(tIndex*10)]=count+1+(tIndex*10);
                }
                triangles[index+1+(tIndex*10)]=count+(tIndex*10);
                triangles[index+2+(tIndex*10)]=count+10+(tIndex*10);
                //str = (triangles[index+(tIndex*10)]).ToString()+","+(triangles[index+1+(tIndex*10)]).ToString()+","+(triangles[index+2+(tIndex*10)]).ToString();
                //Debug.Log(str);
                count++;
            }
            count = 0;
            for(int index=30;index<60;index+=3)
            {

                triangles[index+(tIndex*10)]=count+10+(tIndex*10);

                if(index+3>=60)
                {
                    triangles[index+1+(tIndex*10)]=10+(tIndex*10);
                    triangles[index+2+(tIndex*10)]=(tIndex*10);
                }
                else
                {
                    triangles[index+1+(tIndex*10)]=count+11+(tIndex*10);
                    triangles[index+2+(tIndex*10)]=count+1+(tIndex*10);
                }
                //str = (triangles[index+(tIndex*10)]).ToString()+","+(triangles[index+1+(tIndex*10)]).ToString()+","+(triangles[index+2+(tIndex*10)]).ToString();
                //Debug.Log(str);
                count++;
            }
        }
        
        // {
        //     1, 0, 10,
        //     2, 1, 11,
        //     3, 2, 12,
        //     4, 3, 13,
        //     5, 4, 14,
        //     6, 5, 15,
        //     7, 6, 16,
        //     8, 7, 17,
        //     9, 8, 18,
        //     0, 9, 19,

        //     10, 11, 1,
        //     11, 12, 2,
        //     12, 13, 3,
        //     13, 14, 4,
        //     14, 15, 5,
        //     15, 16, 6,
        //     16, 17, 7,
        //     17, 18, 8,
        //     18, 19, 9,
        //     19, 10, 0
        // };
    }

    // Update is called once per frame
    void Update()
    {
        time+= Time.deltaTime;
        if(time-intervals>1)
        {
            if(direction==0){direction=1;}else{direction=0;}
            Debug.Log(time);
            mesh.Clear();
            centerLine(time);
            mesh.vertices = points;
            mesh.triangles = triangles;
            intervals=time;
        }
    }
}
