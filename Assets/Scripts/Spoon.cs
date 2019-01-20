using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spoon : MonoBehaviour
{
    private Rigidbody rbody;

    private float rotateAngle;
    private float initHeight;

    private Vector3 vDir;
    private Vector3 aDir;

    public float rotationRate = 1f;
    public float velocityRate = 100f;
    public float accellerationRate = 30f;

    // Start is called before the first frame update
    void Start()
    {


        rbody = GetComponent<Rigidbody>();
        rotateAngle = transform.rotation.x;
        initHeight = transform.position.y;
        vDir = (2*transform.right + transform.forward).normalized;
        aDir = 2*transform.right.normalized;
        rbody.AddForce(vDir * velocityRate, ForceMode.VelocityChange);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rotateAngle -= (initHeight - transform.position.y) * Time.deltaTime * rotationRate;
        rbody.rotation = Quaternion.Euler(rotateAngle, 90, -90);
        rbody.AddForce(-aDir * Time.deltaTime * accellerationRate, ForceMode.VelocityChange);
        //Debug.Log(GetComponent<Transform>().position.y);
    }
}
