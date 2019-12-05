using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderBullet : MonoBehaviour
{

    public Vector2 startingForce;
    public float totalTime;
    float timer;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void startBullet()
    {
        totalTime = Random.Range(3, 10);
        GetComponent<Rigidbody2D>().velocity = startingForce;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > totalTime)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Equals("AttackPoint"))
        {
            Destroy(gameObject);
        }
    }

}
