using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    OzHuman playerOz;

    public GameObject ozAI;

    // Start is called before the first frame update
    void Start()
    {
        playerOz = GameObject.Find("Oz").transform.Find("human2a").GetComponent<OzHuman>();

        for(int i = 0; i < 30; i++) {
            Vector3 p = new Vector3(Random.Range(-200f, 200f), 0.3f, Random.Range(-200f, 200f));
            Instantiate(ozAI, p, Quaternion.Euler(0, Random.Range(0f, 360f), 0));
        }

        GetComponent<SpawnRings>().Spawn();
        GetComponent<SpawnSpoon>().Spawn();
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
    }    // Start is called before the first frame update
}
