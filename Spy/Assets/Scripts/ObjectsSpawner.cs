using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject Cloud; 
    public GameObject Flower; 
    public GameObject Tree;

    [Header("Layers")]
    public string skyLayerName = "Sky"; // Ensure this layer exists
    public string groundLayerName = "Ground"; // Ensure this layer exists

    [Header("Spawn Area Range")]
    public float minSpawnX = -15f; // Minimum X coordinate
    public float spawnZ = 0f;
    private int skyLayer;
    private int groundLayer;

    [Header("Cloud Spawn Range")]
    public float cloudMinY = 3f;
    public float cloudMaxY = 10f;
    public float maxCloudScale = 3f; // Maximum scale for the flower

    [Header("Flower Spawn Range")]
    public float flowerMinY = -15f;
    public float flowerMaxY = -8f;
    public float maxFlowerScale = 3f; // Maximum scale for the flower

    [Header("Tree Spawn Range")]
    public float treeMinY = -14f;
    public float treeMaxY = -5f;
    public float maxTreeScale = 2.5f; // Maximum scale for the tree
    private Camera mainCamera;
    private float scaleTimer = 0f; // Timer to track how long the key is held
    private GameObject currentInstance = null; // Reference to the currently spawned object 

    

    void Start()
    {
        mainCamera = Camera.main; // Gets the camera tagged "MainCamera"
        if (mainCamera == null)
        {
            Debug.LogError("ObjectSpawner requires a Camera tagged 'MainCamera' in the scene.");
            this.enabled = false; // Disable the script if there's no main camera
            return;
        }

        // Convert layer names to integer IDs for performance
        skyLayer = LayerMask.NameToLayer(skyLayerName);
        groundLayer = LayerMask.NameToLayer(groundLayerName);

        // Error checking if layers don't exist
        if (skyLayer == -1) // LayerMask.NameToLayer returns -1 if the layer is not found
        {
            Debug.LogError($"Layer '{skyLayerName}' not found. Please ensure it exists in Project Settings -> Tags and Layers.");
        }
        if (groundLayer == -1)
        {
            Debug.LogError($"Layer '{groundLayerName}' not found. Please ensure it exists in Project Settings -> Tags and Layers.");
        }
    }
    /**
     * Comment Update() out when change the keyboard input to Arduino input
     */
   void Update()
    {   
        minSpawnX = mainCamera.transform.position.x - 25f; // Update minSpawnX based on camera position
        // Check if the '1' key is pressed or held
        if (Input.GetKey(KeyCode.Alpha1))
        {
            SpawnCloud(); // Call the SpawnCloud function
        }

        else if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            ResetState();
        }

        if (Input.GetKey(KeyCode.Alpha2))
        {
            SpawnFlower(); // Call the SpawnFlower function
        }
        else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
           ResetState();
        }
        if (Input.GetKey(KeyCode.Alpha3))
        {
            SpawnTree(); // Call the SpawnTree function
        }
        else if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            ResetState();
        }
    }    
    // Function to handle the actual spawning
    GameObject SpawnObject(GameObject prefab, int targetLayer)
    {
        Vector3 spawnPosition = new Vector3(28f, 0, spawnZ); // Create position vector
        // If getting the position failed, don't spawn
        if (spawnPosition == Vector3.negativeInfinity) return null;
        
        if (prefab == Cloud)
        {
            spawnPosition.y = Random.Range(cloudMinY, cloudMaxY); // Get random Y within range
        }    
        else if (prefab == Flower)
        {
            spawnPosition.y = Random.Range(flowerMinY, flowerMaxY); // Get random Y within range
        }
        else if (prefab == Tree)
        {
            spawnPosition.y = Random.Range(treeMinY, treeMaxY); 
        }

        // Create a new instance of the prefab at the calculated position with no rotation
        GameObject newInstance = Instantiate(prefab, spawnPosition, Quaternion.identity);

        // --- Set Layer ---
        // Assign the new object (and its children) to the target layer
        SetLayerRecursively(newInstance, targetLayer);
        SetSortingLayer(newInstance, targetLayer); // Set the sorting layer for the new object

        Debug.Log($"Spawned {newInstance.name} on layer '{LayerMask.LayerToName(targetLayer)}'");
        return newInstance; // Return the spawned object for further manipulation if needed
    }

    public void SpawnCloud()
    {
        if (currentInstance == null && Cloud != null && skyLayer != -1)
        {
            currentInstance = SpawnObject(Cloud, skyLayer);
            currentInstance.transform.localScale = Vector3.one; // Reset scale to 1 when spawning a new object
            scaleTimer = 0f; // Reset the scale timer when spawning a new object
        }
        else if (currentInstance != null)
        {
            scaleTimer += Time.deltaTime; // Increment the timer
            float newScale = 1f + scaleTimer; // Calculate the new scale
            if (newScale > maxCloudScale)
            {
                newScale = maxCloudScale; // Set to max scale if exceeded
                Debug.Log("Max scale reached for Cloud!"); // Debug message
            }
            currentInstance.transform.localScale = Vector3.one * newScale; // Apply the scale
        }
    }

    public void SpawnFlower()
    {
        if (currentInstance == null && Flower != null && groundLayer != -1)
        {
            currentInstance = SpawnObject(Flower, groundLayer);
            currentInstance.transform.localScale = Vector3.one; // Reset scale to 1 when spawning a new object
            scaleTimer = 0f; // Reset the scale timer when spawning a new object
        }
        else if (currentInstance != null)
        {
            scaleTimer += Time.deltaTime; // Increment the timer
            float newScale = 1f + scaleTimer; // Calculate the new scale

            if (newScale > maxFlowerScale) // Limit the scale to a maximum of 1.5
            {
                newScale = maxFlowerScale; // Set to max scale if exceeded
                Debug.Log("Max scale reached for Flower!"); // Debug message
            }
            currentInstance.transform.localScale = Vector3.one * newScale; // Apply the scale
        }
    }

    public void SpawnTree()
    {
        if (currentInstance == null && Tree != null && groundLayer != -1)
        {
            currentInstance = SpawnObject(Tree, groundLayer);
            currentInstance.transform.localScale = Vector3.one; // Reset scale to 1 when spawning a new object
            scaleTimer = 0f; // Reset the scale timer when spawning a new object
        }
        else if (currentInstance != null)
        {
            scaleTimer += Time.deltaTime; // Increment the timer
            float newScale = 1f + scaleTimer; // Calculate the new scale

            if (newScale > maxTreeScale) // Limit the scale to a maximum of 1.5
            {
                newScale = maxTreeScale; // Set to max scale if exceeded
                Debug.Log("Max scale reached for Tree!"); // Debug message
            }
            currentInstance.transform.localScale = Vector3.one * newScale; // Apply the scale
        }
    }
    // Helper function to set the layer for the spawned object and all its children
    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null) return;

        obj.layer = newLayer;

        // Recursively apply to all child objects
        foreach (Transform child in obj.transform)
        {
            if (child != null)
            {
                SetLayerRecursively(child.gameObject, newLayer);
            }
        }
    }
    void SetSortingLayer(GameObject obj, int targetLayer)
    {
        if (obj == null) return;

        // Determine the sorting layer name based on the target layer
        string sortingLayerName = LayerMask.LayerToName(targetLayer);

        // Get the Renderer component (e.g., SpriteRenderer) and set the sorting layer
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.sortingLayerName = sortingLayerName; // Set the sorting layer name
        }

        // Recursively apply to all child objects
        foreach (Transform child in obj.transform)
        {
            if (child != null)
            {
                SetSortingLayer(child.gameObject, targetLayer);
            }
        }
    }
    public void ResetState()
    {// Reset the state of the spawner
        scaleTimer = 0f; // Reset the scale timer
        currentInstance = null; // Clear the reference to the current instance
    }
}
