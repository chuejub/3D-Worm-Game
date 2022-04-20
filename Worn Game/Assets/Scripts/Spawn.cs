using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public int numberOfEnemy;
    public int numberOfReward;
    public GameObject enemy;
    public GameObject reward;
    int x, z, total;
    float[,] allPosition;
    Vector3[] v3Position;
    int unique =0;

    // Start is called before the first frame update
    void Start()
    {
        total = numberOfEnemy+numberOfReward;
        v3Position = new Vector3[total];
        allPosition = new float[total,3];
        //Random rand = new Random();
        for(int count=0;count<numberOfEnemy;count++)
        {
            while(unique==0)
            {
                x = Random.Range(-17, 26);
                z = Random.Range(-16, 20);
                unique = 1;
                for(int index=0;index<allPosition.GetLength(0);index++)
                {
                    if(allPosition[index,0]==x && allPosition[index,2]==z){unique = 0;}
                }
            }unique = 0;
            allPosition[count,0]=x;
            allPosition[count,1]=-0.2f;
            allPosition[count,2]=z;
        }
        for(int count=numberOfEnemy;count<total;count++)
        {
            while(unique==0)
            {
                x = Random.Range(-17, 26);
                z = Random.Range(-16, 20);
                unique = 1;
                for(int index=0;index<allPosition.GetLength(0);index++)
                {
                    if(allPosition[index,0]==x && allPosition[index,2]==z){unique = 0;}
                }
            }unique = 0;
            allPosition[count,0]=x;
            allPosition[count,1]=-0.1f;
            allPosition[count,2]=z;
        }

        for(int count=0;count<numberOfEnemy;count++)
        {
            v3Position[count][0]=allPosition[count,0]*10;
            v3Position[count][1]=allPosition[count,1]*10;
            v3Position[count][2]=allPosition[count,2]*10;
            Instantiate(enemy, v3Position[count], Quaternion.identity);
        }
        for(int count=numberOfEnemy;count<total;count++)
        {
            v3Position[count][0]=allPosition[count,0]*10;
            v3Position[count][1]=allPosition[count,1]*10;
            v3Position[count][2]=allPosition[count,2]*10;
            Instantiate(reward, v3Position[count], Quaternion.identity);
        }
        for(int count=0;count<total;count++)
        {
            Debug.Log(v3Position[count][0].ToString()+','+v3Position[count][2].ToString());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
