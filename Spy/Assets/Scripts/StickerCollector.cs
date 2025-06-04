using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A singleton that collects all stickers right before they would be destroyed
/// and, upon station arrival, repositions them in a 15×20 world‐space “Information Board.”
/// </summary>

public class StickerCollector : MonoBehaviour
{
    [Header("Handmatige Bord-aanpassing")]
    [Tooltip("Met deze factor wordt de breedte van het bord gedeeld. 1 = volle breedte, 2 = halve breedte, etc.")]
    public float widthFactor = 1f;

    [Tooltip("Met deze factor wordt de hoogte van het bord gedeeld. 1 = volle hoogte, 2 = halve hoogte, etc.")]
    public float heightFactor = 1f;

    // ——— SINGLETON SETUP ———
    public static StickerCollector Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ——— FIELDS ———

    /// <summary>
    /// All stickers that have gone off‐screen and are awaiting placement on the board.
    /// We preserve spawn order by adding to this List in the order they go off‐screen.
    /// </summary>
    private List<GameObject> collectedStickers = new List<GameObject>();

    /// <summary>
    /// Reference to the Information Board’s Transform (world-space). 
    /// Assign this in the Inspector to the “Information Board” sprite (which is exactly 15×20 units).
    /// </summary>
    [Tooltip("Drag your 15×20 InformationBoard sprite here (its Transform).")]
    public Transform stationBoardTransform;

    /// <summary>
    /// The world-space width and height of the board (in Unity units).
    /// These must match your board’s actual size (e.g. a 15×20 sprite scaled 1,1,1).
    /// </summary>
    //private float boardWidth = 15f;
    //private float boardHeight = 20f;

    /// <summary>
    /// Minimum cell size (stickers will never shrink below 2×2 units).
    /// If there are more stickers than cells at 2×2, they will overlap in spawn order.
    /// </summary>
    private float minCellSize = 2f;

    // ——— PUBLIC API ———

    /// <summary>
    /// Call this from your movement scripts (e.g. FlowerMovement)
    /// when a sticker’s X < –40f. 
    /// Instead of destroying it, we “collect” it here.
    /// </summary>
    /// <param name="sticker">The sticker GameObject that would otherwise be destroyed.</param>
    public void AddSticker(GameObject sticker)
    {
        if (sticker == null) return;

        // Remove any movement component so it stops drifting.
        // (Assumes each sticker prefab has a component named FlowerMovement; change
        //  this if your stickers use a different script.)
        var movementComp = sticker.GetComponent<FlowerMovement>();
        if (movementComp != null)
            Destroy(movementComp);

        // Optional: If your sticker has any 2D/3D collider you want to disable, do so now:
        // var col2D = sticker.GetComponent<Collider2D>();
        // if (col2D != null) col2D.enabled = false;
        // var col3D = sticker.GetComponent<Collider>();
        // if (col3D != null) col3D.enabled = false;

        // Add to the list in the exact order it went off-screen:
        collectedStickers.Add(sticker);
    }

