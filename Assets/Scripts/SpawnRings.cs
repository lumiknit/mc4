using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRings : MonoBehaviour
{
    public GameObject ringPrefab;
    public GameObject npcPrefab;
    [Min(0.0f)]
    public float minRadius = 10;
    [Min(0.0f)]
    public float maxRadius = 90;
    [Min(0.0f)]
    public float minRingDistance = 2;
    [Range(0, 1)]
    public float npcSpawnRate = 0.75f;
    [Min(0)]
    public int maxLoopIteration = 70;
    [Min(0)]
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
            Vector3 position = new Vector3(0, 0, 0);

            int loop = 0;
            while (loop < maxLoopIteration)
            {
                float r = Random.Range(minRadius, maxRadius);
                float theta = Random.Range(0f, 360f);

                float x = r * Mathf.Cos(theta);
                float z = r * Mathf.Sin(theta);

                position = new Vector3(x, 0, z);
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
                GameObject ring = Instantiate(ringPrefab, position, Quaternion.Euler(90, 0, 0));
                int materialIndex = (int)Random.Range(0, materialsCount);
                ring.GetComponent<MeshRenderer>().material = materials[materialIndex];

                if (Random.Range(0f, 1f) > (1f - npcSpawnRate))
                {
                    Vector3 npcPosition = position - new Vector3(0, 1, 0);
                    GameObject npc = Instantiate(npcPrefab, npcPosition, Quaternion.identity);
                    Physics.IgnoreCollision(ring.transform.GetComponent<Collider>(), npc.transform.GetComponent<Collider>());
                }
            }
        }

        if (failure > 0)
        {
            Debug.Log("Failed to create " + failure + " rings.");
        }
    }
}
