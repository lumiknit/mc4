using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Toast : MonoBehaviour
{
    public string message;

    public float duration;
    private float time;

    // Start is called before the first frame update
    void Start()
    {
        transform.GetChild(0).GetComponent<Text>().text = message;
        time = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time >= time + duration) Destroy(this);
    }

    public static void Make(string message, float duration = 0.5f) {
        GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
        canvas.GetComponent<Canvas>().MakeToast(message, duration);
    }
}
