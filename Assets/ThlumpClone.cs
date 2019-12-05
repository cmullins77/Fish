using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThlumpClone : Enemy
{

    List<GameObject> fleePoints;
    public Transform thwompHeight;
    public Transform groundHeight;

    List<float> shockTimes;
    float nextTime;

    public Sprite thwompSprite;

    float hitTimer;

    public BoxCollider2D thwompAttackPoint;
    public CircleCollider2D bounceAttackPoint;

    public Bounce ThlumpBounce;
    bool foundSpot;

    public AudioClip thlumpThwomp;
    public AudioClip thlumpExplode;

    AudioSource aS;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sR = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        state = Enemy.State.Evade;
        deathParticles.gameObject.SetActive(false);

        targetVector = transform.position;

        gameObject.name = "Clone";

        fleePoints = new List<GameObject>();
        foreach (Transform child in GameObject.Find("FleePoints").transform)
        {
            fleePoints.Add(child.gameObject);
        }

        thwompHeight = GameObject.Find("Thwomp").transform;
        groundHeight = GameObject.Find("Ground").transform;
        print(groundHeight);
        facingRight = false;

        thwompAttackPoint.enabled = false;
        bounceAttackPoint.enabled = false;
        waitTime = 2f;

        aS = GetComponent<AudioSource>();
        aS.enabled = false;
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
            switch (state)
            {
                case (Enemy.State.Idle):
                    handleIdle();
                    break;
                case (Enemy.State.FlyUp):
                    handleFlyUp();
                    break;
                case (Enemy.State.Thwomp):
                    handleThwomp();
                    break;
                case (Enemy.State.Bounce):
                    handleBounce();
                    break;
                case (Enemy.State.Evade):
                    handleEvade();
                    break;
                case (Enemy.State.Explode):
                    handleExplode();
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
        thwompAttackPoint.enabled = false;
        bounceAttackPoint.enabled = false;
        transform.position = Vector3.MoveTowards(transform.position, targetVector, 0.1f);
        //TODO: Decouple
        Vector3 distanceVector = GameObject.Find("Player(Clone)").transform.position - targetVector;
        float fishDistance = (targetVector - transform.position).magnitude;
        float PlayerDistance = distanceVector.magnitude;
        if (PlayerDistance < 7 && fishDistance < 0.1f)
        {
            targetVector = fleePoints[Random.Range(0, fleePoints.Count)].transform.position;
        }
        timer += Time.deltaTime;
        if (timer > waitTime)
        {
            int num3 = Random.Range(0,8);
            if (num3 < 2)
            {
                bounceAttackPoint.enabled = true;
                GetComponent<SpriteRenderer>().enabled = false;
                GetComponent<CircleCollider2D>().enabled = false;
                GetComponent<BoxCollider2D>().enabled = false;
                timer = 0;
                print("Bounce");
                ThlumpBounce.startBounce();
                state = Enemy.State.Bounce;

            } else if (num3 == 2)
            {
                state = Enemy.State.Explode;
                anim.Play("Explode");
                exploding = true;
                ThlumpBounce.GetComponent<Animator>().Play("ExplodeAttack");
                GetComponent<AudioSource>().clip = thlumpExplode;
            }
            else
            {
                state = Enemy.State.FlyUp;
                timer = 0;
                targetVector = new Vector3(GameObject.Find("Player(Clone)").transform.position.x, thwompHeight.position.y, 0);
                foundSpot = false;
            }
        }
    }

    void handleThwomp()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetVector, 0.5f);
        if(transform.position == targetVector && targetVector.y != GameObject.Find("Ground").transform.position.y)
        {
            state = Enemy.State.Evade;
            timer = 0;
            ThlumpBounce.GetComponent<Animator>().Play("UnPuffed");
            print("Unpuffing");
        } else if(transform.position == targetVector)
        {
            anim.Play("Thwomp");
            if(timer == 0)
            {
                GetComponent<AudioSource>().clip = thlumpThwomp;
                GetComponent<AudioSource>().enabled = true;
            }
            timer += Time.deltaTime;
            if (timer > 1.5f)
            {
                targetVector = fleePoints[Random.Range(0, fleePoints.Count)].transform.position;
                anim.Play("Idle");
                thwompAttackPoint.enabled = false;

            }
        }
    }

    void handleExplode()
    {
        print("Explode Time~");
        if(!exploding)
        {
            print("Done");
            state = Enemy.State.Evade;
            timer = 0;
            ThlumpBounce.GetComponent<Animator>().Play("UnPuffed");
            anim.Play("Idle");
        }
    }

    void handleFlyUp() 
    {
        if (foundSpot)
        {
            print("Found Spot");
            timer += Time.deltaTime;
            print(timer);
            if(timer > 0.45f)
            {
                print("THWOMP;");
                groundHeight = GameObject.Find("Ground").transform;
                targetVector = new Vector3(transform.position.x, groundHeight.position.y, 0);
                state = Enemy.State.Thwomp;
                ThlumpBounce.GetComponent<Animator>().Play("ThwompAttack");
                timer = 0;
                thwompAttackPoint.enabled = true;
                GetComponent<AudioSource>().clip = null;
            }
        }
        else
        {
            print("Bad");
            timer += Time.deltaTime;
            if(timer < 2)
                targetVector = new Vector3(GameObject.Find("Player(Clone)").transform.position.x, thwompHeight.position.y, 0);
            else
            {
                timer = 0;
                targetVector = transform.position;
            }
            transform.position = Vector3.MoveTowards(transform.position, targetVector, 0.25f);
            if(targetVector == transform.position)
            {
                timer = 0;
                foundSpot = true;
            }
        }

    }


    void handleBounce()
    {
        timer++;
        if(!ThlumpBounce.bouncing)
        {
            print("NOT BOUNCING");
            GetComponent<SpriteRenderer>().enabled = true;
            bounceAttackPoint.enabled = false;
            timer = 0;
            state = State.Evade;
            GetComponent<BoxCollider2D>().enabled = true;
        }
        if(ThlumpBounce.hit)
        {
            ThlumpBounce.hit = false;
            health = health - 5;
            waitTime -= 0.05f;
            GameObject.Find("GameController").GetComponent<Game>().setEnemyHealth(health);
            GameObject.Find("EnemyHitSound").GetComponent<AudioSource>().Play();
            hitTimer = 0.6f;
        }
    }

    void flip()
    {
        facingRight = !facingRight;
        print("FLIP: " + facingRight);
        if(facingRight)
        {
            transform.localScale = new Vector3(-0.7f, 0.7f, 0.2f);
        }
        else
        {
            transform.localScale = new Vector3(0.7f, 0.7f, 0.2f);
        }
    }

    void goToEvade()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    { 
        if(hitTimer <= 0)
        {
            if (collision.name.Equals("AttackPoint") &&
            ((collision.transform.position.x - collision.transform.parent.position.x) > 0 && (collision.transform.position.x - transform.position.x) < 0) ||
                ((collision.transform.position.x - collision.transform.parent.position.x) < 0 && (collision.transform.position.x - transform.position.x) > 0))
            {
                collision.transform.parent.gameObject.GetComponent<Player>().invlunerableTimer = 0.5f;
                if (state != Enemy.State.Bounce && state != Enemy.State.Explode)
                {
                    health = health - 4;
                    waitTime -= 0.05f;
                    sR.color = new Color(1, 1, 1);
                    rb.velocity = new Vector2(0, 0);
                    targetVector = fleePoints[Random.Range(0, fleePoints.Count)].transform.position;
                    state = Enemy.State.Evade;
                    thwompAttackPoint.enabled = false;
                    bounceAttackPoint.enabled = false;
                    GameObject.Find("EnemyHitSound").GetComponent<AudioSource>().Play();
                    hitParticles.gameObject.transform.position = GameObject.Find("AttackPoint").transform.position;
                    hitParticles.Play("HotSteveHit");
                    hitTimer = 0.6f;
                    GetComponent<BoxCollider2D>().enabled = true;
                    anim.Play("Hit");
                    rb.AddForce((collision.transform.position - transform.position).normalized * 5);
                }
                else
                {
                    rb.AddForce((collision.transform.position - transform.position).normalized * 10);
                }
            }
        }
    }

}
