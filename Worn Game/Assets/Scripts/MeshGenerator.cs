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
    public float Radius;
    public float speed;
    public float turnSpeed;
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
    Vector3 newC0;
    int[] triangles;
    float[,] wormShape;
    float[,] tan;
    float[,] e0_cal;
    float[,] e2_cal;
    float[] e0_norm;
    float[] tan_norm;
    float direction = 0.0f;
    float wave,x,z,waveC,ForwardC;
    float time = 0.0f;
    float intervals = 0.0f;

    String result="";
    

    // Start is called before the first frame update
    void Start()
    {
        center = new Vector3[Length];
        //Radius=Radius/10;
        
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        centerLine(time);
        CreateShape();

        
    }

    void directionIncrement(float key)
    {
        if(key<0)
        {
            direction+=(turnSpeed/10);
        }
        else if(key>0)
        {
            direction-=(turnSpeed/10);
        }
        if(direction>=360)
        {
            direction-=360;
        }
        else if(direction<=-360)
        {
            direction+=360;
        }
        waveC = (float)Math.Sin(Math.PI*(direction)/180);
        ForwardC = (float)Math.Cos(Math.PI*(direction)/180);
    }

    float keyDetection()
    {
        return Input.GetAxis("Horizontal");
    }

    void centerLine(float time)
    {
        e0 = new Vector3[Length];
        e1 = new Vector3[Length];
        e2 = new Vector3[Length];

        points = new Vector3[Length*10];

        wave = (float)Math.Sin(Math.PI*(time)*150/180);
        x=(wave*waveC)+(ForwardC);
        z=(wave*ForwardC)+(waveC);
        newC0 = new Vector3 (center[0][0]+(x*speed),0,center[0][2]+(z*speed));
        if(newC0 != center[0])
        {
            for(int index=Length-1;index>0;index--)
            {
                center[index] = center[index-1];
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
                Instantiate(line, center[index], Quaternion.identity);

            }
        }
        center[0] = newC0;

        //display the center points----------------
        result="";
        for (int i = 0; i < 10; i++) {     
            result+=center[i];
        }
        //Debug.Log(result);
        //-----------------------------------------
        edgePoints();
        
    }
    
    
    void norm(ref float[] norm,ref float[,] original)
    {
        for(int index=0;index<norm.GetLength(0);index++)
        {
            norm[index] = (float)Math.Sqrt(
                    Math.Pow(original[index,0],2)+
                    Math.Pow(original[index,1],2)+
                    Math.Pow(original[index,2],2));
        }

    }
    void divideAll(ref float[,] dividend,ref float[] divisor)
    {
        for(int index=0;index<dividend.GetLength(0);index++)
        {
            dividend[index,0] = dividend[index,0]/divisor[index];
            dividend[index,1] = dividend[index,1]/divisor[index];
            dividend[index,2] = dividend[index,2]/divisor[index];
        }
    }
    void edgePoints()
    {
        tan_norm = new float[Length-1];
        tan = new float[Length-1,3];
        e2_cal = new float[Length,3];
        e0_cal = new float[Length,3];
        e0_norm = new float[Length];
        wormShape = new float[Length,3];
        for(int index=0;index<Length;index++)
        {
            for(int axis=0;axis<3;axis++)
            {
                //Debug.Log(axis.ToString()+','+index.ToString());
                wormShape[index,axis] = center[index][axis];
                
            }
            if (index>0)
            {
                for(int axis=0;axis<3;axis++)
                {
                    tan[index-1,axis] = wormShape[index,axis] - wormShape[index-1,axis];
                }
            }
        }
        norm(ref tan_norm,ref tan);
        divideAll(ref tan, ref tan_norm);
        //find e0
        for(int index=0;index<Length;index++)
        {
            for(int axis=0;axis<3;axis++)
            {
                if(index==0)
                {
                    e0_cal[index,axis] = tan[index,axis];
                }
                else if(index==Length-1)
                {
                    e0_cal[index,axis] = tan[Length-2,axis];
                }
                else
                {
                    e0_cal[index,axis] = (tan[index,axis]+tan[index-1,axis])/2;
                }
            }
        }
        norm(ref e0_norm, ref e0_cal);
        divideAll(ref e0_cal, ref e0_norm);
        //initialise e2
        for(int index=0;index<Length;index++)
        {
            e2_cal[index,0] = 0;
            e2_cal[index,1] = 1;
            e2_cal[index,2] = 0;
        }
        for(int index=0;index<Length;index++)
        {
            for(int axis=0;axis<3;axis++)
            {
                e0[index][axis] = e0_cal[index,axis];
                e2[index][axis] = e2_cal[index,axis];
            }
        }
        //find e1
        for(int index=0;index<Length;index++)
        {
            e1[index] = Vector3.Cross(e2[index],e0[index]);
        }
        int theta=0;
        int circleCount = 0;
        for(int index=0;index<Length*10;index++)
        {
            Debug.Log(index.ToString()+","+circleCount.ToString());

            points[index] = center[circleCount] + (Radius*(((float)Math.Cos(Math.PI*(theta)/180)*e1[circleCount])+((float)Math.Sin(Math.PI*(theta)/180)*e2[circleCount])));
            theta+=36;
            if((index+1)%10==0){circleCount++;}
            if(theta>=360){
                theta=0;
            }
        }

        for(int index=0;index<Length*10;index++)
        {
            Instantiate(edge, points[index], Quaternion.identity);
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
    void CameraUpdate()
    {
        transform.position = center[0];
        transform.rotation = Quaternion.Euler(0, (-1*direction), 0);
    }
    // Update is called once per frame
    void Update()
    {
        
        directionIncrement(keyDetection());
        CameraUpdate();
        
        
        time+= Time.deltaTime;
        if(time-intervals>0.1)
        {
            //Debug.Log(direction);
            //mesh.Clear();
            centerLine(time);
            //mesh.vertices = points;
            //mesh.triangles = triangles;
            intervals=time;
        }
    }
}
