using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JeremyLeggClone : Enemy
{
    //TODO: Add any Lists of Points or Specific Points within Screen that Are used for Attacks or Other Behavior
    List<GameObject> fleePoints;


    float hitTimer;

    public bool shoot = false;
    public int turretNum = 0;
    public int legNum = 0;

    public GameObject[] legPrefabs;
    GameObject bulletSpawn;
    GameObject bulletTarget;
    bool shot = false;
    GameObject gunTurret;

    public Sprite JeremyHead;
    public Sprite JeremyHeadShoot;
    public Sprite defaultSprite;
    public bool shooting = false;
    public float shootTimer;
    float speed;

    Quaternion startRotation;

    BoxCollider2D normalCollider;
    CircleCollider2D headCollider;
    public Sprite[] bubbleSprites;
    public int bubbleNum;


    public bool head;

    public float bubbleCountdown = 6f;
    int currNum = 3;


    // Start is called before the first frame update
    void Start()
    {
        head = false;

        normalCollider = GetComponent<BoxCollider2D>();
        headCollider = GetComponent<CircleCollider2D>();
        headCollider.enabled = false;
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

        gameObject.name = "Clone";

        facingRight = false;

        //TODO: Modify Value as Starting Time between attacks
        waitTime = 2f;

        transform.position = GameObject.Find("JeremyLeggPosition").transform.position;

        shoot = false;
        bulletSpawn = GameObject.Find("BulletSpawn");
        bulletTarget = GameObject.Find("BulletTarget");

        gunTurret = GameObject.Find("LeggTurret");
        fishName = Enemy.FishName.JeremyLegg;
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
                case (Enemy.State.Shoot):
                    print(GameObject.Find("JeremyLeggShoot").transform.position);
                    handleShoot();
                    break;
                case (Enemy.State.JeremyLegg):
                    handleJeremyLegg();
                    break;
                case (Enemy.State.Bubble):
                    handleBubble();
                    break;
            }
            if (state == Enemy.State.Evade)
            {
                if (transform.position.x < startX && facingRight)
                {
                    flip();
                }
                else if (transform.position.x > startX && !facingRight)
                {
                    flip();
                }
                if (Mathf.Abs(transform.position.x - startX) < 0.1)
                {
                    if (GameObject.Find("Player(Clone)").transform.position.x < transform.position.x && facingRight)
                    {
                        flip();
                    }
                    else if (GameObject.Find("Player(Clone)").transform.position.x > transform.position.x && !facingRight)
                    {
                        flip();
                    }
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

    void handleShoot()
    {
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(0, 3.5f, 0), 1f);
        if (!shot)
        {
            int num2 = Random.Range(1, 5);
            gunTurret = GameObject.Find("LeggTurret");
            GetComponent<Animator>().Play("Legs" + num2);
            shot = true;
            gunTurret.GetComponent<LeggTurret>().shotDone = false;
            if (facingRight && transform.position.x > 0)
            {
                flip();
            }
            else if (!facingRight && transform.position.x < 0)
            {
                flip();
            }
        }
        if (gunTurret.GetComponent<LeggTurret>().shotDone)
        {
            state = Enemy.State.Evade;
            anim.Play("Normal");
            shot = false;
            timer = 0;

        }
    }

    void handleBubble()
    {
        if (bubbleNum == 0)
        {
            GameObject.Find("Number").GetComponent<Animator>().Play("None");
            anim.Play("Normal");
            state = Enemy.State.Evade;
            timer = 0;
            normalCollider.enabled = true;
        }
        else
        {
            bubbleCountdown -= Time.deltaTime;
            if (bubbleCountdown < 0)
            {
                anim.Play("BubbleShatter");
                GameObject.Find("Number").GetComponent<Animator>().Play("None");
                GameObject.Find("Player(Clone)").GetComponent<Animator>().Play("Hit");
                state = Enemy.State.Evade;
                timer = 0;
                normalCollider.enabled = true;
                GameObject.Find("Player(Clone)").GetComponent<Player>().takeDamage(20);
                GameObject.Find("Shatter").GetComponent<AudioSource>().Play();
            }
            else if (bubbleCountdown < 1 && currNum == 1)
            {
                print("LESS THAN 1");
                currNum = 0;
                GameObject.Find("Number").GetComponent<Animator>().Play("1to0");
            }
            else if (bubbleCountdown < 2 && currNum == 2)
            {
                print("LESS THAN 2");
                currNum = 1;
                GameObject.Find("Number").GetComponent<Animator>().Play("2to1");
            }
            else if (bubbleCountdown < 3 && currNum == 3)
            {
                print("LESS THAN 3");
                currNum = 2;
                GameObject.Find("Number").GetComponent<Animator>().Play("3to2");
            }
            if (bubbleNum < 0 || bubbleNum > 7)
            {
                bubbleNum = 0;
            }
            GameObject.Find("Bubble").GetComponent<SpriteRenderer>().sprite = bubbleSprites[bubbleNum];
            normalCollider.enabled = false;
            headCollider.enabled = true;
        }

    }

    void handleJeremyLegg()
    {
        if (head)
        {
            timer += Time.deltaTime;
            if (shooting)
            {
                anim.Play("ShootingHead");
                if (timer > shootTimer)
                {
                    shootTimer = timer + Random.Range(0.3f, 1.2f);
                    shooting = false;
                    GameObject.Find("Beam").GetComponent<Animator>().Play("NotShooting");
                    targetVector = new Vector3(Random.Range(-7.5f, 7.5f), Random.Range(-3.75f, 3.75f), 0);
                    speed = Random.Range(1.75f, 5.5f);
                    GameObject.Find("Beam").transform.rotation = startRotation;
                }
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, targetVector, 0.2f);
                if (transform.position == targetVector)
                {
                    targetVector = new Vector3(Random.Range(-7.5f, 7.5f), Random.Range(-3.75f, 3.75f), 0);
                    speed = Random.Range(1.75f, 5.5f);
                }
                anim.Play("Head");
                if (timer > shootTimer)
                {
                    shootTimer = timer + Random.Range(0.2f, 0.6f);
                    shooting = true;
                    GameObject.Find("Beam").transform.position = GameObject.Find("BeamSpot").transform.position;
                    GameObject.Find("JeremyLeggShoot").GetComponent<AudioSource>().Play();


                    GameObject.Find("Beam").transform.Rotate(0, 0, Random.Range(-70f, 70f));
                    GameObject.Find("Beam").GetComponent<Animator>().Play("Shoot");
                }
            }
        }
    }

    void handleEvade()
    {
        //TODO: Adjust float value to change speed of enemy while moving during evade
        transform.position = Vector3.MoveTowards(transform.position, GameObject.Find("JeremyLeggPosition").transform.position, 1f);

        Vector3 distanceVector = GameObject.Find("Player(Clone)").transform.position - targetVector;
        float fishDistance = (targetVector - transform.position).magnitude;
        float PlayerDistance = distanceVector.magnitude;

        //Adjust the 7 to change how close the player must be to the enemy to trigger fleeing during evade
        if (PlayerDistance < 7 && fishDistance < 0.1f)
        {
            targetVector = fleePoints[Random.Range(0, fleePoints.Count)].transform.position;
        }
        timer += Time.deltaTime;
        if (timer > waitTime && transform.position == GameObject.Find("JeremyLeggPosition").transform.position)
        {
            hitTimer = 1f;
            //TODO: Adjust random range values to change probability of an attack occuring
            int num3 = Random.Range(0, 3);
            //TODO: Here are some examples, change as required for your attacks
            if (num3 == 0)
            {
                timer = 0;
                state = Enemy.State.Shoot;
                targetVector = GameObject.Find("JeremyLeggShoot").transform.position;
                shot = false;
                anim.Play("Summoning");
                targetVector = GameObject.Find("JeremyLeggShoot").transform.position;

            }
            else if (num3 == 1)
            {
                timer = 0;
                shootTimer = timer + Random.Range(0.3f, 1.2f);
                shooting = false;
                state = Enemy.State.JeremyLegg;
                targetVector = new Vector3(Random.Range(-7.5f, 7.5f), Random.Range(-3.75f, 3.75f), 0);
                speed = Random.Range(1.75f, 5.5f);
                headCollider.enabled = true;
                normalCollider.enabled = false;
                startRotation = GameObject.Find("Beam").transform.rotation;
                GameObject.Find("JeremyLeggTransform").GetComponent<AudioSource>().Play();
                head = false;
                anim.Play("HeadTransform");
                hitTimer = 2f;
            }
            else
            {
                timer = 0;
                hitTimer = 0;
                bubbleNum = 7;
                bubbleCountdown = 8;
                currNum = 3;
                state = Enemy.State.Bubble;
                anim.Play("Bubble1");
                GameObject.Find("Bubble").GetComponent<SpriteRenderer>().sprite = bubbleSprites[bubbleNum];
                normalCollider.enabled = false;
                headCollider.enabled = false;
                facingRight = true;
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
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
        if (hitTimer <= 0 && !shooting)
        {
            if (collision.name.Equals("AttackPoint"))
            {

                //TODO: Adjust Value to change how long player is invulnerable after successfully hitting the enemy (can be 0)

                //TODO: Some states/attacks may require their own special results for an attack so if that's the case put those here, and if not remove the if statement and always run the code
                if (state == Enemy.State.Bubble)
                {
                    bubbleNum--;
                    int num = 7 - bubbleNum;
                    anim.Play("Bubble" + num);
                    print(bubbleNum);
                    GameObject.Find("Bubble").GetComponent<AudioSource>().Play();
                }
                else if (state != Enemy.State.Bounce && state != Enemy.State.Explode)
                {
                    collision.transform.parent.gameObject.GetComponent<Player>().invlunerableTimer = 0.5f;
                    //TODO: Adjust health value to balance enemy
                    health = health - 4;

                    //TODO: Adjust value of waittime change, basically attack frequency increases as waittime decreases
                    waitTime -= 0.0005f;



                    //TODO: If any special indication that enemy was hit, change that here
                    sR.color = new Color(1, 1, 1);


                    if (state != Enemy.State.Shoot)
                    {
                        targetVector = fleePoints[Random.Range(0, fleePoints.Count)].transform.position;
                        state = Enemy.State.Evade;
                        sR.sprite = defaultSprite;
                        GameObject.Find("Beam").GetComponent<Animator>().Play("NotShooting");
                        headCollider.enabled = false;
                        normalCollider.enabled = true;

                        anim.Play("Hit");

                    }

                    GameObject.Find("EnemyHitSound").GetComponent<AudioSource>().Play();
                    hitParticles.gameObject.transform.position = GameObject.Find("AttackPoint").transform.position;
                    hitParticles.Play("HotSteveHit");

                    //TODO: Adjust invulnerability time after getting hit
                    hitTimer = 0.6f;


                    //TODO: Could have special hurt animation


                    //TODO: Adjust knockback
                    //rb.AddForce((collision.transform.position - transform.position).normalized * 5);
                }
                else
                {
                    //TODO: Specify anything for Special Cases
                }
            }
        }
    }
}

