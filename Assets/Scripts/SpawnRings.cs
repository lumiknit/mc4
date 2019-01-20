using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRings : MonoBehaviour
{
    public GameObject prefab;
    public float minRadius = 2;
    public float maxRadius = 50;
    public float minRingDistance = 2;
    public int maxLoopIteration = 70;
    public int count = 30;
    public List<Material> materials;

    [HideInInspector]
    public List<Vector3> positions;

    public void Spawn()
    {
        int materialsCount = materials.Count;
        positions = new List<Vector3>();
        int failure = 0;

        for (int i = 0; i < count; i++)
        {
            float r = Random.Range(minRadius, maxRadius);
            float theta = Random.Range(0, 360);

            Vector3 position = new Vector3(0, 0, 0);

            int loop = 0;
            while (loop < maxLoopIteration)
            {
                position = new Vector3(Mathf.Cos(theta) * r, 0, Mathf.Sin(theta) * r);
                bool success = true;
                for (int j = 0; j < i - failure; j++)
                {
                    if (Vector3.Distance(positions[j], position) < minRingDistance)
                    {
                        success = false;
                        break;
                    }
                }
                if (success)
                    break;
                loop++;
            }

            if (loop == maxLoopIteration)
            {
                failure++;
            }
            else
            {
                positions.Add(position);
                GameObject ring = Instantiate(prefab, position, Quaternion.Euler(90, 0, 0));

                int materialIndex = (int)Random.Range(0, materialsCount);
                ring.GetComponent<MeshRenderer>().material = materials[materialIndex];
            }
        }

        if (failure > 0)
        {
            Debug.Log("Failed to create " + failure + " rings.");
        }
    }
}
