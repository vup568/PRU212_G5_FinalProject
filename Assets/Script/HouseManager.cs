using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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

    [Header("Construction UI")]
    [Tooltip("Nút Xây Dựng hiển thị khi đạt Level 2")]
    [SerializeField] private Button btnBuildHouse;

    [Tooltip("Panel chứa chữ 'Đang thi công...'")]
    [SerializeField] private GameObject constructionPanel;

    [Tooltip("Tham chiếu đến CanvasGroup chứa chữ để làm hiệu ứng nhấp nháy mờ dần")]
    [SerializeField] private CanvasGroup textBlinkCanvasGroup;

    [Header("Construction Audio")]
    [Tooltip("AudioSource dùng để phát âm thanh tiếng búa đập")]
    [SerializeField] private AudioSource audioSource;
    
    [Tooltip("File âm thanh tiếng búa đập (cộc cộc)")]
    [SerializeField] private AudioClip hammerClip;

    // Tránh xây lại nhiều lần khi có nhiều event
    private bool hasBuiltHouse2 = false; 

    private void Awake()
    {
        // Đăng ký lắng nghe event lên level
        LevelManager.OnLevelUp += OnLevelChanged;

        // Đảm bảo UI Xây dựng tắt từ đầu
        if (btnBuildHouse != null)
        {
            btnBuildHouse.gameObject.SetActive(false);
            btnBuildHouse.onClick.AddListener(OnBuildButtonClicked);
        }
        if (constructionPanel != null) 
        {
            constructionPanel.SetActive(false);
        }
    }

    private void Start()
    {
        // Khi vào scene, kiểm tra level hiện tại để set đúng trạng thái nhà
        StartCoroutine(WaitForUserDataAndApply());
    }

    private void OnDestroy()
    {
        LevelManager.OnLevelUp -= OnLevelChanged;

        if (btnBuildHouse != null)
        {
            btnBuildHouse.onClick.RemoveListener(OnBuildButtonClicked);
        }
    }

    private IEnumerator WaitForUserDataAndApply()
    {
        while (LoadDataManager.userInGame == null) yield return null;

        int currentLevel = LoadDataManager.userInGame.Level;

        if (currentLevel >= 2)
        {
            // Nếu đã Level 2 nhưng bạn muốn họ VẪN PHẢI BẤM XÂY thì hiện nút
            // Nếu muốn vào là có nhà luôn thì dùng logic cũ của bạn:
            houseLevel1.SetActive(false);
            houseLevel2.SetActive(true);
            hasBuiltHouse2 = true;
            if (btnBuildHouse != null) btnBuildHouse.gameObject.SetActive(false);
        }
        else
        {
            houseLevel1.SetActive(true);
            houseLevel2.SetActive(false);
            // Đảm bảo nút ẩn khi ở Level 1
            if (btnBuildHouse != null) btnBuildHouse.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Được gọi khi player lên level (thông qua event OnLevelUp) trong quá trình chơi.
    /// </summary>
    private void OnLevelChanged(int newLevel)
    {
        Debug.Log($"[HouseManager] Nhận event lên Level {newLevel}!");
        
        // Khi lên level 2
        if (newLevel >= 2 && !hasBuiltHouse2)
        {
            // Bật nút "Xây Dựng" chờ người dùng click
            if (btnBuildHouse != null)
            {
                btnBuildHouse.gameObject.SetActive(true);
                Debug.Log("[HouseManager] Đã hiển thị Nút Xây Dựng để tiến hành nâng cấp.");
            }
            else
            {
                // Nếu chưa gán Nút Xây dựng, tự động chạy hiệu ứng luôn (fallback)
                StartCoroutine(BuildHouseRoutine());
            }
        }
    }

    /// <summary>
    /// Sự kiện khi người chơi bấm vào nút Xây Dựng.
    /// </summary>
    private void OnBuildButtonClicked()
    {
        if (btnBuildHouse != null) 
        {
            btnBuildHouse.gameObject.SetActive(false);
        }
        
        // Bắt đầu quy trình thi công xây nhà
        StartCoroutine(BuildHouseRoutine());
    }

    /// <summary>
    /// Coroutine xử lý quá trình thi công (nhấp nháy chữ, phát âm thanh cộc cộc, delay 2 giây).
    /// </summary>
    private IEnumerator BuildHouseRoutine()
    {
        // 1. Hiện UI Panel "Đang thi công..."
        if (constructionPanel != null) 
        {
            constructionPanel.SetActive(true);
        }

        // 2. Phát âm thanh tiếng búa đập cộc cộc (nếu có)
        if (audioSource != null && hammerClip != null)
        {
            audioSource.clip = hammerClip;
            audioSource.loop = true; // Lặp lại tiếng gõ bùm bụp liên tục
            audioSource.Play();
        }

        // Thời gian thi công
        float buildDuration = 2.0f; 
        float elapsedTime = 0f;
        float blinkSpeed = 6f; // Tốc độ nhấp nháy của chữ

        // 3. Chạy vòng lặp trong 2 giây để tạo animation nhấp nháy UI
        while (elapsedTime < buildDuration)
        {
            elapsedTime += Time.deltaTime;
            
            // Dùng hàm PingPong để alpha thay đổi lên xuống tạo nhấp nháy chữ
            if (textBlinkCanvasGroup != null)
            {
                // Thay đổi độ mờ của Canvas Group (từ 0.2 -> 1.0)
                float alphaVal = Mathf.Lerp(0.2f, 1f, Mathf.PingPong(Time.time * blinkSpeed, 1f));
                textBlinkCanvasGroup.alpha = alphaVal;
            }

            yield return null;
        }

        // Trả lại alpha mặc định cho chắc chắn
        if (textBlinkCanvasGroup != null) 
        {
            textBlinkCanvasGroup.alpha = 1f;
        }

        // 4. Kết thúc thi công
        if (constructionPanel != null) 
        {
            constructionPanel.SetActive(false);
        }

        if (audioSource != null)
        {
            audioSource.Stop();
        }

        // 5. Thay đổi game object (Ẩn Level 1, Hiện Level 2)
        if (houseLevel1 != null) houseLevel1.SetActive(false);
        if (houseLevel2 != null) houseLevel2.SetActive(true);
        
        hasBuiltHouse2 = true;
        Debug.Log("[HouseManager] Thi công hoàn tất! Ngôi nhà Level 2 đã hiển thị.");
    }
}
