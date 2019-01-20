using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingObject : MonoBehaviour
{
    private bool attatched = false;
    private Vector3 offset;
    private GameObject collideObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (attatched)
        {
            transform.position = collideObject.transform.position + offset;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Spoon")
        {
            collideObject = collision.gameObject;
            transform.parent = collision.transform;
            offset = (collision.transform.position - transform.position).normalized * 2;
            attatched = true;
        }
    }
}
