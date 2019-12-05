using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineFollower : MonoBehaviour
{
    public List<Vector3> pointList;
    public Vector2 maxVals;
    public Vector2 minVals;
    public float timer = 0;
    public int currStartingIndex = 0;
    float[] M1 = { -1/6.0f, 3 / 6.0f, -3 / 6.0f, 1 / 6.0f };
    float[] M2 = { 3 / 6.0f, -6 / 6.0f, 3 / 6.0f, 0 };
    float[] M3 = { -3 / 6.0f, 0, 3 / 6.0f, 0 };
    float[] M4 = { 1 / 6.0f, 4 / 6.0f, 1 / 6.0f, 0 };
    public float[][] M;
    public float[] timeCheck;
    public Vector3[] GCheck;
    public float[] tempCheck;
    Vector3 prevPos;

    public bool done = false;


    // Start is called before the first frame update
    void Start()
    {
       
    }

    public void startSpline()
    {
        timer = 0;
        currStartingIndex = 0;
        done = false;
        pointList = new List<Vector3>();
        int numPoints = Random.Range(5, 15);
        pointList.Add(transform.position);
        pointList.Add(transform.position);
        pointList.Add(transform.position);
        for (int i = 3; i < numPoints; i++)
        {
            pointList.Add(new Vector3(Random.Range(minVals.x, maxVals.x + 0.1f), Random.Range(minVals.y, maxVals.y + 0.1f), 0));
            int num = Random.Range(0, 3);
            if(num == 0)
            {
                pointList.Add(GameObject.Find("Player(Clone)").transform.position);
            }
        }
        pointList.Add(transform.position);
        pointList.Add(transform.position);
        pointList.Add(transform.position);

        float[][] temp = { M1, M2, M3, M4 };
        M = temp;
        prevPos = transform.position;
}

    public void updateSpline()
    {
        Vector3 currPos = transform.position;
        timer += 0.025f;
        if (currStartingIndex + 1 <= timer)
        {
            currStartingIndex++;
        }
        if (currStartingIndex + 3 < pointList.Count)
        {
            float currTime = timer - currStartingIndex;
            float[] Time = { currTime * currTime * currTime, currTime * currTime, currTime, 1 };
            timeCheck = Time;
            Vector3[] G = { pointList[currStartingIndex], pointList[currStartingIndex + 1], pointList[currStartingIndex + 2], pointList[currStartingIndex + 3] };
            GCheck = G;
            float[] temp = { Time[0] * M[0][0] + Time[1] * M[1][0] + Time[2] * M[2][0] + Time[3] * M[3][0],
            Time[0] * M[0][1] + Time[1] * M[1][1] + Time[2] * M[2][1] + Time[3] * M[3][1],
            Time[0] * M[0][2] + Time[1] * M[1][2] + Time[2] * M[2][2] + Time[3] * M[3][2],
            Time[0] * M[0][3] + Time[1] * M[1][3] + Time[2] * M[2][3] + Time[3] * M[3][3] };
            tempCheck = temp;
            Vector3 Final = new Vector3(temp[0] * G[0].x + temp[1] * G[1].x + temp[2] * G[2].x + temp[3] * G[3].x,
                temp[0] * G[0].y + temp[1] * G[1].y + temp[2] * G[2].y + temp[3] * G[3].y,
                temp[0] * G[0].z + temp[1] * G[1].z + temp[2] * G[2].z + temp[3] * G[3].z);
            transform.position = Final;
        }
        else
        {
            done = true;
        }
        prevPos = currPos;
    }
}
