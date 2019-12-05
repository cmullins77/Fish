using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    //Audio
    public AudioSource footsteps;
    public bool footstepSound = false;
    public AudioClip m_JumpSound;           // the sound played when character leaves the ground.
    public AudioClip m_LandSound;
    AudioSource aud;
    public AudioClip hitSound1;
    public AudioClip hitSound2;


    //Physics and Movement
    Animator anim;
    Rigidbody2D rb;
    float speed = 5f;
    bool facingRight = true;

    bool grounded = false;
    public Transform groundCheck;
    float groundRadius = 0.2f;
    public LayerMask whatIsGround;

    public bool doingAMove;

    public float jumpForce = 1000;


    //health
    public float health = 100;


    //timers
    float attackTimer;
    float stunTimer;

    public bool beingStrangled;
    public float breakFreeTimer = 0;
    public bool breakingFree;
    public float freeTimer;

    float hitTimer = 0;
    public bool stunned = false;
    public Animator hitParticles;

    public float invlunerableTimer = 0;


    //Parts of Body for Hit
    PlayerHead head;
    PlayerChest chest;
    PlayerAbdomin abdomin;
    PlayerLeg leg1;
    PlayerLeg leg2;
    PlayerArm arm1;
    PlayerArm arm2;


    //Death
    public bool die = false;
    bool dying = false;


    // Start is called before the first frame update
    void Start()
    {
        //GetComponents
        aud = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        //Get all Body Parts
        foreach (Transform c in transform)
        {
            if (c.gameObject.name.Equals("HeadPoint"))
            {
                head = c.gameObject.GetComponent<PlayerHead>();
            }
            if (c.gameObject.name.Equals("ChestPoint"))
            {
                chest = c.gameObject.GetComponent<PlayerChest>();
            }
            if (c.gameObject.name.Equals("GroinPoint"))
            {
                abdomin = c.gameObject.GetComponent<PlayerAbdomin>();
            }
            if (c.gameObject.name.Equals("LegPoint1"))
            {
                leg1 = c.gameObject.GetComponent<PlayerLeg>();
            }
            if (c.gameObject.name.Equals("LegPoint2"))
            {
                leg2 = c.gameObject.GetComponent<PlayerLeg>();
            }
            if (c.gameObject.name.Equals("ArmPoint1"))
            {
                arm1 = c.gameObject.GetComponent<PlayerArm>();
            }
            if (c.gameObject.name.Equals("ArmPoint2"))
            {
                arm2 = c.gameObject.GetComponent<PlayerArm>();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //decrements timer of invunerablity between getting hit
        invlunerableTimer -= Time.deltaTime;

        //If stunned (or shocked) by Thlump player is stuck for period of time
        if (stunned)
        {
            stunTimer += Time.deltaTime;
            if(stunTimer > 1)
            {
                stunned = false;
                anim.Play("Idle");
            }
        }
        else
        {
        
            hitTimer -= Time.deltaTime;

            //If the players health falls below 0 but they are not yet in the dying state, set the animation to dying and dying to true
            if (health <= 0 && !dying)
            {
                GameObject.Find("GameController").GetComponent<Game>().playing = false;
                anim.SetTrigger("Death");
                dying = true;
                beingStrangled = false;
                if(GameObject.Find("Enemy").GetComponent <Enemy>().fishName == Enemy.FishName.Bijou)
                {
                    for (int i = 1; i < GameObject.Find("Enemy").GetComponent<Bijou>().totalFirePatterns; i++)
                    {
                        GameObject.Find("FireBalls" + i).transform.position = new Vector3(0, 1000, 0);
                    }
                }
            }
            else if (dying) //If dying has already been triggered check to see if "die" bool has been set to true by dying animation and then destroy self
            {
                if (die)
                {
                    Destroy(gameObject);
                }
            }
            else if (beingStrangled) //If being strangeld by Hamburger, stop all other movement other than ability to break free
            {
                breakFreeTimer += Time.deltaTime;
                freeTimer += Time.deltaTime;
                if (Input.GetKeyDown("space"))
                {
                    if(!breakingFree)
                    {
                        GameObject.Find("Enemy").GetComponent<Animator>().Play("StrangleBreak");
                    }
                    breakingFree = true;
                    breakFreeTimer = 0;
                }
                if (breakFreeTimer > 0.2f)
                {
                    if(breakingFree)
                    {
                        GameObject.Find("Enemy").GetComponent<Animator>().Play("Strangle");
                    }
                    breakingFree = false;
                    freeTimer = 0;
                }
                freeTimer += Time.deltaTime;
                if (freeTimer > 2f)
                {
                    //print("FREE");
                    beingStrangled = false;
                    breakingFree = false;
                    GameObject.Find("Enemy").GetComponent<Hamburger>().timer = 0;
                    GameObject.Find("Enemy").GetComponent<Hamburger>().state = Enemy.State.FlyDown;
                    GameObject.Find("Enemy").GetComponent<Animator>().Play("StrangleBack");
                    anim.Play("Idle");
                    transform.position = new Vector3(-6.4f, -3.24f, 0);
                }
            }
            else
            {
                //If all is normal allow player control

                //Play footstep sound, triggered by animation
                if (footstepSound)
                {
                    footsteps.Play();
                    footstepSound = false;
                }

                //If player somehow falls off screen, kill player
                if (transform.position.y < -300)
                {
                    health = 0;
                }

                //Check to see if currently grounded or landing
                bool wasGrounded = grounded;
                grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
                //if landing play landing sound
                if (!wasGrounded && grounded)
                {
                    aud.clip = m_LandSound;
                    aud.Play();
                }
                anim.SetBool("Grounded", grounded);

                //If the player isn't currently punching or kicking allow movement control
                if (!doingAMove)
                {
                    //Get horizontal and vertical axises
                    float move = Input.GetAxis("Horizontal");
                    float value = Input.GetAxis("Vertical");

                    //If down button is pressed (either s or down arrow or down on a controller) crouch
                    if (value < 0)
                    {
                        anim.SetTrigger("Crouch");
                    }
                    else //If not crouching allow player to move horizontally
                    {
                        rb.velocity = new Vector2(move * speed, rb.velocity.y);
                    }

                    //Set the speed to the current movement so player either idles or walks
                    anim.SetFloat("Speed", Mathf.Abs(move));

                    //Check if sprite needs to be flipped
                    if ((facingRight && move < 0) || (!facingRight && move > 0))
                    {
                        flip();
                    }

                    //Increment stun and attack timer (first is delay in ability to attack because player has been hit, second is cool down between attacks)
                    stunTimer += Time.deltaTime;
                    attackTimer += Time.deltaTime;

                    //If the player is allowed to hit, left = punch, right = kick
                    if (stunTimer > 0.25f && attackTimer > 0.5f && value >= 0)
                    {
                        if (Input.GetMouseButton(0))
                        {
                            anim.SetTrigger("Punch");
                            GameObject.Find("PlayerPunchSound").GetComponent<AudioSource>().Play();
                            doingAMove = true;
                            if (move > 0 || move < 0)
                            {
                                rb.velocity = new Vector2(move * speed * 3, rb.velocity.y);
                            }
                            attackTimer = 0;
                        }
                        if (Input.GetMouseButton(1))
                        {
                            anim.SetTrigger("Kick");
                            GameObject.Find("PlayerKickSound").GetComponent<AudioSource>().Play();
                            doingAMove = true;
                            if (move > 0 || move < 0)
                            {
                                rb.velocity = new Vector2(move * speed * 3, rb.velocity.y);
                            }
                            attackTimer = 0;
                        }
                    }
                    //If up arrow is pressed and player is currently grounded then the player can jump
                    if (grounded && Input.GetKeyDown("w"))
                    {
                        //print(Input.GetAxis("Vertical"));
                        anim.SetBool("Grounded", false);
                        rb.AddForce(new Vector2(0, jumpForce));
                        aud.clip = m_JumpSound;
                        aud.Play();
                    }
                }
            }
        }
    }

    //Player is stunned (or shocked) by Thlump
    public void Stun()
    {
        health = health - 4;
        GameObject.Find("GameController").GetComponent<Game>().setPlayerHealth(health);
        stunTimer = 0;
        stunned = true;
        anim.Play("Shock");
    }

    //Flip sprite direction
    void flip()
    {
        facingRight = !facingRight;
        //GetComponent<SpriteRenderer>().flipX = !facingRight;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    public void takeDamage(int healthD)
    {
        hitParticles.Play("HotSteveHit");
        hitParticles.transform.position = head.transform.position;
        GameObject.Find("PlayerHitSound").GetComponent<AudioSource>().clip = hitSound1;
        hitTimer = 1;
        health = health - healthD;
        GameObject.Find("GameController").GetComponent<Game>().setPlayerHealth(health);
    }

    //Checks for trigger colliders
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if it has been long enough between hits then check if player has been hit by an enemy object
        if (hitTimer <=0 && invlunerableTimer <= 0)
        {
            if (collision.name.Equals("SliderBullet(Clone)"))
            {   
                GameObject.Find("PlayerHitSound").GetComponent<AudioSource>().clip = hitSound2;
                GameObject.Find("PlayerHitSound").GetComponent<AudioSource>().Play();

                //Set stun timer
                stunTimer = 0;

                //Update health bar
                GameObject.Find("GameController").GetComponent<Game>().setPlayerHealth(health);

                //Play hit particles
                hitParticles.Play("HotSteveHit");

                health = health - 5;
                hitTimer = 1;

                anim.Play("Hit");

            }
            //Attack point is a collider child of the enemy that moves based on their attack
            if (collision.name.Equals("EnemyAttackPoint") || collision.name.Equals("HotSteveProjectile(Clone)") || collision.name.Equals("Beam"))
            {
                //Check which part of body has been hit and decrement health accordingly
                bool hit = false;
                if (head.hit)
                {
                    health = health - 5;
                    if (collision.name.Equals("EnemyAttackPoint") && collision.transform.parent.GetComponent<Enemy>() != null && collision.transform.parent.GetComponent<Enemy>().fishName != Enemy.FishName.Mimosa)
                        health = health + 1f;
                    hit = true;
                    hitParticles.gameObject.transform.position = head.transform.position;
                }
                else if (abdomin.hit)
                {
                    health = health - 4;
                    hit = true;
                    hitParticles.gameObject.transform.position = abdomin.transform.position;
                }
                else if (chest.hit)
                {
                    health = health - 2.5f;
                    hit = true;
                    hitParticles.gameObject.transform.position = chest.transform.position;
                }
                else if (arm1.hit || arm2.hit)
                {
                    health = health - 1.5f;
                    hit = true;
                    if (arm1.hit)
                        hitParticles.gameObject.transform.position = arm1.transform.position;
                    else
                        hitParticles.gameObject.transform.position = arm2.transform.position;
                }
                else if (leg1.hit || leg2.hit)
                {
                    health = health - 0.5f;
                    hit = true;
                    if (leg1.hit)
                        hitParticles.gameObject.transform.position = leg1.transform.position;
                    else
                        hitParticles.gameObject.transform.position = leg2.transform.position;
                }
                if (hit)
                {
                    if(collision.name.Equals("HotSteveProjectile(Clone)"))
                    {
                        Destroy(collision.gameObject);
                    } else if (collision.name.Equals("EnemyAttackPoint") && collision.transform.parent.GetComponent<Object>() != null)
                    {
                        Destroy(collision.gameObject.transform.parent.gameObject);
                    }
                    if(GameObject.Find("Clone") == null)
                    {
                        //Decrement health more based on certain criteria to help balance game
                        if (GameObject.Find("Enemy").GetComponent<Enemy>().exploding)
                        {
                            health = health - 10;
                        }
                        if (GameObject.Find("Enemy").GetComponent<Enemy>().fishName == Enemy.FishName.HotSteve)
                        {
                            health = health - 1;
                        }
                        else if (GameObject.Find("Enemy").GetComponent<Enemy>().fishName == Enemy.FishName.Slider)
                        {
                            health = health - 4;
                        }
                        else if (GameObject.Find("Enemy").GetComponent<Enemy>().fishName == Enemy.FishName.JeremyLegg)
                        {
                            health = health - 3;
                        }
                        else if (GameObject.Find("Enemy").GetComponent<Enemy>().fishName == Enemy.FishName.Bijou)
                        {
                            health = health - 3;
                        }
                        else if (GameObject.Find("Enemy").GetComponent<Enemy>().fishName == Enemy.FishName.Hamburger)
                        {
                            health = health - 3;
                        }

                    }
                    else
                    {
                        if (GameObject.Find("Clone").GetComponent<Enemy>().exploding)
                        {
                            health = health - 10;
                        }
                        if (GameObject.Find("Clone").GetComponent<Enemy>().fishName == Enemy.FishName.HotSteve)
                        {
                            health = health - 1;
                        }
                        else if (GameObject.Find("Clone").GetComponent<Enemy>().fishName == Enemy.FishName.Slider)
                        {
                            health = health - 4;
                        }
                        else if (GameObject.Find("Clone").GetComponent<Enemy>().fishName == Enemy.FishName.JeremyLegg)
                        {
                            health = health - 3;
                        }
                        else if (GameObject.Find("Clone").GetComponent<Enemy>().fishName == Enemy.FishName.Bijou)
                        {
                            health = health - 3;
                        }
                        else if (GameObject.Find("Clone").GetComponent<Enemy>().fishName == Enemy.FishName.Hamburger)
                        {
                            health = health - 3;
                        }
                    }
                    //Add knockback force
                    rb.AddForce(new Vector2((transform.position.x - collision.transform.position.x), (transform.position.y - collision.transform.position.y)).normalized * 100);

                    //Play hit sounds
                    if (collision.name.Equals("EnemyAttackPoint"))
                    {
                        GameObject.Find("PlayerHitSound").GetComponent<AudioSource>().clip = hitSound1;
                    }
                    else
                    {
                        GameObject.Find("PlayerHitSound").GetComponent<AudioSource>().clip = hitSound2;
                    }
                    GameObject.Find("PlayerHitSound").GetComponent<AudioSource>().Play();

                    //Set stun timer
                    stunTimer = 0;

                    //Update health bar
                    GameObject.Find("GameController").GetComponent<Game>().setPlayerHealth(health);

                    //Play hit particles
                    hitParticles.Play("HotSteveHit");


                    hitTimer = 1;


                    //For Thlump: check if Thlump is shocking and if he is then take more health and stun playerxs
                    Bounce b = null;
                    Bounce[] Bs = FindObjectsOfType(typeof(Bounce)) as Bounce[];
                    foreach(Bounce bounce in Bs)
                    {
                        b = bounce;
                    }
                    if (b != null && b.Shocking)
                    {
                        Stun();
                    }
                    if (b != null && collision is BoxCollider2D)
                    {
                        health = health - 5;
                    }
                    if ((b == null || !b.Shocking) && health > 0)
                    {
                        anim.Play("Hit");
                        //GameObject.Find("Main Camera").GetComponent<Animator>().Play("ShakeCam");
                    }
                }
            }
        }
    }
}
