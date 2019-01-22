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
    public float velocityRate = 24f;
    public float accellerationRate = 8f;

    private float rotation;

    // Start is called before the first frame update
    void Start()
    {
        rotation = Random.Range(0f, 360f);

        float r = Random.Range(10, 50);
        float theta = Random.Range(0f, 360f);

        float x = r * Mathf.Cos(theta);
        float z = r * Mathf.Sin(theta);

        x = z = 0;
        
        Vector3 position = new Vector3(x, 43f, z);

        Vector3 direction = Quaternion.Euler(0, rotation, 0) * new Vector3(0, 0, 430f);
        position -= direction;

        transform.position = position;
        transform.rotation = Quaternion.Euler(0, rotation, -90);


        rbody = GetComponent<Rigidbody>();
        rotateAngle = transform.rotation.x;
        initHeight = transform.position.y;

        Vector3 downVec = -Vector3.up;
        Vector3 moveVec = 5*(Quaternion.Euler(0, rotation-90f, 0) * Vector3.right);

        vDir = downVec + moveVec;
        aDir = downVec;
        rbody.AddForce(vDir * velocityRate, ForceMode.VelocityChange);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (rotateAngle > -90)
            rotateAngle -= (initHeight - transform.position.y) * Time.fixedDeltaTime * rotationRate;
        rbody.rotation = Quaternion.Euler(rotateAngle, rotation, -90);
        rbody.AddForce(-aDir * Time.fixedDeltaTime * accellerationRate, ForceMode.VelocityChange);
        Debug.Log(rbody.position);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Ring" ||
            collision.transform.tag == "NPC" ||
            collision.transform.tag == "Player" ||
            collision.transform.tag == "Enemy")
        {
            Vector3 offset = (collision.transform.position - transform.position);
            
            collision.transform.parent = transform;
            offset = offset.normalized * Mathf.Sqrt(3);
            collision.transform.position = transform.position + offset;
            collision.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            FixedJoint fixedJoint = collision.gameObject.AddComponent<FixedJoint>();
            fixedJoint.connectedBody = rbody;
        }
    }
}
