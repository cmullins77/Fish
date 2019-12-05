using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotSteve : Enemy
{
    List<GameObject> shootPoints;
    List<GameObject> groundPoints;
    List<GameObject> fleePoints;

    GameObject gunTurret;
    bool shot;

    private void Start()
    {
        shootPoints = new List<GameObject>();
        foreach(Transform child in GameObject.Find("ShootPoints").transform)
        {
            shootPoints.Add(child.gameObject);
        }
        groundPoints = new List<GameObject>();
        foreach(Transform child in GameObject.Find("GroundPoints").transform)
        {
            groundPoints.Add(child.gameObject);
        }
        fleePoints = new List<GameObject>();
        foreach(Transform child in GameObject.Find("FleePoints").transform)
        {
            fleePoints.Add(child.gameObject);
        }

        gunTurret = GameObject.Find("GunTurret");
        shot = false;
        gunTurret.GetComponent<HotSteveGun>().shotDone = false;

        rb = GetComponent<Rigidbody2D>();
        sR = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        state = Enemy.State.Evade;

        deathParticles.gameObject.SetActive(false);

        targetVector = transform.position;

        gameObject.name = "Enemy";
    }

    private void Update()
    {
        if (GameObject.Find("GameController").GetComponent<Game>().playing)
        {

            //If the enemy should be dying but the dying animation hasn't started, start dying animation
            if (health <= 0 && state != Enemy.State.Dying)
            {
                deathParticles.gameObject.SetActive(true);
                deathParticles.Play();
                gunTurret.SetActive(false);
                sR.enabled = false;
                state = Enemy.State.Dying;
                timer = 0;
            }
            float startX = transform.position.x;
            switch (state)
            {
                case (HotSteve.State.Idle):
                    handleIdle();
                    break;
                case (HotSteve.State.Charge):
                    handleCharge();
                    break;
                case (HotSteve.State.Shoot):
                    handleShoot();
                    break;
                case (HotSteve.State.Evade):
                    handleEvade();
                    break;
                case (HotSteve.State.FlyUp):
                    handleFlyUp();
                    break;
                case (HotSteve.State.FlyDown):
                    handleFlyDown();
                    break;
                case (HotSteve.State.Dying):
                    //If the enemy is dying then nothing else occurs
                    timer += Time.deltaTime;
                    if (timer > 1f)
                    {
                        Destroy(gameObject);
                    }
                    break;
            }
            if (state != Enemy.State.Dying)
            {
                if (transform.position.x < startX && facingRight)
                {
                    flip();
                }
                else if (transform.position.x > startX && !facingRight)
                {
                    flip();
                }

            }
        }
    }
    void handleIdle()
    {
        int num = Random.Range(0, 3);
        if (num == 0)
        {
            state = Enemy.State.FlyUp;
            targetVector = shootPoints[Random.Range(0, shootPoints.Count)].transform.position;
        }
        else
        {
            state = Enemy.State.Charge;
            //TODO: Decouple
            targetVector = GameObject.Find("Player(Clone)").transform.position;
        }
    }
    void handleCharge()
    {
        timer += Time.deltaTime;
        if (timer < 0.2f)
        {
            //TODO: Decouple
            targetVector = GameObject.Find("Player(Clone)").transform.position;
        }
        transform.position = Vector3.MoveTowards(transform.position, targetVector, 0.2f);
        if(transform.position == targetVector)
        {
            state = Enemy.State.Evade;
            timer = 0;
            anim.SetBool("Charging", false);
        }
    }
    void handleShoot()
    {
        rb.velocity = new Vector2(0, 0);
        if(!shot)
        {
            int num2 = Random.Range(1, 6);
            gunTurret = GameObject.Find("GunTurret");
            gunTurret.GetComponent<Animator>().SetInteger("Num", num2);
            gunTurret.GetComponent<Animator>().SetTrigger("Shoot");
            shot = true;
            gunTurret.GetComponent<HotSteveGun>().shotDone = false;
            if(facingRight && transform.position.x > 0)
            {
                flip();
            } else if (!facingRight && transform.position.x < 0)
            {
                flip();
            }
        }
        if(gunTurret.GetComponent<HotSteveGun>().shotDone)
        {
            targetVector = groundPoints[Random.Range(0, groundPoints.Count)].transform.position;
            state = Enemy.State.FlyDown;
            timer = 0;

        }
    }
    void handleEvade()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetVector, 0.1f);
        //TODO: Decouple
        Vector3 distanceVector = GameObject.Find("Player(Clone)").transform.position - targetVector;
        float fishDistance = (targetVector - transform.position).magnitude;
        float PlayerDistance = distanceVector.magnitude;
        if(PlayerDistance < 2 && fishDistance < 0.1f)
        {
            targetVector = fleePoints[Random.Range(0, fleePoints.Count)].transform.position;
        }
        timer += Time.deltaTime;
        if (timer > waitTime)
        {
            int num3 = Random.Range(0, 3);
            if (num3 == 0)
            {
                state = Enemy.State.FlyUp;
                targetVector = shootPoints[Random.Range(0, shootPoints.Count)].transform.position;
            }
            else
            {
                state = Enemy.State.Charge;
                targetVector = GameObject.Find("Player(Clone)").transform.position;
                timer = 0;
                anim.SetBool("Charging", true);
                GameObject.Find("EnemyPunchSound").GetComponent<AudioSource>().Play();
            }
        }
    }
    void handleFlyUp()
    {

        transform.position = Vector3.MoveTowards(transform.position, targetVector, 0.2f);
        if(transform.position == targetVector)
        {
            state = Enemy.State.Shoot;
            shot = false;
            if (gunTurret != null) {
                gunTurret.GetComponent<HotSteveGun>().shotDone = false;
            }
        }
    }
    void handleFlyDown()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetVector, 0.2f);
        if (transform.position == targetVector)
        {
            targetVector = fleePoints[Random.Range(0, fleePoints.Count)].transform.position;
            state = Enemy.State.Evade;
        }
    }

    void flip()
    {
        facingRight = !facingRight;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.name.Equals("AttackPoint"))
        {
            health = health - 2;
            waitTime -= 0.05f;
            //TODO:Decouple
            GameObject.Find("GameController").GetComponent<Game>().setEnemyHealth(health);
            timer = 0.5f;
            if(state != Enemy.State.Shoot)
            {
                targetVector = groundPoints[Random.Range(0, groundPoints.Count)].transform.position;
                state = Enemy.State.FlyDown;
            }
            anim.SetBool("Charging", false); 
            anim.Play("Hit");
            GameObject.Find("EnemyHitSound").GetComponent<AudioSource>().Play();
            hitParticles.gameObject.transform.position = collision.transform.position;
            hitParticles.Play("HotSteveHit");
        }
    }


}
