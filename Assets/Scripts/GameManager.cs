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
    public static int gameCount;
    public static int killCount;
    public static float gameBeginningTime;
    public static float gameEndTime;
    public static float gameDuration { get { return gameEndTime - gameBeginningTime; } }
    public static float maxSpeed;

    public static float spawnTimer;

    public static float bowlSize;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        bowlSize = 250;

        if(!PlayerPrefs.HasKey("gameCount")) {
            PlayerPrefs.SetInt("gameCount", 0);
        }
        gameCount = PlayerPrefs.GetInt("gameCount") + 1;
        PlayerPrefs.SetInt("gameCount", gameCount);

        killCount = 0;
        gameBeginningTime = Time.time;
        gameEndTime = float.NaN;
        maxSpeed = 0f;

        playerOz = GameObject.Find("Oz").transform.Find("human2a").GetComponent<OzHuman>();

        for(int i = 0; i < 30; i++) {
            SpawnRowingNPC();
        }

        GetComponent<SpawnRings>().Spawn();
        GetComponent<SpawnSpoon>().Spawn();

        fadeInPanel = GameObject.Find("FadeInPanel");
        var image = fadeInPanel.GetComponent<Image>();
        var text = fadeInPanel.transform.GetChild(0).GetComponent<Text>();
        image.color = new Color(0f, 0f, 0f, 1f);
        text.color = new Color(1f, 1f, 1f, 1f);
        text.text = "O#" + gameCount;
        spawnTimer = 0f;
    }

    Vector3 cameraTargetPosition = Vector3.zero;

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
        if(Input.GetKeyDown(KeyCode.K)) {
            playerOz.MakeLassitude();
        }

        if(Input.GetKeyDown(KeyCode.Space)) {
            playerOz.action = new OzHuman.TestHSwing1(playerOz);
        }

        { /* Camera */
            var camera = Camera.main;
            var pp = playerOz.transform.position + new Vector3(0f, 1f, 0f);
            var p = pp + (playerOz.transform.rotation * new Vector3(0f, 0f, -8f) + new Vector3(0f, 3f, 0f));
            camera.transform.position = camera.transform.position * 0.9f + p * 0.1f;
            camera.transform.rotation = Quaternion.LookRotation(
                pp - p, Vector3.up);
        }

        /* Beginning Game */
        if(Time.time - gameBeginningTime < 4f) {
            var image = fadeInPanel.GetComponent<Image>();
            var text = fadeInPanel.transform.GetChild(0).GetComponent<Text>();
            var progress = (Time.time - gameBeginningTime) / 2f;
            image.color = new Color(0f, 0f, 0f, 1f - progress);
            text.color = new Color(1f, 1f, 1f, 1.5f - progress);
        }   

        /* Respawn */
        spawnTimer -= Time.deltaTime;
        if(spawnTimer < 0f) {
            if(GameObject.FindGameObjectsWithTag("Enemy").Length < 30)
            SpawnRowingNPC();
            spawnTimer = 5f - Mathf.Max(3f, killCount * 0.02f);
        }

        bowlSize -= 0.008f;
        if(bowlSize < 50f) bowlSize = 50f;
        else {
            GameObject.Find("bowl").transform.localScale = new Vector3(bowlSize / 10f, bowlSize / 10f, 40);
        }

        /* Ending Game */
        if(playerOz.lassitude) {
            FinishGame();
        } else {
            var speed = playerOz.rigid.velocity.magnitude;
            var kmh = speed * 3.6f;
            maxSpeed = Mathf.Max(maxSpeed, speed);
            GameObject.Find("SpeedText").GetComponent<Text>().text = kmh.ToString("000.00") + " km/h";
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
        var r = Random.Range(0.05f, 0.6f) * bowlSize;
        var th = Random.Range(0f, 2 * Mathf.PI);
        Vector3 p = new Vector3(r * Mathf.Cos(th), 0.3f, r * Mathf.Sin(th));
        Instantiate(ozAI, p, Quaternion.Euler(0, Random.Range(0f, 360f), 0));
    }


    
    public static void IncreaseKillCount() {
        killCount += 1;
        Debug.Log("Current Kill Count: " + killCount);
    }

    public static void RecordGameEnd() {
        gameEndTime = Time.time;
        PlayerPrefs.SetInt("gameCount", gameCount + killCount + 1);
    }


    public void FinishGame() {
        if(float.IsNaN(gameEndTime)) {
            RecordGameEnd();

            Debug.Log("Game Over");
        }
    }
}
