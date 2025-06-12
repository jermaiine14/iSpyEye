using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sticker : MonoBehaviour
{
    public static Sticker Instance;

    public GameObject SlagboomSticker, TreeSticker, MolenSticker, KoeSticker, FlowerSticker;
    public Transform scoreboardParent;

    private List<GameObject> spawnedStickers = new List<GameObject>();
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("StickerCollector attached to: " + gameObject.name);
        }
    }

    public void SnapAllToBoard()
    {
        Dictionary<int, int> counts = FindObjectOfType<ObjectSpawner>().GetSpawnCounts();
        var spawner = FindObjectOfType<ObjectSpawner>();
        Debug.Log($"ObjectSpawner found: {spawner?.gameObject.name}");

        ClearPreviousStickers();

        for (int index = 0; index < 5; index++)
        {
            GameObject prefab = GetPrefabByIndex(index);
            int count = counts.ContainsKey(index) ? counts[index] : 0;

            Debug.Log($"Spawning {count} sticker(s) of type {index}");

            for (int i = 0; i < count; i++)
            {
                GameObject sticker = Instantiate(prefab, scoreboardParent);
                sticker.transform.localPosition = GetStickerPosition(index, i);
                sticker.transform.localScale = Vector3.one * 0.8f; // Reset scale to 1
                sticker.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-10f, 10f));// Reset rotation
                spawnedStickers.Add(sticker);
            }
        }

    }

    Vector3 GetStickerPosition(int index, int order)
    {
        // Customize as needed per index (type) and spacing
        // Area size (in local space of Scoreboard)
        float width = 10f;
        float height = 11f;

        float x = Random.Range(-width / 2f, width / 2f);
        float y = Random.Range(-height / 2f, height / 2f);

        return new Vector3(x, y, 0);
    }

    GameObject GetPrefabByIndex(int index)
    {
        return index switch
        {
            0 => KoeSticker,
            1 => SlagboomSticker,
            2 => FlowerSticker,
            3 => MolenSticker,
            4 => TreeSticker,
            _ => null
        };
    }

    public void ClearPreviousStickers()
    {
        foreach (var s in spawnedStickers)
            Destroy(s);
        spawnedStickers.Clear();
    }

    public void ResetCounts()
    {
        FindObjectOfType<ObjectSpawner>().ResetSpawnCounts(); // Add this function to ObjectSpawner
    }

    void OnDestroy()
    {
        ClearPreviousStickers();
    }

}
