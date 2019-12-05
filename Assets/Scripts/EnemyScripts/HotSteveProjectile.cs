using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotSteveProjectile : MonoBehaviour
{
    public Vector3 target;
    float timer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(GameObject.Find("GameController").GetComponent<Game>().playing)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target, 0.1f);
            if (transform.position == target || timer > 2f)
            {
                Destroy(gameObject);
            }
        }
    }
}
