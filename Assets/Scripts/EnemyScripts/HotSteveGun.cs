using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotSteveGun : MonoBehaviour
{
    public bool shoot;
    public GameObject projectilePrefab;
    GameObject bulletSpawn;
    GameObject bulletTarget;

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
        if (shoot && GameObject.Find("GameController").GetComponent<Game>().playing)
        {
            shootBullet();
        }
    }
    void shootBullet()
    {
        shoot = false;
        GameObject newBullet = Instantiate(projectilePrefab);
        newBullet.transform.position = GameObject.Find("BulletSpawn").transform.position;
        newBullet.GetComponent<HotSteveProjectile>().target = GameObject.Find("BulletTarget").transform.position;
    }
}
