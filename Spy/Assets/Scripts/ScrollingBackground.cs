using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    private float length, startPosition;
    public GameObject cam;
    public float parallaxEffect;
    public float scrollSpeed = 2f;

    void Start()
    {
        startPosition = transform.position.x;
        //length = GetComponent<SpriteRenderer>().bounds.size.x;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            length = sr.bounds.size.x;
        }
        else
        {
            Debug.LogError("ScrollingBackground requires a SpriteRenderer component.");
            length = 10; // Assign a default length if no SpriteRenderer found to avoid errors
        }
    }
    /**
    void Update()
    {
        float offset = Mathf.Repeat(Time.time * scrollSpeed, length);

        transform.position = new Vector3(startPosition - offset, transform.position.y, transform.position.z);
    }**/
    void Update()
    {   
        startPosition -= scrollSpeed * Time.deltaTime;
        float temp = (cam.transform.position.x * (1 - parallaxEffect));
        float distance = (cam.transform.position.x * parallaxEffect);

        transform.position = new Vector3(startPosition + distance, transform.position.y, transform.position.z);

        // Reset the background position when it moves out of view
        if (temp > startPosition + length) startPosition += length;
        else if (temp < startPosition - length) startPosition -= length;
    }
}

