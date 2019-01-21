using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OzOar : MonoBehaviour
{
    GameObject endpiece;
    Rigidbody rigid;
    OzOarEnd oarEnd;

    // Start is called before the first frame update
    void Start()
    {
        endpiece = transform.Find("Endpiece").gameObject;
        oarEnd = endpiece.GetComponent<OzOarEnd>();
        rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnCollisionEnter(Collision collision) {
        if(rigid && !rigid.useGravity) {
            var dir = new Vector3(0, 4f, 0) + oarEnd.velocity;
            if(collision.rigidbody != null) {
                collision.rigidbody.AddForceAtPosition(dir * 5, collision.contacts[0].point, ForceMode.Impulse);
            }
        }
    }
}
