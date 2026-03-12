using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Quản lý logic "Phá Đảo" khi người chơi đạt 300 Gold.
/// Hiện popup chúc mừng với 2 lựa chọn: Thoát Game hoặc Chơi Tiếp (Sandbox).
/// Trong chế độ Sandbox: gold vẫn hoạt động bình thường, chỉ không lên level nữa.
/// 
/// SETUP trong Unity:
/// 1. Tạo Empty GameObject tên "GameCompletionManager", gắn script này.
/// 2. Tạo Panel UI "GameCompletionPanel" trong Canvas:
///    - Thêm CanvasGroup component vào Panel.
///    - Text chính: "Chúc mừng! Bạn đã phá đảo!"
///    - Text phụ: "Bạn đã đạt 300 Gold và hoàn thành trò chơi!"
///    - Button "Thoát Game" → kéo vào field btnQuitGame
///    - Button "Chơi Tiếp (Sandbox)" → kéo vào field btnSandbox
/// 3. Kéo thả các UI references vào Inspector.
/// </summary>
public class GameCompletionManager : MonoBehaviour
{
    public static GameCompletionManager Instance { get; private set; }

    /// <summary>
    /// true = đang ở chế độ Sandbox (không lên level, không có phần thưởng progression).
    /// Các script khác kiểm tra flag này để quyết định có trigger level-up hay không.
    /// </summary>
    public static bool IsSandboxMode { get; private set; } = false;

    [Header("Completion Settings")]
    [Tooltip("Số Gold cần đạt để phá đảo")]
    [SerializeField] private int completionGoldThreshold = 300;

    [Header("UI References")]
    [Tooltip("Panel chứa popup chúc mừng phá đảo")]
    [SerializeField] private GameObject completionPanel;

    [Tooltip("Nút Thoát Game")]
    [SerializeField] private Button btnQuitGame;

    [Tooltip("Nút Chơi Tiếp (Sandbox)")]
    [SerializeField] private Button btnSandbox;

    [Header("Fade Settings")]
    [SerializeField] private float fadeInDuration = 0.5f;

    private CanvasGroup canvasGroup;
    private bool hasShownCompletion = false;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Reset sandbox mode khi vào scene mới
        IsSandboxMode = false;

        // Setup CanvasGroup cho fade effect
        if (completionPanel != null)
        {
            canvasGroup = completionPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = completionPanel.AddComponent<CanvasGroup>();
            }

            // Ẩn popup từ đầu
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }

        // Đăng ký button listeners
        if (btnQuitGame != null)
            btnQuitGame.onClick.AddListener(OnQuitGameClicked);

        if (btnSandbox != null)
            btnSandbox.onClick.AddListener(OnSandboxClicked);
    }

    private void OnDestroy()
    {
        if (btnQuitGame != null)
            btnQuitGame.onClick.RemoveListener(OnQuitGameClicked);

        if (btnSandbox != null)
            btnSandbox.onClick.RemoveListener(OnSandboxClicked);
    }

    /// <summary>
    /// Được gọi từ GoldManager sau mỗi lần AddGold.
    /// Kiểm tra xem gold đã đạt ngưỡng phá đảo chưa.
    /// </summary>
    public void CheckGameCompletion()
    {
        if (hasShownCompletion || IsSandboxMode) return;
        if (LoadDataManager.userInGame == null) return;

        int currentGold = LoadDataManager.userInGame.Gold;

        if (currentGold >= completionGoldThreshold)
        {
            Debug.Log($"[GameCompletionManager] 🎉 Người chơi đã đạt {currentGold} Gold! Phá đảo!");
            hasShownCompletion = true;
            ShowCompletionPopup();
        }
    }

    private void ShowCompletionPopup()
    {
        if (completionPanel == null || canvasGroup == null) return;

        // Dừng fade cũ nếu đang chạy
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeInDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // Tạm dừng game khi popup hiện
        Time.timeScale = 0f;
        fadeCoroutine = null;
    }

    /// <summary>
    /// Người chơi chọn "Thoát Game".
    /// </summary>
    private void OnQuitGameClicked()
    {
        Time.timeScale = 1f; // Reset lại timeScale trước khi thoát

        Debug.Log("[GameCompletionManager] Người chơi chọn Thoát Game.");

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    /// <summary>
    /// Người chơi chọn "Chơi Tiếp (Sandbox)".
    /// Gold vẫn hoạt động bình thường, chỉ không lên level nữa.
    /// </summary>
    private void OnSandboxClicked()
    {
        IsSandboxMode = true;
        Time.timeScale = 1f; // Tiếp tục game

        Debug.Log("[GameCompletionManager] Chế độ Sandbox đã bật. Gold vẫn hoạt động, không lên level nữa.");

        // Ẩn popup
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }
    }
}
