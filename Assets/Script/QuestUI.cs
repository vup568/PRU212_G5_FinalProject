using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Hiển thị quest hiện tại trên UI: icon + mô tả + tiến độ.
/// HUD góc trên bên trái, nằm dưới Weather HUD.
/// Gắn script này vào một GameObject trong Scene (ví dụ: "QuestUI").
/// </summary>
public class QuestUI : MonoBehaviour
{
    private GameObject questHUD;
    private Text questIconText;
    private Text questDescText;
    private Text questProgressText;
    private Text questRewardText;
    private Image hudBackground;

    // Màu sắc
    private readonly Color colorWaiting = new Color(0.3f, 0.3f, 0.4f, 0.8f);    // Xám - chờ quest
    private readonly Color colorActive = new Color(0.15f, 0.35f, 0.65f, 0.85f);  // Xanh dương - đang làm
    private readonly Color colorCompleted = new Color(0.1f, 0.6f, 0.2f, 0.9f);   // Xanh lá - hoàn thành

    private Canvas gameCanvas;

    private void Start()
    {
        // Tìm Canvas game (không phải login canvas)
        Canvas[] allCanvas = FindObjectsOfType<Canvas>();
        foreach (Canvas c in allCanvas)
        {
            if (c.gameObject.name != "LoginCanvas")
            {
                gameCanvas = c;
                break;
            }
        }
        if (gameCanvas == null)
            gameCanvas = FindObjectOfType<Canvas>();

        if (gameCanvas == null)
        {
            Debug.LogError("[QuestUI] Không tìm thấy Canvas!");
            return;
        }

        // Tự tạo QuestManager nếu chưa có
        if (QuestManager.Instance == null)
        {
            GameObject qmObj = new GameObject("QuestManager");
            qmObj.AddComponent<QuestManager>();
        }

        CreateHUD();

        // Đăng ký events
        QuestManager.Instance.OnQuestAssigned += OnQuestAssigned;
        QuestManager.Instance.OnQuestCompleted += OnQuestCompleted;
        QuestManager.Instance.OnQuestProgress += OnQuestProgress;

        // Hiển thị trạng thái chờ ban đầu
        ShowWaitingState();
    }

