using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
   public GameObject[] objectPrefabs;
    float timer = 0;
    float intervalTime = 1f;
    float overallTimer = 0;
    int currPattern = 1;
    int currXRange;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void updateSpawn()
    {
        overallTimer += Time.deltaTime;
        transform.position = new Vector3(currXRange * Mathf.Sin(overallTimer * currPattern), transform.position.y, 0);
        timer += Time.deltaTime;
        if (timer > intervalTime)
        {
            GameObject newObj = Instantiate(objectPrefabs[Random.Range(0, objectPrefabs.Length)]);
            newObj.AddComponent<Object>();
            newObj.transform.position = transform.position;
            newObj.GetComponent<Rigidbody2D>().AddTorque(Random.Range(-180, 180));
            newObj.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-10, 10), Random.Range(-5, 5));
            timer = 0;
            intervalTime = Random.Range(0.5f, 1f);
            GameObject.Find("MimosaWhooshSound").GetComponent<AudioSource>().Play();
        }
    }
    public void startSpawning()
    {
        Object[] objects = FindObjectsOfType(typeof(Object)) as Object[];
        foreach(Object o in objects)
        {
            Destroy(o.gameObject);
        }
        timer = 0;
        transform.position = new Vector3(0, transform.position.y, 0);
        currPattern = Random.Range(1, 11);
        currXRange = Random.Range(4, 11);
    }
}
