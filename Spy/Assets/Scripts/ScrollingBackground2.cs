using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBackground2 : MonoBehaviour
{
    [Header("Background Scrolling")]
    private float length, startPosition;
    public GameObject cam;
    public float parallaxEffect;
    public float scrollSpeed = 2f;

    private bool isPaused = false;

    [Header("Passing Object")]
    public GameObject passingObjectPrefab;
    public float passingSpeed = 5f;
    public float spawnInterval = 5f;
    public Vector3 spawnOffset = new Vector3(10f, 0f, 0f);

    private float timer = 0f;

    void Start()
    {
        startPosition = transform.position.x;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        length = sr ? sr.bounds.size.x : 10;
    }

    void Update()
    {
        if (isPaused) return;

        // Background scrolling
        startPosition -= scrollSpeed * Time.deltaTime;
        float temp = cam.transform.position.x * (1 - parallaxEffect);
        float distance = cam.transform.position.x * parallaxEffect;
        transform.position = new Vector3(startPosition + distance, transform.position.y, transform.position.z);

        if (temp > startPosition + length) startPosition += length;
        else if (temp < startPosition - length) startPosition -= length;

        // Object spawning
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnPassingObject();
        }
    }

    void SpawnPassingObject()
    {
        if (passingObjectPrefab != null)
        {
            Vector3 spawnPos = cam.transform.position + spawnOffset;
            GameObject obj = Instantiate(passingObjectPrefab, spawnPos, Quaternion.identity);

            // Pass a reference to this background to the passing object
            PassingObject po = obj.AddComponent<PassingObject>();
            po.speed = passingSpeed;
            po.background = this; // << pass reference
        }
    }

    // Called by the passing object to pause/resume
    public void SetPause(bool pause)
    {
        isPaused = pause;
    }
}
