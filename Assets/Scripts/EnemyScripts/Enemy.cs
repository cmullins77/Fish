using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum State { Idle, Charge, Shoot, Evade, FlyUp, FlyDown, Dying, Strike, Strangle, Summon, Move, Stunned, Thwomp, Bounce, Explode, JeremyLegg, Bubble, Spline, Throw, Duplicate, Fire, Invisible};
    public enum FishName { HotSteve, Thlump, JeremyLegg, Mimosa, Slider, Bijou, Hamburger};
    public Enemy.FishName fishName;
    public Enemy.State state;
    protected bool facingRight = false;

    public bool shocking = false;

    //Components
    protected Rigidbody2D rb;
    protected SpriteRenderer sR;
    protected Animator anim;

    //Death
    public Animator hitParticles;
    public ParticleSystem deathParticles;

    public Vector3 targetVector;

    public float health = 100;

    public float timer;
    protected float waitTime = 1.5f;

    public bool exploding = false;

    private void Start()
    {

    }


}
