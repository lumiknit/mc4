using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NpcInteraction : MonoBehaviour
{
    public float distanceThreshold = 30f;
    public Font font;

    private GameObject player;
    private GameObject canvas;
    private bool hasInteracted;

    bool dead;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform.Find("human2a").gameObject;
        canvas = GameObject.FindGameObjectWithTag("Canvas");
        hasInteracted = false;
        dead = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!dead) {
            Vector3 playerPos = player.transform.position;
            Vector3 npcPos = transform.position;
            Debug.Log("Distance = " + (playerPos - npcPos).magnitude);
            if (Vector3.Distance(playerPos, npcPos) < distanceThreshold)
            {
                LookAtPlayer(playerPos, npcPos);
                InteractPlayer();
            }
            else
            {
                hasInteracted = false;
            }
        }
    }

    void LookAtPlayer(Vector3 playerPos, Vector3 npcPos)
    {
        Vector3 relativePos = (playerPos - npcPos).normalized;
        relativePos.y = 0;
        var fixedJoint = GetComponent<FixedJoint>();
        Rigidbody ringRigid = null;
        Vector3 anchor = Vector3.zero;
        if(fixedJoint != null) {
            ringRigid = fixedJoint.connectedBody;
            anchor = fixedJoint.anchor;
            DestroyImmediate(fixedJoint);
            GetComponent<Rigidbody>().useGravity = false;
        }
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f);
        if(ringRigid != null) {
            var newFixedJoint = gameObject.AddComponent<FixedJoint>();
            newFixedJoint.connectedBody = ringRigid;
            newFixedJoint.anchor = anchor;
            GetComponent<Rigidbody>().useGravity = true;
        }
    }

    void InteractPlayer()
    {
        if (hasInteracted)
            return;

        hasInteracted = true;
        Debug.Log("Hi!");

        GameObject[] prevTexts = GameObject.FindGameObjectsWithTag("NPC-talk");
        foreach (GameObject prevText in prevTexts)
        {
            DestroyImmediate(prevText);
        }

        GameObject textObject = new GameObject("text", typeof(RectTransform));
        textObject.transform.tag = "NPC-talk";
        textObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(66.5f, 31f, 0f);

        Text text = textObject.AddComponent<Text>();

        text.text = Random.Range(0f, 10f).ToString() + ": Hello world!";
        text.font = font;
        text.fontSize = 50;
        text.material = font.material;
        text.color = Color.white;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;

        textObject.transform.SetParent(canvas.transform);

        Destroy(textObject, 5.0f);
    }

    void OnCollisionEnter(Collision collision) {
        if(collision.impulse.magnitude > 10f) {
            var fixedJoint = GetComponent<FixedJoint>();
            if(fixedJoint) {
                Destroy(fixedJoint);
                GetComponent<Rigidbody>().useGravity = true;
            }
            dead = true;
            GetComponent<Rigidbody>().AddForceAtPosition(collision.impulse, collision.contacts[0].point, ForceMode.Impulse);
        }
    }
}
