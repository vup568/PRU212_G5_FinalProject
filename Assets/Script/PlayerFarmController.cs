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

        // Đăng ký event thời tiết để xử lý Bão phá hủy cây đã mọc xong
        if (WeatherManager.Instance != null)
            WeatherManager.Instance.OnWeatherChanged += OnWeatherChanged;
    }

    void OnDestroy()
    {
        if (WeatherManager.Instance != null)
            WeatherManager.Instance.OnWeatherChanged -= OnWeatherChanged;
    }

    /// <summary>
    /// Khi thời tiết thay đổi — nếu là Bão, quét tất cả cây trên map.
    /// </summary>
    private void OnWeatherChanged(WeatherManager.WeatherType newWeather)
    {
        if (newWeather == WeatherManager.WeatherType.Storm)
        {
            OnStormHit();
        }
    }

    /// <summary>
    /// Bão ập đến: mỗi cây (đang mọc + đã mọc xong) có 20% bị phá hủy.
    /// </summary>
    private void OnStormHit()
    {
        if (growingTiles.Count == 0) return;

        float killChance = WeatherManager.Instance != null ? WeatherManager.Instance.GetStormKillChance() : 0.2f;
        List<Vector3Int> tilesToKill = new List<Vector3Int>();

        foreach (Vector3Int cellPos in growingTiles)
        {
            if (Random.value < killChance)
            {
                tilesToKill.Add(cellPos);
            }
        }

        foreach (Vector3Int cellPos in tilesToKill)
        {
            tm_Forest.SetTile(cellPos, null);
            growingTiles.Remove(cellPos);
            tileMapManager.SetStateForTilemapDetail(cellPos.x, cellPos.y, TilemapState.Ground);
            Debug.Log($"[Weather] ⛈️ Bão đã phá hủy cây tại {cellPos}!");
        }

        if (tilesToKill.Count > 0)
        {
            SeedShopUI shopUI = FindObjectOfType<SeedShopUI>();
            if (shopUI != null)
                shopUI.ShowNotification($"⛈️ Bão phá hủy {tilesToKill.Count} cây!", 3f);

            Debug.Log($"[Weather] ⛈️ Tổng cộng {tilesToKill.Count}/{growingTiles.Count + tilesToKill.Count} cây bị phá hủy!");
        }
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

                // Report quest progress: Đào đất
                if (QuestManager.Instance != null)
                    QuestManager.Instance.ReportProgress(QuestType.DigTiles, 1);
            }

        }

        //Plant Corn (Level 1) - YÊU CẦU CÓ SEED
        if (Input.GetKeyDown(KeyCode.V))
        {
            Vector3Int cellPos = tm_Ground.WorldToCell(transform.position); //take precise location of player with integer location
            Debug.Log("Cell position: " + cellPos);

            TileBase currentTb = tm_Grass.GetTile(cellPos);
            TileBase currentForest = tm_Forest.GetTile(cellPos);
            if (currentTb == null && currentForest == null && !growingTiles.Contains(cellPos))
            {
                // Kiểm tra seed trước khi trồng
                if (SeedShopManager.Instance == null || !SeedShopManager.Instance.UseSeed("Corn"))
                {
                    Debug.Log("[PlayerFarm] Hết hạt Ngô! Hãy mua thêm tại cửa hàng (nhấn P).");
                    SeedShopUI shopUI = FindObjectOfType<SeedShopUI>();
                    if (shopUI != null) shopUI.ShowNotification("Hết hạt Ngô! Nhấn P để mua.");
                    return;
                }

                StartCoroutine(GrowPlantWithCustomTime(cellPos, tm_Forest, listTilebase_Corn, cornGrowTime));
                FindObjectOfType<AudioManager>().PlayPlantSound();
                tileMapManager.SetStateForTilemapDetail(cellPos.x, cellPos.y, TilemapState.Corn);

                // Report quest progress: Trồng cây (Corn)
                if (QuestManager.Instance != null)
                    QuestManager.Instance.ReportProgress(QuestType.PlantAny, 1);
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
                // Kiểm tra seed trước khi trồng
                if (SeedShopManager.Instance == null || !SeedShopManager.Instance.UseSeed("Flower"))
                {
                    Debug.Log("[PlayerFarm] Hết hạt Hoa! Hãy mua thêm tại cửa hàng (nhấn P).");
                    SeedShopUI shopUI = FindObjectOfType<SeedShopUI>();
                    if (shopUI != null) shopUI.ShowNotification("Hết hạt Hoa! Nhấn P để mua.");
                    return;
                }

                StartCoroutine(GrowPlantWithCustomTime(cellPos, tm_Forest, listTilebase_Flower, flowerGrowTime));
                FindObjectOfType<AudioManager>().PlayPlantSound();
                tileMapManager.SetStateForTilemapDetail(cellPos.x, cellPos.y, TilemapState.Flower);

                // Report quest progress: Trồng cây (Flower) + Trồng cây bất kỳ
                if (QuestManager.Instance != null)
                {
                    QuestManager.Instance.ReportProgress(QuestType.PlantAny, 1);
                    QuestManager.Instance.ReportProgress(QuestType.PlantFlower, 1);
                }
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
                tileMapManager.SetStateForTilemapDetail(cellPos.x, cellPos.y, TilemapState.Grass);

                // Report quest progress: Thu hoạch Corn
                if (QuestManager.Instance != null)
                    QuestManager.Instance.ReportProgress(QuestType.HarvestCorn, 1);
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
                tileMapManager.SetStateForTilemapDetail(cellPos.x, cellPos.y, TilemapState.Grass);

                // Report quest progress: Thu hoạch Flower
                if (QuestManager.Instance != null)
                    QuestManager.Instance.ReportProgress(QuestType.HarvestFlower, 1);
            }
        }
    }

    /// <summary>
    /// Coroutine mọc cây với thời gian tùy chỉnh cho mỗi giai đoạn.
    /// Áp dụng hiệu ứng thời tiết mỗi stage:
    /// - Mưa: mọc nhanh x1.5
    /// - Bão: 20% cây chết
    /// </summary>
    IEnumerator GrowPlantWithCustomTime(Vector3Int cellPos, Tilemap tilemap, List<TileBase> listTilebase, float growTimePerStage)
    {
        growingTiles.Add(cellPos);
        int currentState = 0;
        while(currentState < listTilebase.Count)
        {
            tilemap.SetTile(cellPos, listTilebase[currentState]);

            // Lấy tốc độ mọc theo thời tiết hiện tại
            float multiplier = WeatherManager.Instance != null ? WeatherManager.Instance.GetGrowthMultiplier() : 1f;
            float actualGrowTime = growTimePerStage / multiplier;

            yield return new WaitForSeconds(actualGrowTime);

            // Bão: kiểm tra cây có bị chết không
            float killChance = WeatherManager.Instance != null ? WeatherManager.Instance.GetStormKillChance() : 0f;
            if (killChance > 0f && Random.value < killChance)
            {
                // Cây bị bão phá hủy!
                tilemap.SetTile(cellPos, null);
                growingTiles.Remove(cellPos);
                tileMapManager.SetStateForTilemapDetail(cellPos.x, cellPos.y, TilemapState.Ground);
                Debug.Log($"[Weather] ⛈️ Bão đã phá hủy cây tại {cellPos}!");

                // Thông báo cho người chơi
                SeedShopUI shopUI = FindObjectOfType<SeedShopUI>();
                if (shopUI != null) shopUI.ShowNotification("⛈️ Bão đã phá hủy cây!", 2f);

                yield break; // Dừng coroutine - cây đã chết
            }

            currentState++;
        }
        // Cây đã mọc xong, không remove khỏi growingTiles ở đây
        // Sẽ remove khi thu hoạch (press X)
    }
}

