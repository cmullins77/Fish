//To Create New Enemy Scene:
//Create New Scene
//Delete Everything in Scene (Yes, including the camera)
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
public class Bijou : Enemy
{
    //TODO: Add any Lists of Points or Specific Points within Screen that Are used for Attacks or Other Behavior
    List<GameObject> fleePoints;
    List<GameObject> mimicPoints;

    public GameObject mimicPrefab;
    float hitTimer;
    float currentSpeed;
    public int totalFirePatterns = 5;
    int currentFirePattern;
    bool fireStarted = false;
    float fireTime = 10;

    // Start is called before the first frame update
    void Start()
    {
        sR = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        fleePoints = new List<GameObject>();
        foreach (Transform child in GameObject.Find("FleePoints").transform)
        {
            fleePoints.Add(child.gameObject);
        }
        mimicPoints = new List<GameObject>();
        foreach(Transform child in GameObject.Find("MimicPoints").transform)
        {
            mimicPoints.Add(child.gameObject);
        }

        state = Enemy.State.Evade;
        deathParticles.gameObject.SetActive(false);

        targetVector = transform.position;

        gameObject.name = "Enemy";

        facingRight = false;

        waitTime = 2f;

        fishName = Enemy.FishName.Bijou;
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
                for(int i = 1; i < totalFirePatterns + 1; i++)
                {
                    GameObject.Find("FireBalls" + i).transform.position = new Vector3(0, 1000, 0);
                }
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
                case (Enemy.State.Duplicate):
                    handleDuplicate();
                    break;
                case (Enemy.State.Fire):
                    handleFire();
                    break;
                case (Enemy.State.Invisible):
                    handleInvisible();
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

    void handleDuplicate()
    {
        timer += Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetVector, currentSpeed);
        if (targetVector == transform.position)
        {
            targetVector = new Vector3(mimicPoints[Random.Range(0, mimicPoints.Count)].transform.position.x, 3.43f, 0);
            currentSpeed = Random.Range(0.02f, 0.3f);
        }
        if((FindObjectsOfType(typeof(BijouMimic)) as BijouMimic[]).Length == 0)
        {
            timer = -2f;
            state = Enemy.State.Evade;
            targetVector = fleePoints[Random.Range(0, fleePoints.Count)].transform.position;

        }
        if (timer > 10)
        {
            timer = 0;
            state = Enemy.State.Evade;
            targetVector = fleePoints[Random.Range(0, fleePoints.Count)].transform.position;
            BijouMimic[] mimics = FindObjectsOfType(typeof(BijouMimic)) as BijouMimic[];
            foreach(BijouMimic b in mimics)
            {
                b.health = -100;
            }
        }
    }

    void handleFire()
    {
        if(fireStarted)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetVector, 0.2f);
            timer += Time.deltaTime;
            if (timer > fireTime)
            {
                timer = 0;
                state = Enemy.State.Evade;
                targetVector = fleePoints[Random.Range(0, fleePoints.Count)].transform.position;
                GameObject.Find("FireBalls" + currentFirePattern).transform.position = new Vector3(0, 1000, 0);
                GameObject.Find("FireBalls" + currentFirePattern).GetComponent<Animator>().Play("Fire" + currentFirePattern + "Idle");
                GameObject.Find("Fireballs").GetComponent<Animator>().Play("FadeOut");
            }
        }
        else
        {
            if(GameObject.Find("Fireballs").GetComponent<Fireballs>().layerNum == 0)
            {
                fireStarted = true;
            }
        }
    }

    void handleInvisible()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetVector, currentSpeed);
        if (targetVector == transform.position)
        {
            targetVector = fleePoints[Random.Range(0, fleePoints.Count)].transform.position;
            currentSpeed = Random.Range(.05f, .25f);
        }
        timer += Time.deltaTime;
        if (timer > 0.4f)
        {
            GameObject.Find("Healing").GetComponent<Animator>().Play("Healing");
            health = health + 1;
            if(health > 100)
            {
                health = 100;
            }
            GameObject.Find("GameController").GetComponent<Game>().setEnemyHealth(health);
            timer = 0;
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
            int num3 = Random.Range(0, 3);

            //TODO: Here are some examples, change as required for your attacks
            if (num3 == 0)
            {
                timer = 0;
                state = Enemy.State.Duplicate;
                int num = Random.Range(1, 4);
                for (int i = 0; i < num; i++)
                {
                    GameObject newMimic = Instantiate(mimicPrefab);
                    newMimic.transform.position = transform.position;
                }
                targetVector = mimicPoints[Random.Range(0, mimicPoints.Count)].transform.position;
                targetVector = new Vector3(targetVector.x, 3.43f, 0);
                currentSpeed = Random.Range(0.02f, 0.3f);
                GameObject.Find("BijouDuplicate").GetComponent<AudioSource>().Play();
                //anim.Play("Attack1");

            }
            else if (num3 == 1)
            {
                fireStarted = false;
                GameObject.Find("Fireballs").GetComponent<Animator>().Play("FadeIn");
                timer = 0;
                state = Enemy.State.Fire;
                currentFirePattern = Random.Range(1, totalFirePatterns + 1);
                print(currentFirePattern);
                GameObject.Find("FireBalls" + currentFirePattern).transform.position = new Vector3(0, 0, 0);
                GameObject.Find("FireBalls" + currentFirePattern).GetComponent<Animator>().Play("FirePattern" + currentFirePattern);
                targetVector = transform.position;
                fireTime = Random.Range(6.0f, 10.0f);
                GameObject.Find("BijouFire").GetComponent<AudioSource>().Play();
            }
            else
            {
                timer = 0;
                state = Enemy.State.Invisible;
                targetVector = targetVector = fleePoints[Random.Range(0, fleePoints.Count)].transform.position;
                anim.Play("Invisible");
                currentSpeed = Random.Range(0.1f, 0.4f);
                hitTimer = 1f;
                GameObject.Find("BijouInvisible").GetComponent<AudioSource>().Play();
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
                if (state != Enemy.State.Bounce && state != Enemy.State.Explode && state != Enemy.State.Duplicate)
                {
                    //TODO: Adjust health value to balance enemy
                    health = health - 6;

                    //TODO: Adjust value of waittime change, basically attack frequency increases as waittime decreases



                    GameObject.Find("GameController").GetComponent<Game>().setEnemyHealth(health);



                    //TODO: If any special indication that enemy was hit, change that here
                    sR.color = new Color(1, 1, 1);
                    if(state == Enemy.State.Invisible)
                    {
                        timer = 0;
                        state = Enemy.State.Evade;
                    }else if (state != Enemy.State.Fire)
                    {
                        targetVector = fleePoints[Random.Range(0, fleePoints.Count)].transform.position;
                        state = Enemy.State.Evade;

                        timer = 0.5f;
                    }
                    else
                    {
                        targetVector = fleePoints[Random.Range(0, fleePoints.Count)].transform.position;
                    }
                    anim.Play("Hit");

                    GameObject.Find("EnemyHitSound").GetComponent<AudioSource>().Play();
                    hitParticles.gameObject.transform.position = GameObject.Find("AttackPoint").transform.position;
                    hitParticles.Play("HotSteveHit");

                    //TODO: Adjust invulnerability time after getting hit
                    hitTimer = 0.6f;


                }
                else
                {
                    //TODO: Specify anything for Special Cases
                }
            }
        }
    }
}
