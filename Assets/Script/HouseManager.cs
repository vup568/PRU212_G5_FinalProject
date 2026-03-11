using UnityEngine;

/// <summary>
/// Quản lý ẩn/hiện ngôi nhà theo Level.
/// Gắn script này vào một Empty GameObject trong PlayScene.
/// Kéo thả các GameObject nhà Level 1 và Level 2 vào Inspector.
/// </summary>
public class HouseManager : MonoBehaviour
{
    [Header("House References")]
    [Tooltip("GameObject ngôi nhà Level 1 (hiển thị mặc định)")]
    [SerializeField] private GameObject houseLevel1;

    [Tooltip("GameObject ngôi nhà Level 2 (ẩn mặc định, hiện khi lên Level 2)")]
    [SerializeField] private GameObject houseLevel2;

    private void Awake()
    {
        // Đăng ký lắng nghe event lên level
        LevelManager.OnLevelUp += OnLevelChanged;
    }

    private void Start()
    {
        // Khi vào scene, kiểm tra level hiện tại để set đúng trạng thái nhà
        // (phòng trường hợp user đã Level 2 từ trước đó)
        StartCoroutine(WaitForUserDataAndApply());
    }

    private void OnDestroy()
    {
        LevelManager.OnLevelUp -= OnLevelChanged;
    }

    /// <summary>
    /// Đợi cho đến khi userInGame được load từ Firebase xong rồi mới apply trạng thái nhà.
    /// </summary>
    private System.Collections.IEnumerator WaitForUserDataAndApply()
    {
        // Đợi cho userInGame được load (có thể mất vài frame do async Firebase)
        while (LoadDataManager.userInGame == null)
        {
            yield return null;
        }

        int currentLevel = LoadDataManager.userInGame.Level;
        Debug.Log($"[HouseManager] User hiện tại Level {currentLevel}, đang set trạng thái nhà...");
        ApplyHouseState(currentLevel);
    }

    /// <summary>
    /// Được gọi khi player lên level (thông qua event OnLevelUp).
    /// </summary>
    private void OnLevelChanged(int newLevel)
    {
        Debug.Log($"[HouseManager] Nhận event lên Level {newLevel}!");
        ApplyHouseState(newLevel);
    }

    /// <summary>
    /// Set trạng thái ẩn/hiện nhà dựa theo level.
    /// </summary>
    private void ApplyHouseState(int level)
    {
        if (level >= 2)
        {
            // Level 2+: Ẩn nhà cũ, hiện nhà mới
            if (houseLevel1 != null) houseLevel1.SetActive(false);
            if (houseLevel2 != null) houseLevel2.SetActive(true);
            Debug.Log("[HouseManager] Đã chuyển sang nhà Level 2!");
        }
        else
        {
            // Level 1: Hiện nhà cũ, ẩn nhà mới
            if (houseLevel1 != null) houseLevel1.SetActive(true);
            if (houseLevel2 != null) houseLevel2.SetActive(false);
            Debug.Log("[HouseManager] Đang hiển thị nhà Level 1.");
        }
    }
}
