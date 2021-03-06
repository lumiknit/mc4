﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSpoon : MonoBehaviour
{
    public GameObject spoonPrefab;
    [Min(0.0f)]
    public float startTime = 5.0f;
    [Min(0.0f)]
    public float spawnInterval = 10.0f;
    [Min(0.0f)]
    public float destroyInterval = 10.0f;
    [Min(0.0f)]
    public float minRadius = 10;
    [Min(0.0f)]
    public float maxRadius = 50;

    public void Spawn()
    {
        InvokeRepeating("LaunchSpoon", startTime, spawnInterval);
    }

    void LaunchSpoon()
    {
        // -70

        GameObject spoon = Instantiate(spoonPrefab);
        //spoon.GetComponent<Transform>().localScale = new Vector3(3, 3, 3);
        Destroy(spoon, destroyInterval);
    }
}
