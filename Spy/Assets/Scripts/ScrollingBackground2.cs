using System.Collections;
using UnityEngine;

public class ScrollingBackground2 : MonoBehaviour
{
    [Header("Background Scrolling")]
    private float length, startPosition;
    public GameObject cam;
    public float parallaxEffect;
    public float scrollSpeed = 2f;
    public float maxScrollSpeed = 2f; // Target speed when accelerating
    private float currentScrollSpeed;

    public bool isPaused = false;
    public bool ObjectSpawned = false;

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

        currentScrollSpeed = scrollSpeed;
    }

    void Update()
    {
        if (isPaused) return;

        // Background scrolling
        startPosition -= currentScrollSpeed * Time.deltaTime;
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
            ObjectSpawned = true;
            Vector3 spawnPos = cam.transform.position + spawnOffset;
            GameObject obj = Instantiate(passingObjectPrefab, spawnPos, Quaternion.identity);

            PassingObject po = obj.GetComponent<PassingObject>();
po.background = this;

            // Begin slowing down scroll when object spawns
            StartCoroutine(SmoothScrollSpeed(0f, 10f)); // 1.5 seconds to slow down
        }
    }

    public void ResumeScroll()
    {
        ObjectSpawned = false;
        StartCoroutine(SmoothScrollSpeed(maxScrollSpeed, 3f)); // 1.5 seconds to speed up
    }

    public void SetPause(bool pause)
    {
        isPaused = pause;
    }

    IEnumerator SmoothScrollSpeed(float targetSpeed, float duration)
    {
        float initialSpeed = currentScrollSpeed;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            currentScrollSpeed = Mathf.Lerp(initialSpeed, targetSpeed, time / duration);
            yield return null;
        }

        currentScrollSpeed = targetSpeed;
    }
}
