using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NpcInteraction : MonoBehaviour
{
    public float distanceThreshold = 10f;
    public Font font;

    private GameObject player;
    private GameObject canvas;
    private bool hasInteracted;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        canvas = GameObject.FindGameObjectWithTag("Canvas");
        hasInteracted = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
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

    void LookAtPlayer(Vector3 playerPos, Vector3 npcPos)
    {
        Vector3 relativePos = (playerPos - npcPos).normalized;
        relativePos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f);
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
}