    /// <summary>
    /// Call this exactly when the train “arrives” at a station (e.g. inside PassingObject.PauseMovement()).
    /// All collected stickers are re-parented to the stationBoardTransform and laid out in a grid.
    /// </summary>
    public void SnapAllToBoard()
    {
        if (stationBoardTransform == null)
        {
            Debug.LogError("StickerCollector: You must assign stationBoardTransform in the Inspector!");
            return;
        }

        SpriteRenderer boardSR = stationBoardTransform.GetComponent<SpriteRenderer>();
        if (boardSR == null)
        {
            // zoek bij de kinderen:
            boardSR = stationBoardTransform.GetComponentInChildren<SpriteRenderer>();
        }

        if (boardSR == null)
        {
            Debug.LogError("StickerCollector: stationBoardTransform of één van zijn kinderen heeft geen SpriteRenderer!");
            return;
        }
        Vector2 boardWorldSize = boardSR.bounds.size; // b.v. (38.4, 51.2)
        Debug.Log($"[Debug] raw boardWorldSize = {boardWorldSize.x:F2} × {boardWorldSize.y:F2}");

        // Pas hier de handmatige factor toe:
        float rawBoardWidth = boardWorldSize.x;    // b.v. 38.4
        float rawBoardHeight = boardWorldSize.y;    // b.v. 51.2
        float boardWidth = rawBoardWidth / widthFactor;
        float boardHeight = rawBoardHeight / heightFactor;
        Debug.Log($"[Debug] widthFactor={widthFactor:F2}, heightFactor={heightFactor:F2} → boardWidth={boardWidth:F2}, boardHeight={boardHeight:F2}");

        int count = collectedStickers.Count;
        if (count == 0)
        {
            // Nothing to snap; return early.
            return;
        }

        // 1) Compute how many columns/rows we need, subject to "minCellSize = 2×2".
        int maxCols = Mathf.FloorToInt(boardWidth / minCellSize);
        int maxRows = Mathf.FloorToInt(boardHeight / minCellSize);

        float cellWidth, cellHeight;
        int cols, rows;

        if (maxCols * maxRows >= count)
        {
            // We have enough “2×2 cells” to hold all stickers. 
            // But to avoid tiny empty gaps, we can expand the cell size uniformly so they fill the 15×20 board.

            // 2a) Compute an approximate “best” column count based on aspect ratio:
            float ratio = boardWidth / boardHeight; // 15/20 = 0.75
            int approxCols = Mathf.CeilToInt(Mathf.Sqrt(count * ratio));
            approxCols = Mathf.Clamp(approxCols, 1, maxCols);

            // 2b) Determine rows from final column count:
            int approxRows = Mathf.CeilToInt((float)count / approxCols);
            approxRows = Mathf.Clamp(approxRows, 1, maxRows);

            // 2c) Compute exact cell size so all cells fit in 15×20:
            float w = boardWidth / approxCols;
            float h = boardHeight / approxRows;
            float finalCellSize = Mathf.Min(w, h);

            cellWidth = finalCellSize;
            cellHeight = finalCellSize;
            cols = approxCols;
            rows = approxRows;
        }
        else
        {
            // There are more stickers than a strict 2×2 grid can hold (maxCols × maxRows).
            // In that case, use exactly 2×2 cells, and allow overlap if there are “extra” stickers.
            cellWidth = minCellSize;
            cellHeight = minCellSize;
            cols = maxCols;
            rows = maxRows;
        }
        Debug.Log($"[Debug] count={count}, maxCols={maxCols}, maxRows={maxRows}, cols={cols}, rows={rows}, cellWidth={cellWidth:F2}");


        // 2) Re-parent and place each sticker in spawn‐order:
        for (int i = 0; i < count; i++)
        {
            GameObject s = collectedStickers[i];
            if (s == null) continue;

            // Parent it under the board:
            s.transform.SetParent(stationBoardTransform, worldPositionStays: false);

            // Determine which grid cell “i” belongs in:
            int colIndex = i % cols;         // 0..(cols-1)
            int rowIndex = i / cols;         // 0..(rows-1); might exceed if count > cols*rows

            // Compute local X/Y so that top-left is (–boardWidth/2, +boardHeight/2):
            float xOffset = -boardWidth / 2f + (colIndex + 0.5f) * cellWidth;
            float yOffset = boardHeight / 2f - (rowIndex + 0.5f) * cellHeight;

            // If “rowIndex >= rows” (overflow beyond max rows), it will just keep increasing yOffset downward
            // (causing overlap), which is fine per our “overlap if > capacity” rule.

            s.transform.localPosition = new Vector3(xOffset, yOffset, 0f);
            Debug.Log($"[Debug] Sticker '{s.name}' localPos = ({xOffset:F2}, {yOffset:F2})");


            // Scale each sticker uniformly so its sprite fits inside an (cellWidth × cellHeight) square.
            // We assume the sticker’s original sprite is roughly square. If not, you can adjust individually.
            SpriteRenderer sr = s.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                float spriteWorldWidth = sr.sprite.bounds.size.x;
                float scaleFactor = cellWidth / spriteWorldWidth;
                Debug.Log($"[Debug] Sticker '{s.name}' spriteWorldWidth={spriteWorldWidth:F2}, scaleFactor={scaleFactor:F2}, resultingWidth={spriteWorldWidth * scaleFactor:F2}");
                s.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
            }
            else
            {
                s.transform.localScale = Vector3.one * (cellWidth / 5f);
            }
        }

        // 3) Clear the list so the next station collects a fresh batch:
        collectedStickers.Clear();
    }
}