    private void OnDestroy()
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnQuestAssigned -= OnQuestAssigned;
            QuestManager.Instance.OnQuestCompleted -= OnQuestCompleted;
            QuestManager.Instance.OnQuestProgress -= OnQuestProgress;
        }
    }

    /// <summary>
    /// Tạo HUD quest góc trên bên trái, dưới Weather HUD.
    /// </summary>
    private void CreateHUD()
    {
        // === Panel container ===
        questHUD = new GameObject("QuestHUD");
        questHUD.transform.SetParent(gameCanvas.transform, false);

        RectTransform hudRt = questHUD.AddComponent<RectTransform>();
        hudRt.anchorMin = new Vector2(0, 1); // top-left
        hudRt.anchorMax = new Vector2(0, 1);
        hudRt.pivot = new Vector2(0, 1);
        hudRt.anchoredPosition = new Vector2(10, -95); // Dưới Weather HUD (y = -10 - 75 - 10)
        hudRt.sizeDelta = new Vector2(280, 80);

        hudBackground = questHUD.AddComponent<Image>();
        hudBackground.color = colorWaiting;
        hudBackground.raycastTarget = false;

        // === Icon quest (bên trái) ===
        GameObject iconGo = new GameObject("QuestIcon");
        iconGo.transform.SetParent(questHUD.transform, false);
        RectTransform iconRt = iconGo.AddComponent<RectTransform>();
        iconRt.anchorMin = new Vector2(0, 0);
        iconRt.anchorMax = new Vector2(0, 1);
        iconRt.pivot = new Vector2(0, 0.5f);
        iconRt.anchoredPosition = new Vector2(5, 0);
        iconRt.sizeDelta = new Vector2(45, 0);

        questIconText = iconGo.AddComponent<Text>();
        questIconText.text = "...";
        questIconText.fontSize = 28;
        questIconText.alignment = TextAnchor.MiddleCenter;
        questIconText.color = Color.white;
        questIconText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        questIconText.raycastTarget = false;

        // === Mô tả quest (dòng 1) ===
        GameObject descGo = new GameObject("QuestDesc");
        descGo.transform.SetParent(questHUD.transform, false);
        RectTransform descRt = descGo.AddComponent<RectTransform>();
        descRt.anchorMin = new Vector2(0, 0.55f);
        descRt.anchorMax = new Vector2(1, 1);
        descRt.pivot = new Vector2(0, 1);
        descRt.anchoredPosition = new Vector2(50, -5);
        descRt.sizeDelta = new Vector2(-60, 0);

        questDescText = descGo.AddComponent<Text>();
        questDescText.text = "Chờ nhiệm vụ...";
        questDescText.fontSize = 16;
        questDescText.fontStyle = FontStyle.Bold;
        questDescText.alignment = TextAnchor.MiddleLeft;
        questDescText.color = Color.white;
        questDescText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        questDescText.raycastTarget = false;

        // === Tiến độ (dòng 2, bên trái) ===
        GameObject progressGo = new GameObject("QuestProgress");
        progressGo.transform.SetParent(questHUD.transform, false);
        RectTransform progressRt = progressGo.AddComponent<RectTransform>();
        progressRt.anchorMin = new Vector2(0, 0);
        progressRt.anchorMax = new Vector2(0.6f, 0.55f);
        progressRt.pivot = new Vector2(0, 0);
        progressRt.anchoredPosition = new Vector2(50, 5);
        progressRt.sizeDelta = new Vector2(0, 0);

        questProgressText = progressGo.AddComponent<Text>();
        questProgressText.text = "";
        questProgressText.fontSize = 14;
        questProgressText.alignment = TextAnchor.MiddleLeft;
        questProgressText.color = new Color(1f, 1f, 0.6f, 1f); // Vàng nhạt
        questProgressText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        questProgressText.raycastTarget = false;

        // === Thưởng (dòng 2, bên phải) ===
        GameObject rewardGo = new GameObject("QuestReward");
        rewardGo.transform.SetParent(questHUD.transform, false);
        RectTransform rewardRt = rewardGo.AddComponent<RectTransform>();
        rewardRt.anchorMin = new Vector2(0.6f, 0);
        rewardRt.anchorMax = new Vector2(1, 0.55f);
        rewardRt.pivot = new Vector2(1, 0);
        rewardRt.anchoredPosition = new Vector2(-10, 5);
        rewardRt.sizeDelta = new Vector2(0, 0);

        questRewardText = rewardGo.AddComponent<Text>();
        questRewardText.text = "";
        questRewardText.fontSize = 14;
        questRewardText.fontStyle = FontStyle.Bold;
        questRewardText.alignment = TextAnchor.MiddleRight;
        questRewardText.color = new Color(1f, 0.85f, 0.2f, 1f); // Vàng gold
        questRewardText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        questRewardText.raycastTarget = false;
    }

    // === Event Handlers ===

    private void OnQuestAssigned(QuestData quest)
    {
        if (questIconText != null)
            questIconText.text = GetQuestIcon(quest.questType);

        if (questDescText != null)
            questDescText.text = quest.description;

        if (questProgressText != null)
            questProgressText.text = "Tiến độ: " + quest.GetProgressText();

        if (questRewardText != null)
            questRewardText.text = "+" + quest.goldReward + " Gold";

        if (hudBackground != null)
            hudBackground.color = colorActive;

        Debug.Log($"[QuestUI] Hiển thị quest: {quest.description}");
    }

    private void OnQuestProgress(QuestData quest)
    {
        if (questProgressText != null)
            questProgressText.text = "Tiến độ: " + quest.GetProgressText();
    }

    private void OnQuestCompleted(QuestData quest)
    {
        if (questIconText != null)
            questIconText.text = "OK";

        if (questDescText != null)
            questDescText.text = "Hoàn thành!";

        if (questProgressText != null)
            questProgressText.text = quest.GetProgressText();

        if (questRewardText != null)
            questRewardText.text = "+" + quest.goldReward + " Gold";

        if (hudBackground != null)
            hudBackground.color = colorCompleted;

        Debug.Log($"[QuestUI] Quest hoàn thành! +{quest.goldReward} Gold");
    }

    private void ShowWaitingState()
    {
        if (questIconText != null)
            questIconText.text = "...";

        if (questDescText != null)
            questDescText.text = "Chờ nhiệm vụ...";

        if (questProgressText != null)
            questProgressText.text = "";

        if (questRewardText != null)
            questRewardText.text = "";

        if (hudBackground != null)
            hudBackground.color = colorWaiting;
    }

    /// <summary>
    /// Lấy icon cho từng loại quest.
    /// </summary>
    private string GetQuestIcon(QuestType type)
    {
        switch (type)
        {
            case QuestType.HarvestCorn:   return "C";
            case QuestType.HarvestFlower: return "H";
            case QuestType.PlantAny:      return "T";
            case QuestType.DigTiles:      return "D";
            case QuestType.SellGold:      return "$";
            case QuestType.PlantFlower:   return "F";
            default:                      return "?";
        }
    }
}
