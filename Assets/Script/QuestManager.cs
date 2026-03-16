using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton quản lý hệ thống Quest/Nhiệm vụ ngẫu nhiên.
/// Random quest mỗi 120 giây. Hoàn thành quest → nhận Gold bonus.
/// 
/// SETUP trong Unity:
/// 1. Tạo Empty GameObject tên "QuestManager", gắn script này.
/// 2. Không cần gán gì trong Inspector (tự hoạt động).
/// </summary>
public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [Header("Quest Timing")]
    [Tooltip("Thời gian chờ trước khi giao quest đầu tiên (giây)")]
    public float firstQuestDelay = 10f;

    [Tooltip("Thời gian giữa các quest (giây) - nếu quest cũ chưa hoàn thành sẽ bị thay thế")]
    public float questInterval = 120f;

    [Tooltip("Thời gian chờ sau khi hoàn thành quest trước khi giao quest mới (giây)")]
    public float afterCompleteDelay = 5f;

    /// <summary>
    /// Quest hiện tại (null nếu chưa có).
    /// </summary>
    public QuestData CurrentQuest { get; private set; }

    // === Events ===
    /// <summary>Event khi có quest mới được giao.</summary>
    public System.Action<QuestData> OnQuestAssigned;
    /// <summary>Event khi quest hoàn thành.</summary>
    public System.Action<QuestData> OnQuestCompleted;
    /// <summary>Event khi tiến độ quest thay đổi.</summary>
    public System.Action<QuestData> OnQuestProgress;

    private Coroutine questCycleCoroutine;
    private bool waitingForNextQuest = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Bắt đầu cycle quest
        questCycleCoroutine = StartCoroutine(QuestCycle());
        Debug.Log("[QuestManager] 📋 Hệ thống Quest đã khởi động!");
    }

    /// <summary>
    /// Coroutine chính: chờ delay → giao quest → chờ interval → lặp lại.
    /// </summary>
    private IEnumerator QuestCycle()
    {
        // Chờ trước khi giao quest đầu tiên
        yield return new WaitForSeconds(firstQuestDelay);

        while (true)
        {
            // Giao quest mới
            AssignNewQuest();

            // Chờ interval (quest cũ sẽ bị thay thế nếu chưa hoàn thành)
            float elapsed = 0f;
            waitingForNextQuest = false;

            while (elapsed < questInterval)
            {
                // Nếu quest vừa hoàn thành, chờ afterCompleteDelay rồi giao quest mới
                if (waitingForNextQuest)
                {
                    yield return new WaitForSeconds(afterCompleteDelay);
                    break; // Thoát vòng while để giao quest mới
                }

                elapsed += Time.deltaTime;
                yield return null;
            }
        }
    }

    /// <summary>
    /// Tạo và giao quest ngẫu nhiên.
    /// </summary>
    private void AssignNewQuest()
    {
        CurrentQuest = GenerateRandomQuest();
        Debug.Log($"[QuestManager] 📋 Quest mới: {CurrentQuest.description} (Thưởng: {CurrentQuest.goldReward} Gold)");
        OnQuestAssigned?.Invoke(CurrentQuest);
    }

    /// <summary>
    /// Random quest từ danh sách, lọc theo Level hiện tại.
    /// </summary>
    private QuestData GenerateRandomQuest()
    {
        int currentLevel = LevelManager.Instance != null ? LevelManager.Instance.GetCurrentLevel() : 1;

        // Danh sách tất cả quest có thể
        List<QuestData> possibleQuests = new List<QuestData>
        {
            new QuestData(QuestType.HarvestCorn, "Thu hoạch 3 Ngô", 3, 30),
            new QuestData(QuestType.PlantAny, "Trồng 5 cây bất kỳ", 5, 25),
            new QuestData(QuestType.DigTiles, "Đào 8 ô đất", 8, 20),
            new QuestData(QuestType.SellGold, "Bán hàng kiếm 50 Gold", 50, 35),
        };

        // Quest Flower chỉ xuất hiện khi Level >= 2
        if (currentLevel >= 2)
        {
            possibleQuests.Add(new QuestData(QuestType.HarvestFlower, "Thu hoạch 2 Hoa", 2, 40));
            possibleQuests.Add(new QuestData(QuestType.PlantFlower, "Trồng 3 Hoa", 3, 35));
        }

        // Random 1 quest
        int randomIndex = Random.Range(0, possibleQuests.Count);
        return possibleQuests[randomIndex];
    }

    /// <summary>
    /// Các script khác gọi method này để report tiến độ.
    /// Ví dụ: QuestManager.Instance.ReportProgress(QuestType.HarvestCorn, 1);
    /// </summary>
    public void ReportProgress(QuestType type, int amount)
    {
        if (CurrentQuest == null) return;
        if (CurrentQuest.IsCompleted) return;

        // Chỉ cộng tiến độ nếu đúng loại quest
        if (CurrentQuest.questType != type) return;

        bool justCompleted = CurrentQuest.AddProgress(amount);

        Debug.Log($"[QuestManager] 📋 Tiến độ: {CurrentQuest.description} → {CurrentQuest.GetProgressText()}");
        OnQuestProgress?.Invoke(CurrentQuest);

        if (justCompleted)
        {
            CompleteQuest();
        }
    }

    /// <summary>
    /// Xử lý khi quest hoàn thành: cộng Gold bonus + thông báo.
    /// </summary>
    private void CompleteQuest()
    {
        if (CurrentQuest == null) return;

        Debug.Log($"[QuestManager] ✅ Quest hoàn thành: {CurrentQuest.description}! +{CurrentQuest.goldReward} Gold");

        // Cộng Gold thưởng
        if (GoldManager.Instance != null)
        {
            GoldManager.Instance.AddGold(CurrentQuest.goldReward);
        }

        // Thông báo UI
        SeedShopUI shopUI = FindObjectOfType<SeedShopUI>();
        if (shopUI != null)
            shopUI.ShowNotification($"✅ Quest hoàn thành! +{CurrentQuest.goldReward} Gold", 3f);

        OnQuestCompleted?.Invoke(CurrentQuest);

        // Đánh dấu chờ quest mới
        waitingForNextQuest = true;
    }
}
