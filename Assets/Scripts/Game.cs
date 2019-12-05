using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    //Prefabs of Player and Enemy
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    //Current Instance of Player and Enemy
    Player p;
    Enemy enemy;

    //Checks if Game has been started
    bool started;
    public bool playing;

    //Overall Timer and Score
    float timer;
    int Score;

    float enemyHealth;

    //Songs to Choose from for end
    public AudioClip[] winSongs;
    public AudioClip[] loseSongs;



    // Start is called before the first frame update
    void Start()
    {
        resetGame();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        //If Player is gone then enemy has won
        if (p == null && started)
        {
            Score = (int)Mathf.Floor((1500f / timer)+ enemyHealth / 3.0f)/2;
            print(Score);
            GameObject.Find("Pause").GetComponent<MenuController>().gameLost(Score);
            GameObject.Find("Canvas").GetComponent<AudioSource>().clip = loseSongs[Random.Range(0, loseSongs.Length)];
            GameObject.Find("Canvas").GetComponent<AudioSource>().Play();
            started = false;
            playing = false;
            print(timer);
        }
        //If enemy is gone then player has won
        if(enemy == null && started)
        {
            Score = (int)Mathf.Floor((1500f / timer) + p.health/3.0f)/2;
            print(timer);
            print(Score);
            GameObject.Find("Pause").GetComponent<MenuController>().gameWon(Score);
            GameObject.Find("Canvas").GetComponent<AudioSource>().clip = winSongs[Random.Range(0, winSongs.Length)];
            GameObject.Find("Canvas").GetComponent<AudioSource>().Play();
            started = false;
            playing = false;
        }
        //Restarts Game
        if(Input.GetKeyDown("r"))
        {
            SceneManager.LoadScene(0);
        }
    }


    public void resetGame()
    {
        //Tells Menu Controller that the game is not over
        GameObject.Find("Pause").GetComponent<MenuController>().over = false;

        //Reset the timer and pause game
        timer = 0;
        Time.timeScale = 0;

        //Destroy all Players and Enemies
        Player[] players = FindObjectsOfType(typeof(Player)) as Player[];
        Enemy[] enemies = FindObjectsOfType(typeof(Enemy)) as Enemy[];
        foreach (Player player in players)
        {
            Destroy(player.gameObject);
        }
        foreach(Enemy e in enemies)
        {
            Destroy(e.gameObject);
        }
     

        //Instantiate new Player and Enemy
        GameObject newP = Instantiate(playerPrefab);
        newP.transform.position = GameObject.Find("PlayerSpawn").transform.position;
        GameObject newE = Instantiate(enemyPrefab);
        newE.transform.position = GameObject.Find("EnemySpawn").transform.position;
        p = newP.GetComponent<Player>();
        enemy = newE.GetComponent<Enemy>();

        //Start the Game
        started = false;
        playing = false;
        StartGame();
           
    }
    void StartGame()
    {
        //Unpause
        Time.timeScale = 1;
        started = true;

        //Reset health bars
        GameObject.Find("FishHealthBar").GetComponent<RectTransform>().sizeDelta = new Vector2(100, 10);
        GameObject.Find("PlayerHealthBar").GetComponent<RectTransform>().sizeDelta = new Vector2(100, 10);
        playing = true;
    }

    //Set Health Bars
    public void setEnemyHealth(float health)
    {
        if(health < 0)
        {
            health = 0;
        }
        enemyHealth = health;
        GameObject.Find("FishHealthBar").GetComponent<RectTransform>().sizeDelta = new Vector2(health, 10);
    }
    public void setPlayerHealth(float health)
    {
        if (health < 0)
        {
            health = 0;
        }
        GameObject.Find("PlayerHealthBar").GetComponent<RectTransform>().sizeDelta = new Vector2(health, 10);

    }
}
