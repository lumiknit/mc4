using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OzArmature : MonoBehaviour
{
    GameObject human;
    OzHuman ozHuman;

    // Start is called before the first frame update
    void Start()
    {
        human = transform.parent.gameObject;
        ozHuman = human.GetComponent<OzHuman>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision) {
        /* Cascade impulse to the body */
        ozHuman.CascadeImpulse(-collision.impulse, collision.contacts[0].point);
    }
}
