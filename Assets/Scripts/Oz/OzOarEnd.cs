using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OzOarEnd : MonoBehaviour
{
    public Vector3 lastPosition;
    public Vector3 velocity;
    
    // Start is called before the first frame update
    void Start()
    {
        lastPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        velocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;
    }
}
