using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerMovement : MonoBehaviour
{
    
    public float scrollSpeed;
    public float acceleration;
    private float mainSpeed;
    private float zeroSpeed = 0f;
    private ScrollingBackground2 SB3;

    void Start()
{
    SB3 = FindObjectOfType<ScrollingBackground2>();
}
    // Update is called once per frame
    void Update()
    {
        if(SB3.isPaused == true){
            mainSpeed = zeroSpeed;
        }
        else {
            mainSpeed = scrollSpeed;
        }
        mainSpeed += acceleration * Time.deltaTime; // Increase speed over time
        
        transform.Translate(Vector3.left * mainSpeed * Time.deltaTime);

        if (transform.position.x < -40f)
        {
            // Instead of destroying, hand off to the collector:
            if (StickerCollector.Instance != null)
            {
                StickerCollector.Instance.AddSticker(this.gameObject);
            }
            // Remove this movement component so it stops drifting.
            Destroy(this);
        }
    }
}
