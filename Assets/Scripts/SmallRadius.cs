using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallRadius : MonoBehaviour
{
    public bool player;
    public bool wanderpoint;
    public GameObject point;
    // Start is called before the first frame update
    void Start()
    {}

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.name.Equals("Point1"))
        {
            point = collision.gameObject;
            wanderpoint = true;
        } else if (collision.name.Equals("Point2"))
        {
            point = collision.gameObject;
            wanderpoint = true;
        } else if(collision.name.Equals("Point"))
        {
            wanderpoint = true;
            point = collision.gameObject;
        }
        if (collision.name.Equals("Player(Clone)"))
        {
            player = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name.Equals("Point1"))
        {
            wanderpoint = false;
        }
        else if (collision.name.Equals("Point2"))
        {
            wanderpoint = false;
        } else if (collision.name.Equals("Point"))
        {
            wanderpoint = false;
        }
        if (collision.name.Equals("Player(Clone)"))
        {
            player = false;
        }
    }
}
