using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LumiTestGameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q)) {
            var oz = GameObject.Find("human2a").GetComponent<OzHuman>();
            oz.action = new OzHuman.TestHSwing1(oz);
        }
        if(Input.GetKeyDown(KeyCode.D)) {
            var oz = GameObject.Find("human2a").GetComponent<OzHuman>();
            oz.action = new OzHuman.TestRow1(oz);
        }
        if(Input.GetKeyDown(KeyCode.W)) {
            var oz = GameObject.Find("human2a").GetComponent<OzHuman>();
            oz.action = new OzHuman.TestRow3(oz);
        }
        if(Input.GetKeyDown(KeyCode.A)) {
            var oz = GameObject.Find("human2a").GetComponent<OzHuman>();
            oz.action = new OzHuman.TestRow2(oz);
        }
        if(Input.GetKeyDown(KeyCode.Alpha1)) {
            var oz = GameObject.Find("human2a").GetComponent<OzHuman>();
            oz.action = new OzHuman.TestAction(oz);
        }
        if(Input.GetKeyDown(KeyCode.Z)) {
            GameObject.Find("human2a").GetComponent<OzHuman>().SetBoat(
                GameObject.Find("bigflake1")
            );
        }
        { /* Camera */
            var camera = Camera.main;
            var oz = GameObject.Find("spine1");
            var p = oz.transform.position + oz.transform.rotation * new Vector3(0f, 0f, -10f) + new Vector3(0f, 2f, 0f);
            camera.transform.position = p;
            camera.transform.rotation = Quaternion.LookRotation(oz.transform.position - p, Vector3.up);
        }
    }

}
