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
public class BijouMimic : Enemy
{
    //TODO: Add any Lists of Points or Specific Points within Screen that Are used for Attacks or Other Behavior
    List<GameObject> fleePoints;


    float hitTimer;
    float currentSpeed = 0.2f;
    public GameObject mimicPrefab;

    // Start is called before the first frame update
    void Start()
    {
        deathParticles.gameObject.SetActive(false);
        sR = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        //TODO: Find any of the Defined Points or List of Points
        fleePoints = new List<GameObject>();
        foreach (Transform child in GameObject.Find("MimicPoints").transform)
        {
            fleePoints.Add(child.gameObject);
        }

        state = Enemy.State.Evade;

        targetVector = transform.position;


        facingRight = false;

        //TODO: Modify Value as Starting Time between attacks
        waitTime = 2f;

        //Change to FishName
        fishName = Enemy.FishName.Bijou;
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0 && state != Enemy.State.Dying)
        {
            GameObject.Find("EnemyShootSound").GetComponent<AudioSource>().Play();
            deathParticles.gameObject.SetActive(true);
            deathParticles.Play();
            sR.enabled = false;
            state = Enemy.State.Dying;
            timer = 0;
            foreach(Transform t in transform)
            {
                if(t.name.Equals("EnemyAttackPoint"))
                {
                    Destroy(t.gameObject);
                }
            }
        }
        if (GameObject.Find("GameController").GetComponent<Game>().playing)
        {
            hitTimer -= Time.deltaTime;
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
        transform.position = Vector3.MoveTowards(transform.position, targetVector, currentSpeed);

        anim.Play("Duplicate");

        //Adjust the 7 to change how close the player must be to the enemy to trigger fleeing during evade
        if (targetVector == transform.position)
        {
            targetVector = fleePoints[Random.Range(0, fleePoints.Count)].transform.position;
            currentSpeed = Random.Range(0.02f, 0.3f);
        }
        int num = Random.Range(0, 1000);
        if(num == 0)
        {
            GameObject newMimic = Instantiate(mimicPrefab);
            newMimic.transform.position = transform.position;
            GameObject.Find("BijouDuplicate").GetComponent<AudioSource>().Play();
        }
    }

    void flip()
    {
        facingRight = !facingRight;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Equals("AttackPoint"))
        { 
            collision.transform.parent.gameObject.GetComponent<Player>().invlunerableTimer = 0.2f;

            health = -100;
        }
    }
}
