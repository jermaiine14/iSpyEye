using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerMovement : MonoBehaviour
{
    public float scrollSpeed;
    public float acceleration;

    // Update is called once per frame
    void Update()
    {
        scrollSpeed += acceleration * Time.deltaTime; // Increase speed over time
        
        transform.Translate(Vector3.left * scrollSpeed * Time.deltaTime);

        if (transform.position.x < -40f)
        {
            Destroy(gameObject);
        }
    }
}
