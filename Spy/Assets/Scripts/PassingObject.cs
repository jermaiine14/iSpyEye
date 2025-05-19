using UnityEngine;
using System.Collections;

public class PassingObject : MonoBehaviour
{
    public float speed = 5f;
    public float lifetime = 15f;
    public float pauseDuration = 5f;

    private bool hasPaused = false;
    private bool isPaused = false;
    private Camera mainCam;

    [HideInInspector]
    public ScrollingBackground2 background;

    void Start()
    {
        mainCam = Camera.main;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (isPaused) return;

        transform.Translate(Vector3.left * speed * Time.deltaTime);

        if (!hasPaused)
        {
            Vector3 viewportPos = mainCam.WorldToViewportPoint(transform.position);
            if (viewportPos.x >= 0.49f && viewportPos.x <= 0.50f)
            {
                StartCoroutine(PauseMovement());
            }
        }
    }

    IEnumerator PauseMovement()
    {
        isPaused = true;
        hasPaused = true;

        if (background != null)
            background.SetPause(true);

        yield return new WaitForSeconds(pauseDuration);

        if (background != null)
        {
            background.SetPause(false);
            background.ResumeScroll(); // resume scroll with acceleration
        }

        isPaused = false;
    }
}
