using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegMissile : MonoBehaviour
{
    public Vector3 target;
    float timer;
    float speed;
    // Start is called before the first frame update
    void Start()
    {
        name = "HotSteveProjectile(Clone)";
        speed = Random.Range(0.08f, 0.3f);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.Find("GameController").GetComponent<Game>().playing)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target, speed); 
            if (transform.position == target)
            {
                Destroy(gameObject);
            }
        }
    }


}
