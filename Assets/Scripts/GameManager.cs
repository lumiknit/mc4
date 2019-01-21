using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    OzHuman playerOz;

    public GameObject fadeInPanel;

    public GameObject ozAI;

    /* Game Statistics */
    public static int killCount;
    public static float gameBeginningTime;
    public static float gameEndTime;
    public static float gameDuration { get { return gameEndTime - gameBeginningTime; } }

    public static float spawnTimer;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        killCount = 0;
        gameBeginningTime = Time.time;
        gameEndTime = float.NaN;

        playerOz = GameObject.Find("Oz").transform.Find("human2a").GetComponent<OzHuman>();

        for(int i = 0; i < 30; i++) {
            SpawnRowingNPC();
        }

        GetComponent<SpawnRings>().Spawn();
        GetComponent<SpawnSpoon>().Spawn();

        fadeInPanel = GameObject.Find("FadeInPanel");
        var image = fadeInPanel.GetComponent<Image>();
        image.color = new Color(0f, 0f, 0f, 1f);
        spawnTimer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.W)) {
            playerOz.action = new OzHuman.TestRowFront(playerOz);
        }
        if(Input.GetKeyDown(KeyCode.S)) {
            playerOz.action = new OzHuman.TestRowBack(playerOz);
        }
        if(Input.GetKeyDown(KeyCode.A)) {
            playerOz.action = new OzHuman.TestRowLeft(playerOz);
        }
        if(Input.GetKeyDown(KeyCode.D)) {
            playerOz.action = new OzHuman.TestRowRight(playerOz);
        }

        if(Input.GetKeyDown(KeyCode.Space)) {
            playerOz.action = new OzHuman.TestHSwing1(playerOz);
        }

        { /* Camera */
            var camera = Camera.main;
            var p = playerOz.transform.position + playerOz.transform.rotation * new Vector3(0f, 0f, -10f) + new Vector3(0f, 2f, 0f);
            camera.transform.position = p;
            camera.transform.rotation = Quaternion.LookRotation(playerOz.transform.position - p, Vector3.up);
        }

        /* Beginning Game */
        if(Time.time - gameBeginningTime < 2f) {
            var image = fadeInPanel.GetComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 1f - (Time.time - gameBeginningTime) / 2f);
        }   

        /* Respawn */
        spawnTimer -= Time.deltaTime;
        if(spawnTimer < 0f) {
            SpawnRowingNPC();
            spawnTimer = 6f;
        }

        /* Ending Game */
        if(playerOz.lassitude) {
            FinishGame();
        }

        if(!float.IsNaN(gameEndTime))
        {
            GameObject[] prevTexts = GameObject.FindGameObjectsWithTag("NPC-talk");
            foreach (GameObject prevText in prevTexts)
            {
                DestroyImmediate(prevText);
            }

            if (Time.time - gameEndTime > 5f) {
                SceneManager.LoadScene("GameOverScene");
            } else {
                var progress1 = (Time.time - gameEndTime) / 5f;
                var progress2 = Mathf.Min(1f, (Time.time - gameEndTime) / 3f);
                var panel = GameObject.Find("GameOverPanel");
                var text = panel.transform.Find("Text").gameObject;
                var c = panel.GetComponent<Image>().color;
                panel.GetComponent<Image>().color = new Color(c.r, c.g, c.b, progress1);
                var d = text.GetComponent<Text>().color;
                text.GetComponent<Text>().color = new Color(d.r, d.g, d.b, progress2);
            }
        }
    }    // Start is called before the first frame update


    public void SpawnRowingNPC() {
        Vector3 p = new Vector3(Random.Range(-200f, 200f), 0.3f, Random.Range(-200f, 200f));
        Instantiate(ozAI, p, Quaternion.Euler(0, Random.Range(0f, 360f), 0));
    }


    
    public static void IncreaseKillCount() {
        killCount += 1;
        Debug.Log("Current Kill Count: " + killCount);
    }

    public static void RecordGameEnd() {
        gameEndTime = Time.time;
    }


    public void FinishGame() {
        if(float.IsNaN(gameEndTime)) {
            RecordGameEnd();

            Debug.Log("Game Over");
        }
    }
}
