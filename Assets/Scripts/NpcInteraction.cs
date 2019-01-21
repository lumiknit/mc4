using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NpcInteraction : MonoBehaviour
{
    enum NpcType { Kill, Playtime, Position, EnemyCount, Normal, Crazy, Count};

    public float distanceThreshold = 30f;
    public GameObject canvasPrefab;

    private GameObject player;
    private GameObject canvas;
    private bool hasInteracted;

    bool dead;

    private NpcType npcType;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform.Find("human2a").gameObject;
        hasInteracted = false;
        dead = false;

        npcType = (NpcType)Random.Range(0, (int)NpcType.Count);
        npcType = NpcType.Position;
    }

    // Update is called once per frame
    void Update()
    {
        if(dead) {
            if(transform.position.y < -20) {
                GameManager.IncreaseKillCount();
                Destroy(gameObject);
            }
        } else {
            Vector3 playerPos = player.transform.position;
            Vector3 npcPos = transform.position;
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

        GameObject panelObject = Instantiate(canvasPrefab);
        panelObject.transform.tag = "NPC-talk";

        string npcText = "Hello world!";

        switch(npcType)
        {
            case NpcType.Kill:
                break;
            case NpcType.Playtime:
                break;
            case NpcType.Position:
                float radius = transform.position.magnitude;
                float percent = 100f * radius / 474;
                npcText = "여기는 중심으로부터 " + percent.ToString().Substring(0, 4) + "%...";
                break;
            case NpcType.EnemyCount:
                break;
            case NpcType.Normal:
                npcText = "안녕!";
                break;
            case NpcType.Crazy:
                npcText = "뭘 봐? 뒤질래?";
                break;
            default:
                Debug.Log("NPC type error!");
                break;
        }

        panelObject.transform.Find("Text").GetComponent<Text>().text = npcText;

        Destroy(panelObject, 5.0f);
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
