using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject Cloud;
    public GameObject Flower;
    public GameObject Tree;
    public GameObject Molen;
    public GameObject Koe;

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
    // scaleTimer = 0f; // Timer to track how long the key is held
    [Header("Molen Spawn Range")]
    public float molenMinY = 3f;
    public float molenMaxY = 10f;
    public float maxMolenScale = 3f; // Maximum scale for the flower

    [Header("Koe Spawn Range")]
    public float koeMinY = 3f;
    public float koeMaxY = 10f;
    public float maxKoeScale = 3f; // Maximum scale for the flower


    private Dictionary<GameObject, float> scaleTimers = new Dictionary<GameObject, float>();
    private Dictionary<int, List<GameObject>> activeInstances = new Dictionary<int, List<GameObject>>();    private GameObject currentInstance = null; // Reference to the currently spawned object 
    private Dictionary<int, GameObject> growingInstance = new Dictionary<int, GameObject>();

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
        minSpawnX = mainCamera.transform.position.x - 25f;

        // Map number keys to button indices (0 to 4)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SpawnObjectByIndex(0);
        }
        if (Input.GetKey(KeyCode.Alpha1))
        {
            GrowObject(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SpawnObjectByIndex(1);
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            GrowObject(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SpawnObjectByIndex(2);
        }
        if (Input.GetKey(KeyCode.Alpha3))
        {
            GrowObject(2);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SpawnObjectByIndex(3);
        }
        if (Input.GetKey(KeyCode.Alpha4))
        {
            GrowObject(3);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SpawnObjectByIndex(4);
        }
        if (Input.GetKey(KeyCode.Alpha5))
        {
            GrowObject(4);
        }
    }
    public void SpawnObjectByIndex(int index)
    {
        GameObject prefab = null;
        float minY = 0f, maxY = 0f;

        switch (index)
        {
            case 0: prefab = Koe; minY = koeMinY; maxY = koeMaxY; break;
            case 1: prefab = Cloud; minY = cloudMinY; maxY = cloudMaxY; break;
            case 2: prefab = Flower; minY = flowerMinY; maxY = flowerMaxY; break;
            case 3: prefab = Molen; minY = molenMinY; maxY = molenMaxY; break;
            case 4: prefab = Tree; minY = treeMinY; maxY = treeMaxY; break;
        }

        Vector3 spawnPos = new Vector3(28f, Random.Range(minY, maxY), spawnZ);
        GameObject instance = Instantiate(prefab, spawnPos, Quaternion.identity);

        SetLayerRecursively(instance, groundLayer);
        SetSortingLayer(instance, groundLayer);

        // Add to the list for this index
        if (!activeInstances.ContainsKey(index))
            activeInstances[index] = new List<GameObject>();
        activeInstances[index].Add(instance);

        // Optionally, you can track scaleTimers for each instance if needed
        // For now, just set initial scale
        float scaleBase = 1f;
        if (index == 2) scaleBase = 0.5f; // Flower starts at 0.5
        instance.transform.localScale = Vector3.one * scaleBase;
        scaleTimers[instance] = 0f;

        // Set this as the currently growing object for this type
        growingInstance[index] = instance;
    }

    public void GrowObject(int index)
    {
        if (!growingInstance.ContainsKey(index)) return;

        GameObject obj = growingInstance[index];
        if (obj == null)
        {
            growingInstance.Remove(index);
            return;
        }

        // Update timer
        scaleTimers[obj] += Time.deltaTime;

        float scaleBase = 1f;
        float maxScale = 3f;

        switch (index)
        {
            case 0: maxScale = maxKoeScale; break;
            case 1: maxScale = maxCloudScale; break;
            case 2: scaleBase = 0.5f; maxScale = maxFlowerScale; break;
            case 3: maxScale = maxMolenScale; break;
            case 4: maxScale = maxTreeScale; break;
        }

        float newScale = Mathf.Min(scaleBase + scaleTimers[obj], maxScale);
        obj.transform.localScale = Vector3.one * newScale;
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

}