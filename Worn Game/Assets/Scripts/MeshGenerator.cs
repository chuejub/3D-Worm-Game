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
    public GameObject cam;
    public GameObject head;
    public GameObject tail;
    public float Radius;
    public float speed;
    public float turnSpeed;
    public int Length;
    public float offsetDistance;
    Vector2[] texture;
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
    Vector3 offset;
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
                

            }
        }
        center[0] = newC0;

        if(center[Length-1]!=new Vector3 (0,0,0))
        {
            edgePoints();
        }
        
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
            points[index] = center[circleCount] + (Radius*(((float)Math.Cos(Math.PI*(theta)/180)*e1[circleCount])+((float)Math.Sin(Math.PI*(theta)/180)*e2[circleCount])));
            theta+=36;
            if((index+1)%10==0){circleCount++;}
            if(theta>=360){
                theta=0;
            }
        }

    }
    void CreateShape()
    {
        triangles = new int[(Length-1)*60];
        int a=0;

        for(int outindex=0;outindex<(Length-1)*60;outindex+=60)
        {
            for(int index=0;index<30;index+=3)
            {
                triangles[outindex+index] = a;
                if(index==27){triangles[outindex+index+1] = a-9;}
                else{triangles[outindex+index+1] = a+1;}
                triangles[outindex+index+2] = a+10;
                a++;
            }
            for(int index=30;index<60;index+=3)
            {
                if(index==57){triangles[outindex+index] = a-9;Debug.Log(a-9);}
                else{triangles[outindex+index] = a+1;}
                triangles[outindex+index+1] = a;
                if(index==57){triangles[outindex+index+2] = a-19;}
                else{triangles[outindex+index+2] = a-9;}
                a++;
            }
            a-=10;
        }
        for(int index=0;index<(2-1)*60;index+=3)
            Debug.Log(triangles[index].ToString()+","+triangles[index+1].ToString()+","+triangles[index+2].ToString());
    }

    void CameraUpdate()
    {
        offset = new Vector3(-offsetDistance*(float)Math.Cos(Math.PI*(direction)/180)
                            ,10
                            ,-offsetDistance*(float)Math.Sin(Math.PI*(direction)/180));
        head.transform.position = center[0];
        cam.transform.position = center[0]+offset;
        //Debug.Log();
        cam.transform.rotation = Quaternion.Euler(20, (-1*direction)+90, 0);
    }
    // Update is called once per frame
    void Update()
    {
        
        directionIncrement(keyDetection());
        CameraUpdate();
        
        
        time+= Time.deltaTime;
        if(time-intervals>0.1)
        {
            
            centerLine(time);
            if(center[Length-1]!=new Vector3 (0,0,0))
            {
                tail.transform.position = center[Length-1];
                // Instantiate(line, center[0], Quaternion.identity);
                // for(int index=0;index<10; index++)
                // {
                //     Instantiate(edge, points[index], Quaternion.identity);
                // }
                mesh.Clear();
                mesh.vertices = points;
                mesh.triangles = triangles;
                
            }
            intervals=time;
            
        }
    }
}
