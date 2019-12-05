using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeggTurret : MonoBehaviour
{
    public bool shoot;
    public int prefabNum;
    public GameObject[] projectilePrefabs;
    GameObject bulletSpawn;
    GameObject bulletTarget;
    float timer = 0;


    public bool shotDone;
    // Start is called before the first frame update
    void Start()
    {
        shoot = false;
        bulletSpawn = GameObject.Find("BulletSpawn");
        bulletTarget = GameObject.Find("BulletTarget");
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (shoot && GameObject.Find("GameController").GetComponent<Game>().playing && timer > 0.1f)
        {
            shootBullet();
        }
    }
    void shootBullet()
    {
        GetComponent<AudioSource>().Play();
        shoot = false;
        timer = 0;
        GameObject newBullet = Instantiate(projectilePrefabs[prefabNum]);
        newBullet.transform.position = transform.position;
        newBullet.transform.rotation = transform.rotation;
        newBullet.GetComponent<LegMissile>().target = GameObject.Find("BulletTarget").transform.position;
    }
}
