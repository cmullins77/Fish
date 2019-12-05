using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public int[] arr = { 3, 16, 5, 8, 99, 12, 16, 6, 23, -10, 9 };
    // Start is called before the first frame update
    void Start()
    {
        quick(arr);   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void quick(int[] theArr)
    {
        quick(theArr, 0, theArr.Length - 2, theArr.Length - 1);
    }
    void quick(int[] theArr, int startIndex, int endIndex, int pivotIndex)
    {
        print(theArr);
        print("Start " + startIndex + " End " + endIndex + " Piv " + pivotIndex);
        if(endIndex - 2 <= startIndex)
        {
            return;
        }
        int pivot = theArr[pivotIndex];
        while(startIndex <= endIndex + 1)
        {
            int start = theArr[startIndex];
            int end = theArr[endIndex];
            if(start < pivot && end > pivot)
            {
                int temp = start;
                start = end;
                end = temp;
                theArr[startIndex] = start;
                theArr[endIndex] = end;
                startIndex++;
                endIndex--;
            } else if (start < pivot)
            {
                endIndex--;
            } else if (end > pivot)
            {
                startIndex++;
            }
            else
            {
                endIndex--;
                startIndex++;
            }
        }
        theArr[pivotIndex] = theArr[endIndex];
        theArr[endIndex] = pivot;
        quick(theArr, 0, endIndex - 2, endIndex - 1);
        quick(theArr, endIndex + 1, theArr.Length - 2, theArr.Length - 1);
    }
}
