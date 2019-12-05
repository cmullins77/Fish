using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : MonoBehaviour
{
    public int timer = 0;
    public int totalTime;
    public List<int> switchTimes;
    public List<int> addTimes;
    public bool bouncing = false;

    Sprite normalSprite;
    public bool Shocking = false;

    Quaternion startQuat;
    Vector3 startLoc;
    SpriteRenderer sR;
    CircleCollider2D cC;
    Animator anim;
    AudioSource aS;
    public bool hit = false;

    // Start is called before the first frame update
    void Start()
    {
        startQuat = transform.rotation;
        sR = GetComponent<SpriteRenderer>();
        cC = GetComponent<CircleCollider2D>();
        anim = GetComponent<Animator>();
        aS = GetComponent<AudioSource>();
        sR.enabled = false;
        cC.enabled = false;
        normalSprite = sR.sprite;
    }

    // Update is called once per frame
    void Update()
    {
        if(sR.enabled == false)
        {
            transform.localPosition = new Vector3(0, 0, 0);
        }
        else if (bouncing && timer <= totalTime)
        {
            if(!GetComponent<SpriteRenderer>().isVisible)
            {
                transform.localPosition = new Vector3(0, 0, 0);
            }
            timer++;
            if (timer == totalTime)
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                GetComponent<Rigidbody2D>().angularVelocity = 0;
                Shocking = false;
            }
            if (addTimes.Count > 0 && timer == addTimes[0])
            {
                addTimes.Remove(addTimes[0]);
                Vector2 vel = GetComponent<Rigidbody2D>().velocity.normalized * Random.Range(150, 450);
                GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                GetComponent<Rigidbody2D>().AddForce(vel);
                GetComponent<Rigidbody2D>().AddTorque(Random.Range(-10, 11));
                print(Shocking);
                if (Shocking)
                {
                    anim.Play("Puffed");
                    Shocking = false;
                    sR.sprite = normalSprite;
                }
                else
                {
                    anim.SetTrigger("Shocked");
                    GameObject.Find("ThlumpElectricSound").GetComponent<AudioSource>().Play();
                    Shocking = true;
                }
            }
            if (switchTimes.Count > 0 && timer == switchTimes[0])
            {
                switchTimes.Remove(switchTimes[0]);
                Vector2 vel = new Vector2(Random.Range(-1, 1.01f), Random.Range(-1, 1.01f)).normalized * Random.Range(150, 450);
                GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                GetComponent<Rigidbody2D>().AddForce(vel);
                GetComponent<Rigidbody2D>().AddTorque(Random.Range(-10, 11));
            }
        } else if (bouncing && (transform.rotation != startQuat || transform.localPosition != new Vector3(0,0,0)) )
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, startQuat, 5);
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, new Vector3(0,0,0), 0.5f);
        } else if(bouncing && transform.rotation == startQuat)
        {
            cC.enabled = false;
            anim.Play("UnPuff");
        }
        else
        {
            sR.enabled = false;
        }
    }
    public void startBounce()
    {
        if(!bouncing)
        {
            GameObject.Find("ThlumpInflateSound").GetComponent<AudioSource>().Play();
            Shocking = false;
            sR.sprite = normalSprite;
            anim.Play("Puff");
            startLoc = transform.position;
            transform.localPosition = new Vector3(0, 0, 0);
            sR.enabled = true;
            cC.enabled = true;
            bouncing = true;
            timer = 0;
            GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-1, 1.01f), Random.Range(-1, 1.01f)).normalized * Random.Range(150, 450));
            GetComponent<Rigidbody2D>().AddTorque(Random.Range(-30, 31));
            switchTimes = new List<int>();
            addTimes = new List<int>();
            totalTime = Random.Range(500, 1000);
            int num = 0;
            while (num < totalTime)
            {
                int num2 = Random.Range(0, 100);
                if (num2 == 0)
                {
                    addTimes.Add(num);
                }
                num2 = Random.Range(0, 200);
                if (num2 == 0)
                {
                    switchTimes.Add(num);
                }
                num++;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(bouncing && collision.name.Equals("Tilemap"))
        {
            GameObject.Find("ThlumpBounceSound").GetComponent<AudioSource>().Play();
        }
        if (collision.name.Equals("AttackPoint") && !Shocking)
        {
            hit = true;
        } else if (collision.name.Equals("AttackPoint"))
        {
            GameObject.Find("Player(Clone)").GetComponent<Player>().Stun();
        }
    }
}
