using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Điều khiển popup thông báo lên level.
/// Gắn script này vào Panel popup trong Canvas.
/// Panel cần có: CanvasGroup (để fade), levelUpText, okButton (tùy chọn).
/// 
/// LƯU Ý: GameObject này phải luôn ACTIVE trong Scene.
/// Popup ẩn/hiện bằng CanvasGroup (alpha = 0), KHÔNG dùng SetActive(false).
/// </summary>
public class LevelUpPopup : MonoBehaviour
{
    public static LevelUpPopup Instance { get; private set; }

    [Header("UI References")]
    [Tooltip("Text hiển thị thông báo lên level")]
    public Text levelUpText;

    [Tooltip("Nút OK để đóng popup (tùy chọn - nếu không có sẽ tự đóng)")]
    public Button okButton;

    [Header("Settings")]
    [Tooltip("Thời gian fade in (giây)")]
    public float fadeInDuration = 0.5f;

    [Tooltip("Thời gian hiển thị trước khi tự ẩn (giây)")]
    public float displayDuration = 3f;

    [Tooltip("Thời gian fade out (giây)")]
    public float fadeOutDuration = 0.5f;

    private CanvasGroup canvasGroup;
    private Coroutine currentRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Lấy hoặc tạo CanvasGroup để điều khiển fade
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Ẩn popup khi bắt đầu (dùng CanvasGroup, KHÔNG dùng SetActive)
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        // Gắn nút OK nếu có
        if (okButton != null)
        {
            okButton.onClick.AddListener(HidePopup);
        }

        // Đăng ký event trong Awake để luôn nhận được event
        // (không dùng OnEnable vì nếu GameObject bị SetActive(false) thì OnEnable sẽ không chạy)
        LevelManager.OnLevelUp += ShowLevelUp;
        Debug.Log("[LevelUpPopup] Đã đăng ký event OnLevelUp thành công!");
    }

    private void OnDestroy()
    {
        // Hủy đăng ký khi bị destroy
        LevelManager.OnLevelUp -= ShowLevelUp;
    }

    /// <summary>
    /// Hiển thị popup thông báo lên level.
    /// </summary>
    public void ShowLevelUp(int newLevel)
    {
        Debug.Log($"[LevelUpPopup] ShowLevelUp được gọi! Level mới: {newLevel}");

        // Cập nhật text
        if (levelUpText != null)
        {
            levelUpText.text = "Chúc mừng!\nBạn đã đạt Level " + newLevel + "!";
        }

        // Dừng animation cũ nếu đang chạy
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }

        // Đảm bảo GameObject đang active để chạy Coroutine
        gameObject.SetActive(true);
        currentRoutine = StartCoroutine(PopupSequence());
    }

    /// <summary>
    /// Ẩn popup ngay lập tức (khi nhấn OK).
    /// </summary>
    public void HidePopup()
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        // KHÔNG gọi SetActive(false) để giữ event subscription
    }

    /// <summary>
    /// Chuỗi animation: Fade In → Hiển thị → Fade Out.
    /// </summary>
    private IEnumerator PopupSequence()
    {
        // === FADE IN ===
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        float elapsed = 0f;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeInDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // === HIỂN THỊ ===
        yield return new WaitForSeconds(displayDuration);

        // === FADE OUT ===
        elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = 1f - Mathf.Clamp01(elapsed / fadeOutDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        // KHÔNG gọi SetActive(false) để giữ event subscription
        currentRoutine = null;
    }
}
