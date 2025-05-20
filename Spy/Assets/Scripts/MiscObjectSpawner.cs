using System.Collections;
using UnityEngine;

public class MiscObjectSpawner : MonoBehaviour
{
     [System.Serializable]
    public class SpawnEntry
    {
        public GameObject prefab;
        public Vector3 spawnPosition;
    }

    [Header("Spawn Settings")]
    public SpawnEntry[] spawnEntries = new SpawnEntry[3];
    public float spawnInterval = 10f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnRandomPrefab();
        }
    }

    void SpawnRandomPrefab()
    {
        if (spawnEntries.Length == 0) return;

        int index = Random.Range(0, spawnEntries.Length);
        SpawnEntry entry = spawnEntries[index];

        if (entry.prefab != null)
        {
            Instantiate(entry.prefab, entry.spawnPosition, Quaternion.identity);
        }
    }
}