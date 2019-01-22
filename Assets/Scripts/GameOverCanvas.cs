using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverCanvas : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var canvas = GameObject.Find("Canvas");
        var text = canvas.transform.Find("StatisticText").gameObject;
        string stat = "";
        stat += GameManager.killCount + " Os were zapped\n";
        stat += "You played " + GameManager.gameDuration.ToString("0.0") + " seconds\n";
        stat += "Your max speed was " + (GameManager.maxSpeed * 3.6).ToString("0.0") + " km/h\n";
        text.GetComponent<Text>().text = stat;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnRestartButtonClicked() {
        SceneManager.LoadScene("GameScene");
    }
}
