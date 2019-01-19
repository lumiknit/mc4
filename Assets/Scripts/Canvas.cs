using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canvas : MonoBehaviour
{
    public GameObject toast;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    public void MakeToast(string message, float duration) {
        GameObject t = Instantiate(toast);
        t.transform.SetParent(transform, false);
        Toast ts = t.GetComponent<Toast>();
        ts.message = message;
        ts.duration = duration;
        Debug.Log("Toast(" + duration + "): " + message);
    }
}
