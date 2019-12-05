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
using UnityEngine.UI;

//TODO: Rename from EnemyTemplate to your Enemy's Name
public class Hamburger : Enemy
{
    //TODO: Add any Lists of Points or Specific Points within Screen that Are used for Attacks or Other Behavior
    List<GameObject> fleePoints;


    float hitTimer;
    public GameObject[] clonePrefabs;
    GameObject enemyClone = null;

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
        waitTime = 3f;

        //Change to FishName
        fishName = Enemy.FishName.Hamburger;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.Find("GameController").GetComponent<Game>().playing)
        {
            hitTimer -= Time.deltaTime;
            if (health <= 0 && state != Enemy.State.Dying)
            {
                //deathParticles.gameObject.SetActive(true);
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
                case (Enemy.State.Strangle):
                    handleStrangle();
                    break;
                case (Enemy.State.Summon):
                    handleSummon();
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

    void handleStrangle()
    {
        timer += Time.deltaTime;
        if (timer > 0.4f)
        {
                GameObject.Find("Player(Clone)").GetComponent<Player>().health--;
                GameObject.Find("GameController").GetComponent<Game>().setPlayerHealth(GameObject.Find("Player(Clone)").GetComponent<Player>().health);
                timer = 0;
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

    void handleSummon()
    {
        if(GameObject.Find("Clone") == null)
        {
            timer = 0;
            state = Enemy.State.Evade;
            anim.Play("Idle");
            GameObject.Find("ChampionText").GetComponent<Text>().color = new Color(255, 255, 255, 0);
        }
    }

    void handleFlyDown()
    {
        GameObject.Find("StrangleText").GetComponent<Text>().color = new Color(255, 255, 255, 0);
        timer += Time.deltaTime;
        if(timer > 1f)
        {
            timer = 0;
            state = Enemy.State.Evade;
        }
    }

    void handleEvade()
    {
        timer += Time.deltaTime;
        if (timer > waitTime)
        {
            //TODO: Adjust random range values to change probability of an attack occuring
            int num3 = Random.Range(0, 3);

            //TODO: Here are some examples, change as required for your attacks
            if (num3 ==0)
            {
                timer = 0;
                state = Enemy.State.Strangle;
                GameObject.Find("StrangleText").GetComponent<Text>().color = new Color(255, 255, 255, 255);
                anim.Play("StrangleStrike");
                GameObject.Find("Player(Clone)").GetComponent<Player>().beingStrangled = true;
                GameObject.Find("Player(Clone)").GetComponent<Animator>().Play("Strangle");
                GameObject.Find("Player(Clone)").GetComponent<Player>().breakingFree = false;
                GameObject.Find("Player(Clone)").GetComponent<Player>().breakFreeTimer = 0;
                GameObject.Find("Player(Clone)").GetComponent<Player>().freeTimer = 0;
                GameObject.Find("EnemyKickSound").GetComponent<AudioSource>().Play();
            }
            else if (num3 == 1)
            {
                timer = -1;
                anim.Play("Strike");
                GameObject.Find("EnemyHitSound").GetComponent<AudioSource>().Play();
                //state = Enemy.State.Attack2;
                //anim.Play("Attack2");
            }
            else
            {
                timer = 0;
                state = Enemy.State.Summon;
                GameObject.Find("ChampionText").GetComponent<Text>().color = new Color(255, 255, 255, 1);
                GameObject newE = Instantiate(clonePrefabs[Random.Range(0, clonePrefabs.Length)]);
                newE.transform.position = new Vector3(0, 0, 0);
                enemyClone = newE;
                enemyClone.GetComponent<Enemy>().health = 10;
                anim.Play("Summon");
                GameObject.Find("SummonSound").GetComponent<AudioSource>().Play();
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
        print(collision.name);
        if (hitTimer <= 0)
        {
            if (collision.name.Equals("AttackPoint"))
            {
                //TODO: Adjust Value to change how long player is invulnerable after successfully hitting the enemy (can be 0)
                collision.transform.parent.gameObject.GetComponent<Player>().invlunerableTimer = 0.5f;

                //TODO: Some states/attacks may require their own special results for an attack so if that's the case put those here, and if not remove the if statement and always run the code
                if (state == Enemy.State.Evade || state == Enemy.State.FlyDown)
                {
                    //TODO: Adjust health value to balance enemy
                    health = health - 4;

                    //TODO: Adjust value of waittime change, basically attack frequency increases as waittime decreases
       


                    GameObject.Find("GameController").GetComponent<Game>().setEnemyHealth(health);


                    //TODO: If any special indication that enemy was hit, change that here
                    sR.color = new Color(1, 1, 1);



                    targetVector = fleePoints[Random.Range(0, fleePoints.Count)].transform.position;

                    //TODO: Adjust invulnerability time after getting hit
                    hitTimer = 0.2f;


                    //TODO: Could have special hurt animation
                    if(state != Enemy.State.FlyDown)
                    {
                        anim.Play("Hit");
                    }
                    GameObject.Find("EnemyHitSound").GetComponent<AudioSource>().Play();
                    hitParticles.gameObject.transform.position = GameObject.Find("AttackPoint").transform.position;
                    hitParticles.Play("HotSteveHit");
                }
                else
                {
                    //TODO: Specify anything for Special Cases
                }
            }
        }
    }
  
}
