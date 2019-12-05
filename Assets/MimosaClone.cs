using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimosaClone : Enemy
{
    //TODO: Add any Lists of Points or Specific Points within Screen that Are used for Attacks or Other Behavior
    List<GameObject> fleePoints;
    List<GameObject> shootPoints;

    SplineFollower sF;


    float hitTimer;
    bool moving = false;

    public GameObject attackPoint;
    bool flipStart = false;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.name = "Clone";
        sF = GetComponent<SplineFollower>();
        sR = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        attackPoint.SetActive(false);

        //TODO: Find any of the Defined Points or List of Points
        fleePoints = new List<GameObject>();
        foreach (Transform child in GameObject.Find("FleePoints").transform)
        {
            fleePoints.Add(child.gameObject);
        }

        shootPoints = new List<GameObject>();
        foreach (Transform child in GameObject.Find("ShootPoints").transform)
        {
            shootPoints.Add(child.gameObject);
        }

        state = Enemy.State.Evade;
        deathParticles.gameObject.SetActive(false);

        targetVector = transform.position;


        facingRight = false;

        //TODO: Modify Value as Starting Time between attacks
        waitTime = 3f;

        //Change to FishName
        fishName = Enemy.FishName.Mimosa;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.Find("GameController").GetComponent<Game>().playing)
        {
            hitTimer -= Time.deltaTime;
            if (health <= 0 && state != Enemy.State.Dying)
            {
                deathParticles.gameObject.SetActive(true);
                deathParticles.Play();
                sR.enabled = false;
                state = Enemy.State.Dying;
                timer = 0;
            }
            float startX = transform.position.x;
            float startY = transform.position.y;

            //TODO: Add any new Enemy States to State Enum in Enemy Script
            //Then add these cases and handle functions
            switch (state)
            {
                case (Enemy.State.Idle):
                    handleIdle();
                    break;
                case (Enemy.State.Evade):
                    handleEvade();
                    break;
                case (Enemy.State.Spline):
                    handleSpline();
                    break;
                case (Enemy.State.Charge):
                    handleCharge();
                    break;
                case (Enemy.State.Shoot):
                    handleShoot();
                    break;
                case (Enemy.State.FlyUp):
                    handleFlyUp();
                    break;
                case (Enemy.State.FlyDown):
                    handleFlyDown();
                    break;
                case (Enemy.State.Dying):
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
                if (state != Enemy.State.Shoot && state != Enemy.State.FlyDown)
                {
                    if (moving && Mathf.Abs(transform.position.x - startX) < 0.05f && Mathf.Abs(transform.position.y - startY) < 0.05f)
                    {
                        anim.Play("Idle");
                        moving = false;
                    }
                    else if (!moving && (Mathf.Abs(transform.position.x - startX) >= 0.05f || Mathf.Abs(transform.position.y - startY) >= 0.05f))
                    {
                        anim.Play("Move");
                        moving = true;
                    }
                }
                if (transform.position.x < startX && facingRight)
                {
                    facingRight = false;
                    transform.localScale = new Vector3(0.24013f, 0.24013f, 0);
                }
                else if (transform.position.x > startX && !facingRight)
                {
                    facingRight = true;
                    transform.localScale = new Vector3(-0.24013f, 0.240132f, 0);
                }

            }
        }
    }

    void handleIdle()
    {
        if (GameObject.Find("GameController").GetComponent<Game>().playing)
        {
            state = Enemy.State.Evade;
            targetVector = fleePoints[Random.Range(0, fleePoints.Count)].transform.position;
        }
    }

    void handleSpline()
    {
        if (sF.done)
        {
            targetVector = fleePoints[Random.Range(0, fleePoints.Count)].transform.position;
            state = Enemy.State.Evade;
            timer = 0;
            attackPoint.SetActive(false);
        }
        else
        {
            sF.updateSpline();
        }
    }

    void handleCharge()
    {
        timer += Time.deltaTime;
        if (timer < -1f)
        {
            targetVector = GameObject.Find("Player(Clone)").transform.position;
            Vector3 distanceVect = targetVector - transform.position;
            targetVector = targetVector + distanceVect * 3;
        }
        transform.position = Vector3.MoveTowards(transform.position, targetVector, 0.35f);
        if (transform.position == targetVector)
        {
            state = Enemy.State.Evade;
            timer = 0;
            attackPoint.SetActive(false);
            targetVector = fleePoints[Random.Range(0, fleePoints.Count)].transform.position;
        }
    }

    void handleShoot()
    {
        if (transform.position.x < 0 && !facingRight)
        {
            facingRight = true;
            transform.localScale = new Vector3(-0.24013f, 0.24013f, 0);
        }
        else if (transform.position.x > 0 && facingRight)
        {
            facingRight = false;
            transform.localScale = new Vector3(0.24013f, 0.240132f, 0);
        }
        GameObject.Find("ObjectSpawn").GetComponent<ObjectSpawner>().updateSpawn();
        timer += Time.deltaTime;
        if (timer > 5f)
        {
            timer = 0;
            targetVector = fleePoints[Random.Range(0, fleePoints.Count)].transform.position;
            state = Enemy.State.Evade;
            anim.Play("Idle");
            moving = false;
        }
    }
    void handleFlyUp()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetVector, 0.2f);
        if (transform.position == targetVector)
        {
            state = Enemy.State.Shoot;
            anim.Play("Summon");
            timer = 0;
        }
    }

    void handleFlyDown()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetVector, 0.2f);
        if (transform.position == targetVector && !flipStart)
        {
            anim.Play("Flip");
            flipStart = true;
            timer = 0;
            GameObject.Find("MimosaWhooshSound").GetComponent<AudioSource>().Play();
        }
        if (flipStart)
        {
            timer += Time.deltaTime;
            if (timer > 0.8f)
            {
                timer = 0;
                state = Enemy.State.FlyUp;
                targetVector = shootPoints[Random.Range(0, shootPoints.Count)].transform.position;
                GameObject.Find("ObjectSpawn").GetComponent<ObjectSpawner>().startSpawning();
            }
        }
    }


    void handleEvade()
    {
        //TODO: Adjust float value to change speed of enemy while moving during evade
        transform.position = Vector3.MoveTowards(transform.position, targetVector, 0.2f);

        Vector3 distanceVector = GameObject.Find("Player(Clone)").transform.position - targetVector;
        float fishDistance = (targetVector - transform.position).magnitude;
        float PlayerDistance = distanceVector.magnitude;

        //Adjust the 7 to change how close the player must be to the enemy to trigger fleeing during evade
        if (PlayerDistance < 4 && fishDistance < 0.1f)
        {
            targetVector = fleePoints[Random.Range(0, fleePoints.Count)].transform.position;
        }
        timer += Time.deltaTime;
        if (timer > waitTime)
        {
            //TODO: Adjust random range values to change probability of an attack occuring
            int num3 = Random.Range(0, 3);

            //TODO: Here are some examples, change as required for your attacks
            if (num3 == 0)
            {
                sF.done = false;
                timer = 0;
                state = Enemy.State.Spline;
                sF.startSpline();
                attackPoint.SetActive(true);

            }
            else if (num3 == 1)
            {
                state = Enemy.State.Charge;
                targetVector = GameObject.Find("Player(Clone)").transform.position;
                Vector3 distanceVect = targetVector - transform.position;
                targetVector = targetVector + distanceVect * 1.5f;
                timer = 0;
                GameObject.Find("EnemyPunchSound").GetComponent<AudioSource>().Play();
                attackPoint.SetActive(true);
            }
            else
            {
                timer = 0;
                state = Enemy.State.FlyDown;
                flipStart = false;
                targetVector = new Vector3(transform.position.x, GameObject.Find("MimosaFlipSpot").transform.position.y, 0);
            }
        }
    }

    void flip()
    {
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hitTimer <= 0)
        {
            if (collision.name.Equals("AttackPoint"))
            {
                //TODO: Adjust Value to change how long player is invulnerable after successfully hitting the enemy (can be 0)
                collision.transform.parent.gameObject.GetComponent<Player>().invlunerableTimer = 0.5f;

                //TODO: Some states/attacks may require their own special results for an attack so if that's the case put those here, and if not remove the if statement and always run the code
                if (state != Enemy.State.Bounce && state != Enemy.State.Explode)
                {
                    //TODO: Adjust health value to balance enemy
                    health = health - 4;

                    //TODO: Adjust value of waittime change, basically attack frequency increases as waittime decreases
                    waitTime -= 0.005f;


                    //TODO: If any special indication that enemy was hit, change that here
                    sR.color = new Color(1, 1, 1);

                    if (state == Enemy.State.Charge)
                    {
                        targetVector = fleePoints[Random.Range(0, fleePoints.Count)].transform.position;
                        state = Enemy.State.Evade;
                    }


                    GameObject.Find("EnemyHitSound").GetComponent<AudioSource>().Play();
                    hitParticles.gameObject.transform.position = GameObject.Find("AttackPoint").transform.position;
                    hitParticles.Play("HotSteveHit");

                    //TODO: Adjust invulnerability time after getting hit
                    hitTimer = 0.6f;
                    anim.Play("Hit");
                    moving = false;


                }
                else
                {
                    //TODO: Specify anything for Special Cases
                }
            }
        }
    }
}
