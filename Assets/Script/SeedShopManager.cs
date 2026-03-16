using UnityEngine;

/// <summary>
/// Singleton quản lý số lượng hạt giống và logic mua/sử dụng seed.
/// Gắn script này vào một GameObject trong Scene (ví dụ: "SeedShopManager").
/// </summary>
public class SeedShopManager : MonoBehaviour
{
    public static SeedShopManager Instance { get; private set; }

    [Header("Seed Prices (Gold)")]
    [Tooltip("Giá mua 1 hạt Corn")]
    public int cornSeedPrice = 5;
    [Tooltip("Giá mua 1 hạt Flower")]
    public int flowerSeedPrice = 10;

    [Header("Current Seed Inventory")]
    public int cornSeedCount = 0;
    public int flowerSeedCount = 0;

    // Event để UI cập nhật khi seed thay đổi
    public System.Action OnSeedChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Mua hạt giống. Trả về true nếu mua thành công.
    /// </summary>
    public bool BuySeed(string seedType)
    {
        int price = GetSeedPrice(seedType);
        if (price <= 0)
        {
            Debug.LogWarning($"[SeedShop] Loại hạt giống '{seedType}' không hợp lệ!");
            return false;
        }

        // Kiểm tra và trừ Gold
        if (GoldManager.Instance == null || !GoldManager.Instance.SpendGold(price))
        {
            Debug.Log($"[SeedShop] Không đủ Gold để mua {seedType} Seed! (Cần {price} Gold)");
            return false;
        }

        // Tăng số seed
        if (seedType == "Corn")
            cornSeedCount++;
        else if (seedType == "Flower")
            flowerSeedCount++;

        Debug.Log($"[SeedShop] Đã mua 1 {seedType} Seed. Số lượng: {GetSeedCount(seedType)}");
        OnSeedChanged?.Invoke();
        return true;
    }

    /// <summary>
    /// Sử dụng 1 hạt giống khi trồng. Trả về true nếu còn seed.
    /// </summary>
    public bool UseSeed(string seedType)
    {
        if (seedType == "Corn" && cornSeedCount > 0)
        {
            cornSeedCount--;
            Debug.Log($"[SeedShop] Đã dùng 1 Corn Seed. Còn lại: {cornSeedCount}");
            OnSeedChanged?.Invoke();
            return true;
        }
        else if (seedType == "Flower" && flowerSeedCount > 0)
        {
            flowerSeedCount--;
            Debug.Log($"[SeedShop] Đã dùng 1 Flower Seed. Còn lại: {flowerSeedCount}");
            OnSeedChanged?.Invoke();
            return true;
        }

        Debug.Log($"[SeedShop] Hết {seedType} Seed! Hãy mua thêm tại cửa hàng (nhấn P).");
        return false;
    }

    /// <summary>
    /// Lấy số seed hiện có.
    /// </summary>
    public int GetSeedCount(string seedType)
    {
        if (seedType == "Corn") return cornSeedCount;
        if (seedType == "Flower") return flowerSeedCount;
        return 0;
    }

    /// <summary>
    /// Lấy giá seed.
    /// </summary>
    public int GetSeedPrice(string seedType)
    {
        if (seedType == "Corn") return cornSeedPrice;
        if (seedType == "Flower") return flowerSeedPrice;
        return 0;
    }
}
