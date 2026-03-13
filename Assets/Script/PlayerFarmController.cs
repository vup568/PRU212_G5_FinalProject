using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerFarmController : MonoBehaviour
{
    public Tilemap tm_Ground;
    public Tilemap tm_Grass;
    public Tilemap tm_Forest;

    public TileBase tb_Ground;
    public TileBase tb_Grass;
    public TileBase tb_Forest;

    public List<TileBase> listTilebase_Corn;
    //public List<TileBase> listTilebase_CanHarvest;
    //can harvest la co the thu hoach

    [Header("=== Level 2: Flower ===")]
    [Tooltip("Danh sách TileBase cho các giai đoạn phát triển của hoa (từ Sprout Lands - Basic Plants)")]
    public List<TileBase> listTilebase_Flower;

    [SerializeField] private Sprite cornInventoryIcon;

    [Header("Flower Inventory Icon")]
    [Tooltip("Icon hiển thị trong inventory khi thu hoạch hoa")]
    [SerializeField] private Sprite flowerInventoryIcon;

    [Header("Grow Time Settings")]
    [Tooltip("Thời gian mỗi giai đoạn phát triển của Corn (giây)")]
    public float cornGrowTime = 5f;
    [Tooltip("Thời gian mỗi giai đoạn phát triển của Flower (giây) - lâu hơn Corn")]
    public float flowerGrowTime = 8f;

    [Header("Sell Price Settings")]
    [Tooltip("Giá bán Corn")]
    public int cornSellPrice = 10;
    [Tooltip("Giá bán Flower - cao hơn Corn")]
    public int flowerSellPrice = 25;

    private RecyclableInventoryManager recyclableInventoryManager;

    public TileMapManager tileMapManager;

    public GameObject usernameWizard;

    // Theo dõi các ô đang có cây mọc (tránh trồng chồng lên)
    private HashSet<Vector3Int> growingTiles = new HashSet<Vector3Int>();

    // Start is called before the first frame update
    void Start()
    {
        recyclableInventoryManager = GameObject.Find("InventoryManager").GetComponent<RecyclableInventoryManager>();

    }

    // Update is called once per frame
    void Update()
    {
        HandleFarmAction();
    }


    public void HandleFarmAction()
    {
        //Digging
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Digging");
            Vector3Int cellPos = tm_Ground.WorldToCell(transform.position); //take precise location of player with integer location
            Debug.Log("Cell position: " + cellPos);

            TileBase currentTb = tm_Grass.GetTile(cellPos);
            if (currentTb == tb_Grass)
            {
                tm_Grass.SetTile(cellPos, null);
                FindObjectOfType<AudioManager>().PlayDigSound();
                tileMapManager.SetStateForTilemapDetail(cellPos.x, cellPos.y, TilemapState.Ground);

            }

        }

        //Plant Corn (Level 1)
        if (Input.GetKeyDown(KeyCode.V))
        {
            Vector3Int cellPos = tm_Ground.WorldToCell(transform.position); //take precise location of player with integer location
            Debug.Log("Cell position: " + cellPos);

            TileBase currentTb = tm_Grass.GetTile(cellPos);
            TileBase currentForest = tm_Forest.GetTile(cellPos);
            if (currentTb == null && currentForest == null && !growingTiles.Contains(cellPos))
            {
                //tm_Forest.SetTile(cellPos, tb_Forest);
                StartCoroutine(GrowPlantWithCustomTime(cellPos, tm_Forest, listTilebase_Corn, cornGrowTime));
                FindObjectOfType<AudioManager>().PlayPlantSound();
                tileMapManager.SetStateForTilemapDetail(cellPos.x, cellPos.y, TilemapState.Corn);

            }
        }

        // Plant Flower (Level 2+) - Phím B
        if (Input.GetKeyDown(KeyCode.B))
        {
            // Kiểm tra level
            int currentLevel = LevelManager.Instance != null ? LevelManager.Instance.GetCurrentLevel() : 1;
            if (currentLevel < 2)
            {
                Debug.Log("[PlayerFarm] Cần Level 2 để trồng hoa! Hãy bán thêm Corn để đạt 200 Gold.");
                return;
            }

            // Kiểm tra có đủ tile flower không
            if (listTilebase_Flower == null || listTilebase_Flower.Count == 0)
            {
                Debug.LogWarning("[PlayerFarm] listTilebase_Flower chưa được gán trong Inspector!");
                return;
            }

            Vector3Int cellPos = tm_Ground.WorldToCell(transform.position);
            Debug.Log("[PlayerFarm] Trồng hoa tại: " + cellPos);

            TileBase currentTb = tm_Grass.GetTile(cellPos);
            TileBase currentForest = tm_Forest.GetTile(cellPos);
            if (currentTb == null && currentForest == null && !growingTiles.Contains(cellPos))
            {
                StartCoroutine(GrowPlantWithCustomTime(cellPos, tm_Forest, listTilebase_Flower, flowerGrowTime));
                FindObjectOfType<AudioManager>().PlayPlantSound();
                tileMapManager.SetStateForTilemapDetail(cellPos.x, cellPos.y, TilemapState.Flower);
            }
            else
            {
                Debug.Log("[PlayerFarm] Ô này đã có cây hoặc chưa được đào!");
            }
        }

        //take tree (harvest) - Thu hoạch cả Corn và Flower
        if (Input.GetKeyDown(KeyCode.X))
        {
            Vector3Int cellPos = tm_Ground.WorldToCell(transform.position); //take precise location of player with integer location
            Debug.Log("Cell position: " + cellPos);

            TileBase currentTb = tm_Forest.GetTile(cellPos);

            // Thu hoạch Corn (giai đoạn cuối cùng)
            if (listTilebase_Corn.Count > 0 && currentTb == listTilebase_Corn[listTilebase_Corn.Count - 1])
            {
                tm_Grass.SetTile(cellPos, null);
                tm_Forest.SetTile(cellPos, null);
                growingTiles.Remove(cellPos);

                InvenItems itemCorn = new InvenItems("Corn", "Fresh Corn", cornInventoryIcon, cornSellPrice, true);

                Debug.Log(itemCorn.ToString());

                recyclableInventoryManager.AddInventoryItem(itemCorn);
                FindObjectOfType<AudioManager>().PlayCollectSound();
                tileMapManager.SetStateForTilemapDetail(cellPos.x, cellPos.y, TilemapState.Ground);
            }
            // Thu hoạch Flower (giai đoạn cuối cùng)
            else if (listTilebase_Flower != null && listTilebase_Flower.Count > 0 
                     && currentTb == listTilebase_Flower[listTilebase_Flower.Count - 1])
            {
                tm_Grass.SetTile(cellPos, null);
                tm_Forest.SetTile(cellPos, null);
                growingTiles.Remove(cellPos);

                InvenItems itemFlower = new InvenItems("Tomato", "Fresh Tomato", flowerInventoryIcon, flowerSellPrice, true);

                Debug.Log(itemFlower.ToString());

                recyclableInventoryManager.AddInventoryItem(itemFlower);
                FindObjectOfType<AudioManager>().PlayCollectSound();
                tileMapManager.SetStateForTilemapDetail(cellPos.x, cellPos.y, TilemapState.Ground);
            }
        }
    }

    /// <summary>
    /// Coroutine mọc cây với thời gian tùy chỉnh cho mỗi giai đoạn.
    /// </summary>
    IEnumerator GrowPlantWithCustomTime(Vector3Int cellPos, Tilemap tilemap, List<TileBase> listTilebase, float growTimePerStage)
    {
        growingTiles.Add(cellPos);
        int currentState = 0;
        while(currentState < listTilebase.Count)
        {
            tilemap.SetTile(cellPos, listTilebase[currentState]);
            yield return new WaitForSeconds(growTimePerStage);
            currentState++;
        }
        // Cây đã mọc xong, không remove khỏi growingTiles ở đây
        // Sẽ remove khi thu hoạch (press X)
    }
}

