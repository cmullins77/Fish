using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireballs : MonoBehaviour
{
    public int layerNum = 10;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.layer != layerNum)
        {
            gameObject.layer = layerNum;
            foreach(Transform bigT in transform)
            {
                bigT.gameObject.layer = layerNum;
                foreach (Transform medT in bigT)
                {
                    medT.gameObject.layer = layerNum;
                    foreach (Transform littleT in medT)
                    {
                        littleT.gameObject.layer = layerNum;
                        foreach(Transform miniT in littleT)
                        {
                            miniT.gameObject.layer = layerNum;
                        }
                    }
                }
            }
        }
    }
}
