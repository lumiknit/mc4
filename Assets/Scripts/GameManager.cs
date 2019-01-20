using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject spoonPrefab;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpawnRings>().Spawn();
        InvokeRepeating("LaunchSpoon", 0.0f, 5.0f);
    }

    void LaunchSpoon()
    {
        Vector3 position = new Vector3(-70, 65, 0);
        GameObject spoon = Instantiate(spoonPrefab, position, Quaternion.Euler(0, 90, -90));
        //spoon.GetComponent<Transform>().localScale = new Vector3(3, 3, 3);
        Destroy(spoon, 5.0f);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
