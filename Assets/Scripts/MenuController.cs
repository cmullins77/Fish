using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public int scene = 0;
    public int currentSelection = 0;
    public bool randomizing = false;
    public int randomTime = 0;
    public float timeBetween = 0;
    public int prevFrame = 0;
    public int timer = 0;

    public bool Paused = false;

    public bool over = false;
    

    List<GameObject> menuElems;

    public GameObject WonGame;
    public GameObject LostGame;

    private void Start()
    {
        WonGame = GameObject.Find("Congratulations");
        LostGame = GameObject.Find("GameOver");
        if (scene != 0 && scene != 1)
        {
            menuElems = new List<GameObject>();
            foreach (Transform t in transform)
            {
                menuElems.Add(t.gameObject);
                t.gameObject.SetActive(false);
            }
        }
        Paused = false;
        Time.timeScale = 1;
        GameObject.Find("GameController").GetComponent<Game>().playing = true;
        menuElems = new List<GameObject>();
        foreach (Transform t in transform)
        {
            menuElems.Add(t.gameObject);
            t.gameObject.SetActive(false);
        }
        LostGame.SetActive(false);
        WonGame.SetActive(false);
    }

    public void enterGame()
    {
        SceneManager.LoadScene(1);
    }
    public void quitGame()
    {
        Application.Quit(); 
    }
    private void Update()
    {
        if (randomizing)
        {
            timer++;
            if (timer < randomTime && timer >= prevFrame + timeBetween)
            {
                prevFrame = timer;
                currentSelection++;
                if (currentSelection > 6)
                {
                    currentSelection = 0;
                }
                GameObject highlight = GameObject.Find("Highlight");
                highlight.transform.SetParent(GameObject.Find(currentSelection.ToString()).transform);
                highlight.transform.localPosition = new Vector3(0, 0, 0);

                timeBetween++;
            }
            if(timer == randomTime + 100)
            {
                SceneManager.LoadScene(currentSelection + 2);
            }
        }
        else
        {
            if (scene == 1)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown("a"))
                {
                    currentSelection--;
                    if (currentSelection < 0)
                    {
                        currentSelection = 7;
                    }
                    GameObject highlight = GameObject.Find("Highlight");
                    highlight.transform.SetParent(GameObject.Find(currentSelection.ToString()).transform);
                    highlight.transform.localPosition = new Vector3(0, 0, 0);
                }
                if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown("d"))
                {
                    currentSelection++;
                    if (currentSelection > 7)
                    {
                        currentSelection = 0;
                    }
                    GameObject highlight = GameObject.Find("Highlight");
                    highlight.transform.SetParent(GameObject.Find(currentSelection.ToString()).transform);
                    highlight.transform.localPosition = new Vector3(0, 0, 0);
                }
                if (Input.GetKeyDown("space"))
                {
                    continueToGame();
                }

            }
            if(scene == 2)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if (Paused)
                    {
                        resumeGame();
                    }
                    else
                    {
                        Paused = true;
                        Time.timeScale = 0;
                        GameObject.Find("GameController").GetComponent<Game>().playing = false;
                        foreach (GameObject g in menuElems)
                        {
                            g.SetActive(true);
                        }
                    }
                }
            }
        }
    }

    public void continueToGame()
    {
        if (currentSelection == 7)
        {
            randomTime = Random.Range(100, 500);
            timer = 0;
            randomizing = true;
        }
        else
        {
            SceneManager.LoadScene(currentSelection + 2);
        }
    }

    public void selectBoss(int num)
    {
        currentSelection = num;
        GameObject highlight = GameObject.Find("Highlight");
        highlight.transform.SetParent(GameObject.Find(currentSelection.ToString()).transform);
        highlight.transform.localPosition = new Vector3(0, 0, 0);
    }

    public void resumeGame()
    {
        Paused = false;
        Time.timeScale = 1;
        GameObject.Find("GameController").GetComponent<Game>().playing = true;
        menuElems = new List<GameObject>();
        foreach (Transform t in transform)
        {
            menuElems.Add(t.gameObject);
            t.gameObject.SetActive(false);
        }
    }
    public void restartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void quitToTitle()
    {
        SceneManager.LoadScene(0);
    }

    public void gameWon(int Score)
    {
        WonGame.SetActive(true);
        over = true;
        GameObject.Find("Score").GetComponent<Text>().text = Score.ToString();
    }

    public void gameLost(int Score)
    {
        LostGame.SetActive(true);
        over = true;
        GameObject.Find("Score").GetComponent<Text>().text = Score.ToString();
    }
}
