//To Create New Enemy Scene:
//Create New Scene
//Delete Everything in Scene
//Save Scene Desired Name
//Drag in GameThings prefab
//Create Your New Enemy by copy and pasting this code and changing EnemyTemplate to the Enemy's name
//Prefab the New Enemy
//Under GameThings select the GameController object and drag the new Enemy Prefab into the EnemyPrefab box
//Add any points you may need for the enemy's code (or you can just use some of the ones already there)
//Under Canvas and Enemy Health modify the text to say the Enemy's Name
//Under Congratulations and GameOver modify name to use the enemy's name and also change the Blank Fish Image to a sprite of the Fishx
//Now Modify the Enemy's Code as Desired to Create the new Enemy - See TODOs
//Add Animations, Sound Effects, and any other Required Game Objects




using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Rename from EnemyTemplate to your Enemy's Name
public class EnemyTemplate : Enemy
{
    //TODO: Add any Lists of Points or Specific Points within Screen that Are used for Attacks or Other Behavior
    List<GameObject> fleePoints;


    float hitTimer;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sR = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        //TODO: Find any of the Defined Points or List of Points
        fleePoints = new List<GameObject>();
        foreach (Transform child in GameObject.Find("FleePoints").transform)
        {
            fleePoints.Add(child.gameObject);
        }

        state = Enemy.State.Evade;
        deathParticles.gameObject.SetActive(false);

        targetVector = transform.position;

        gameObject.name = "Enemy";

        facingRight = false;

        //TODO: Modify Value as Starting Time between attacks
        waitTime = 2f;

        //Change to FishName
        fishName = Enemy.FishName.HotSteve;
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


    void handleEvade()
    {
        //TODO: Adjust float value to change speed of enemy while moving during evade
        transform.position = Vector3.MoveTowards(transform.position, targetVector, 0.1f);

        Vector3 distanceVector = GameObject.Find("Player(Clone)").transform.position - targetVector;
        float fishDistance = (targetVector - transform.position).magnitude;
        float PlayerDistance = distanceVector.magnitude;

        //Adjust the 7 to change how close the player must be to the enemy to trigger fleeing during evade
        if (PlayerDistance < 7 && fishDistance < 0.1f)
        {
            targetVector = fleePoints[Random.Range(0, fleePoints.Count)].transform.position;
        }
        timer += Time.deltaTime;
        if (timer > waitTime)
        {
            //TODO: Adjust random range values to change probability of an attack occuring
            int num3 = Random.Range(0, 8);

            //TODO: Here are some examples, change as required for your attacks
            if (num3 < 2)
            {
                timer = 0;
                //state = Enemy.State.Attack1;
                //anim.Play("Attack1");

            }
            else if (num3 == 2)
            {
                timer = 0;
                //state = Enemy.State.Attack2;
                //anim.Play("Attack2");
            }
            else
            {
                timer = 0;
                //state = Enemy.State.Attack3;
                //anim.Play("Attack3");
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
                    waitTime -= 0.05f;


                    GameObject.Find("GameController").GetComponent<Game>().setEnemyHealth(health);


                    timer = 0.5f;

                    //TODO: If any special indication that enemy was hit, change that here
                    sR.color = new Color(1, 1, 1);


                    rb.velocity = new Vector2(0, 0);


                    targetVector = fleePoints[Random.Range(0, fleePoints.Count)].transform.position;
                    state = Enemy.State.Evade;


                    GameObject.Find("EnemyHitSound").GetComponent<AudioSource>().Play();
                    hitParticles.gameObject.transform.position = GameObject.Find("AttackPoint").transform.position;
                    hitParticles.Play("HotSteveHit");

                    //TODO: Adjust invulnerability time after getting hit
                    hitTimer = 0.6f;


                    //TODO: Could have special hurt animation
                    anim.Play("Idle");

                    //TODO: Adjust knockback
                    rb.AddForce((collision.transform.position - transform.position).normalized * 5);
                }
                else
                {
                    //TODO: Specify anything for Special Cases
                }
            }
        }
    }
}
