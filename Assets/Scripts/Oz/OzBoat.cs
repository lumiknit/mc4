using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OzBoat : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision) {
        if(collision.contactCount > 0) {
            transform.parent.Find("human2a").GetComponent<Rigidbody>().AddForceAtPosition(collision.impulse, collision.contacts[0].point, ForceMode.Impulse);
        }
    }
}
