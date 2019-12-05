//To Create New Enemy Scene:
//Add any points you may need for the enemy's code (or you can just use some of the ones already there)
//Under Canvas and Enemy Health modify the text to say the Enemy's Name
//Under Congratulations and GameOver modify name to use the enemy's name and also change the Blank Fish Image to a sprite of the Fishx
//Now Modify the Enemy's Code as Desired to Create the new Enemy - See TODOs
//Add Animations, Sound Effects, and any other Required Game Objects




using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Rename from EnemyTemplate to your Enemy's Name
public class Slider : Enemy
{
    //TODO: Add any Lists of Points or Specific Points within Screen that Are used for Attacks or Other Behavior
    List<GameObject> fleePoints;
    List<GameObject> shootPoints;

    public GameObject bulletPrefab;
    float hitTimer;
    float spikesTimer;

    bool spikeSound = false;

    // Start is called before the first frame update
    void Start()
    {
        sR = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        shootPoints = new List<GameObject>();
        foreach (Transform child in GameObject.Find("ShootPoints").transform)
        {
            shootPoints.Add(child.gameObject);
        }
        //TODO: Find any of the Defined Points or List of Points
        fleePoints = new List<GameObject>();
        foreach (Transform child in GameObject.Find("FleePoints").transform)
        {
            fleePoints.Add(child.gameObject);
        }

        state = Enemy.State.Evade;
        deathParticles.gameObject.SetActive(false);

        targetVector = fleePoints[Random.Range(0, fleePoints.Count)].transform.position;

        gameObject.name = "Enemy";

        facingRight = false;

        //TODO: Modify Value as Starting Time between attacks
        waitTime = 2f;

        //Change to FishName
        fishName = Enemy.FishName.Slider;
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
                GameObject.Find("Hat").GetComponent<SpriteRenderer>().enabled = false;
                state = Enemy.State.Dying;
                timer = 0;
            }
            float startX = transform.position.x;

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
                case (Enemy.State.FlyUp):
                    handleFlyUp();
                    break;
                case (Enemy.State.Throw):
                    handleThrow();
                    break;
                case (Enemy.State.Charge):
                    handleCharge();
                    break;
                case (Enemy.State.Shoot):
                    handleShoot();
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
            if (state != Enemy.State.Dying && state != Enemy.State.Bounce)
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
        if (GameObject.Find("GameController").GetComponent<Game>().playing)
        {
            state = Enemy.State.Evade;
            targetVector = fleePoints[Random.Range(0, fleePoints.Count)].transform.position;
        }
    }

    void handleThrow()
    {
        if(transform.position.x < 0 && !facingRight)
        {
            facingRight = true;
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, 0);
        } else if (transform.position.x > 0 && facingRight)
        {
            facingRight = false;
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, 0);
        }
        timer += Time.deltaTime;
        if(timer > 3f)
        {
            timer = 0;
            state = Enemy.State.Evade;
            anim.Play("Idle");
            targetVector = fleePoints[Random.Range(0, fleePoints.Count)].transform.position;
        }
    }

    void handleFlyUp()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetVector, 0.2f);
        if(transform.position == targetVector)
        {
            state = Enemy.State.Throw;
            anim.Play("Boomerang");
            GameObject.Find("EnemyKickSound").GetComponent<AudioSource>().Play();
            timer = 0;
        }
    }

    void handleShoot()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetVector, 0.3f);
        timer += Time.deltaTime;
        if(GameObject.Find("SliderBullet(Clone)") == null && timer > 2)
        {
            timer = 0;
            state = Enemy.State.Evade;
            anim.Play("Idle");
            targetVector = fleePoints[Random.Range(0, fleePoints.Count)].transform.position;
        }
    }

    void handleCharge()
    {
        timer += Time.deltaTime;
        if (timer < 0.2f)
        {
            Vector3 playerLoc = GameObject.Find("Player(Clone)").transform.position;
            Vector3 playerDir = playerLoc.normalized;
            float playerDistance = Mathf.Abs((transform.position - playerLoc).magnitude);
            float travelDistance = playerDistance - 1.5f;
            if(playerDistance < 1.5f)
            {
                travelDistance = 0;
            }
            targetVector = playerDir * travelDistance;

        }
        transform.position = Vector3.MoveTowards(transform.position, targetVector, 0.2f);
        if (transform.position == targetVector)
        {
            anim.Play("Spikes");
            spikesTimer += Time.deltaTime;
            if(!spikeSound)
            {
                GameObject.Find("SpikesSound").GetComponent<AudioSource>().Play();
                spikeSound = true;
            }
            if (spikesTimer > 0.4f)
            {
                timer = 0;
                state = Enemy.State.Evade;
                anim.Play("Idle");
                targetVector = fleePoints[Random.Range(0, fleePoints.Count)].transform.position;
            }
        }
    }

    void handleEvade()
    {
        //TODO: Adjust float value to change speed of enemy while moving during evade
        transform.position = Vector3.MoveTowards(transform.position, targetVector, 0.18f);

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
                timer = 0;
                int num = Random.Range(1, 3);
                targetVector = GameObject.Find("BoomerangPoint" + num).transform.position;
                state = Enemy.State.FlyUp;
            }
            else if (num3 == 1)
            {
                timer = 0;
                spikesTimer = 0;
                spikeSound = false;
                state = Enemy.State.Charge;
            }
            else
            {
                timer = 0;
                state = Enemy.State.Shoot;
                int num = Random.Range(1, 5);
                GameObject.Find("EnemyShootSound").GetComponent<AudioSource>().Play();
                for (int i = 0; i < num; i++)
                {
                    GameObject bullet = Instantiate(bulletPrefab);
                    bullet.transform.position = transform.position;
                    bullet.GetComponent<SliderBullet>().startingForce = new Vector2(Random.Range(-10, 10), Random.Range(-10, 10));
                    bullet.GetComponent<SliderBullet>().startBullet();
                    bullet.transform.parent = GameObject.Find("GameThings").transform;
                }
                targetVector = shootPoints[Random.Range(0, shootPoints.Count)].transform.position;
            }
        }
    }

    void flip()
    {
        facingRight = !facingRight;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
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
                    health = health - 5;

                    //TODO: Adjust value of waittime change, basically attack frequency increases as waittime decreases
                    //waitTime -= 0.05f;


                    GameObject.Find("GameController").GetComponent<Game>().setEnemyHealth(health);


                    timer -= 0.25f;

                    //TODO: If any special indication that enemy was hit, change that here
                    sR.color = new Color(1, 1, 1);

                    //TODO: Adjust invulnerability time after getting hit
                    hitTimer = 0.6f;

                    if (state == Enemy.State.Throw || state == Enemy.State.FlyUp)
                    {
                        hitTimer = 2f;
                    }
                    else
                    {
                        targetVector = fleePoints[Random.Range(0, fleePoints.Count)].transform.position;
                        state = Enemy.State.Evade;
                        anim.Play("Hit");
                    }


                    GameObject.Find("EnemyHitSound").GetComponent<AudioSource>().Play();
                    hitParticles.gameObject.transform.position = GameObject.Find("AttackPoint").transform.position;
                    hitParticles.Play("HotSteveHit");


                    //TODO: Could have special hurt animation


                }
                else
                {
                    //TODO: Specify anything for Special Cases
                }
            }
        }
    }
}
