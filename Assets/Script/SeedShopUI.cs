using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Tạo Shop Panel và HUD hiển thị số seed bằng code.
/// Gắn script này vào một GameObject trong Scene (ví dụ: "SeedShopUI").
/// Nhấn P để mở/đóng Shop.
/// </summary>
public class SeedShopUI : MonoBehaviour
{
    [Header("Seed Icons (gán trong Inspector)")]
    [Tooltip("Icon hạt Ngô")]
    public Sprite cornSeedIcon;
    [Tooltip("Icon hạt Hoa")]
    public Sprite flowerSeedIcon;

    // --- Shop Panel ---
    private GameObject shopPanel;
    private Text cornCountInShop;
    private Text flowerCountInShop;
    private Text cornPriceText;
    private Text flowerPriceText;
    private GameObject flowerRow; // Ẩn/hiện theo level

    // --- HUD (luôn hiển thị) ---
    private GameObject hudPanel;
    private Text hudCornText;
    private Text hudFlowerText;
    private GameObject hudFlowerGroup; // Ẩn/hiện theo level

    // --- Thông báo ---
    private GameObject notificationPanel;
    private Text notificationText;
    private float notificationTimer = 0f;

    private Canvas gameCanvas;

    private void Start()
    {
        // Tìm Canvas từ game (không phải login canvas)
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
            Debug.LogError("[SeedShopUI] Không tìm thấy Canvas!");
            return;
        }

        // Tự tạo SeedShopManager nếu chưa có
        if (SeedShopManager.Instance == null)
        {
            GameObject smObj = new GameObject("SeedShopManager");
            smObj.AddComponent<SeedShopManager>();
        }

        CreateHUD();
        CreateShopPanel();
        CreateNotificationPanel();

        // Đăng ký event
        SeedShopManager.Instance.OnSeedChanged += RefreshUI;
        RefreshUI();

        // Ẩn shop lúc đầu
        shopPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (SeedShopManager.Instance != null)
            SeedShopManager.Instance.OnSeedChanged -= RefreshUI;
    }

    private void Update()
    {
        // Toggle shop bằng phím P
        if (Input.GetKeyDown(KeyCode.P))
        {
            shopPanel.SetActive(!shopPanel.activeSelf);
            if (shopPanel.activeSelf)
                RefreshUI();
        }

        // Timer thông báo
        if (notificationPanel != null && notificationPanel.activeSelf)
        {
            notificationTimer -= Time.deltaTime;
            if (notificationTimer <= 0f)
                notificationPanel.SetActive(false);
        }

        // Cập nhật hiển thị flower theo level
        UpdateFlowerVisibility();
    }

    // =========================================
    // HUD (góc trên bên phải, luôn hiển thị)
    // =========================================
    private void CreateHUD()
    {
        // Panel container
        hudPanel = new GameObject("SeedHUD");
        hudPanel.transform.SetParent(gameCanvas.transform, false);
        RectTransform hudRt = hudPanel.AddComponent<RectTransform>();
        hudRt.anchorMin = new Vector2(1, 1); // top-right
        hudRt.anchorMax = new Vector2(1, 1);
        hudRt.pivot = new Vector2(1, 1);
        hudRt.anchoredPosition = new Vector2(-10, -50);
        hudRt.sizeDelta = new Vector2(180, 70);

        // Background
        Image hudBg = hudPanel.AddComponent<Image>();
        hudBg.color = new Color(0.1f, 0.1f, 0.15f, 0.75f);
        hudBg.raycastTarget = false;

        // --- Corn row ---
        GameObject cornGroup = new GameObject("CornSeedHUD");
        cornGroup.transform.SetParent(hudPanel.transform, false);
        RectTransform cornGrpRt = cornGroup.AddComponent<RectTransform>();
        cornGrpRt.anchorMin = new Vector2(0, 1);
        cornGrpRt.anchorMax = new Vector2(1, 1);
        cornGrpRt.pivot = new Vector2(0.5f, 1);
        cornGrpRt.anchoredPosition = new Vector2(0, -5);
        cornGrpRt.sizeDelta = new Vector2(0, 30);

        // Corn icon
        if (cornSeedIcon != null)
        {
            GameObject cornIconGo = new GameObject("CornIcon");
            cornIconGo.transform.SetParent(cornGroup.transform, false);
            RectTransform ciRt = cornIconGo.AddComponent<RectTransform>();
            ciRt.anchorMin = new Vector2(0, 0.5f);
            ciRt.anchorMax = new Vector2(0, 0.5f);
            ciRt.anchoredPosition = new Vector2(20, 0);
            ciRt.sizeDelta = new Vector2(24, 24);
            Image ciImg = cornIconGo.AddComponent<Image>();
            ciImg.sprite = cornSeedIcon;
            ciImg.raycastTarget = false;
        }

        // Corn text
        GameObject cornTxtGo = new GameObject("CornSeedText");
        cornTxtGo.transform.SetParent(cornGroup.transform, false);
        RectTransform ctRt = cornTxtGo.AddComponent<RectTransform>();
        ctRt.anchorMin = new Vector2(0, 0.5f);
        ctRt.anchorMax = new Vector2(1, 0.5f);
        ctRt.anchoredPosition = new Vector2(20, 0);
        ctRt.sizeDelta = new Vector2(-40, 28);
        hudCornText = cornTxtGo.AddComponent<Text>();
        hudCornText.text = "Ngô: x0";
        hudCornText.fontSize = 16;
        hudCornText.fontStyle = FontStyle.Bold;
        hudCornText.color = new Color(1f, 0.9f, 0.3f);
        hudCornText.alignment = TextAnchor.MiddleCenter;
        hudCornText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        hudCornText.raycastTarget = false;

        // --- Flower row ---
        hudFlowerGroup = new GameObject("FlowerSeedHUD");
        hudFlowerGroup.transform.SetParent(hudPanel.transform, false);
        RectTransform flwGrpRt = hudFlowerGroup.AddComponent<RectTransform>();
        flwGrpRt.anchorMin = new Vector2(0, 1);
        flwGrpRt.anchorMax = new Vector2(1, 1);
        flwGrpRt.pivot = new Vector2(0.5f, 1);
        flwGrpRt.anchoredPosition = new Vector2(0, -35);
        flwGrpRt.sizeDelta = new Vector2(0, 30);

        // Flower icon
        if (flowerSeedIcon != null)
        {
            GameObject flwIconGo = new GameObject("FlowerIcon");
            flwIconGo.transform.SetParent(hudFlowerGroup.transform, false);
            RectTransform fiRt = flwIconGo.AddComponent<RectTransform>();
            fiRt.anchorMin = new Vector2(0, 0.5f);
            fiRt.anchorMax = new Vector2(0, 0.5f);
            fiRt.anchoredPosition = new Vector2(20, 0);
            fiRt.sizeDelta = new Vector2(24, 24);
            Image fiImg = flwIconGo.AddComponent<Image>();
            fiImg.sprite = flowerSeedIcon;
            fiImg.raycastTarget = false;
        }

        // Flower text
        GameObject flwTxtGo = new GameObject("FlowerSeedText");
        flwTxtGo.transform.SetParent(hudFlowerGroup.transform, false);
        RectTransform ftRt = flwTxtGo.AddComponent<RectTransform>();
        ftRt.anchorMin = new Vector2(0, 0.5f);
        ftRt.anchorMax = new Vector2(1, 0.5f);
        ftRt.anchoredPosition = new Vector2(20, 0);
        ftRt.sizeDelta = new Vector2(-40, 28);
        hudFlowerText = flwTxtGo.AddComponent<Text>();
        hudFlowerText.text = "Hoa: x0";
        hudFlowerText.fontSize = 16;
        hudFlowerText.fontStyle = FontStyle.Bold;
        hudFlowerText.color = new Color(1f, 0.5f, 0.8f);
        hudFlowerText.alignment = TextAnchor.MiddleCenter;
        hudFlowerText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        hudFlowerText.raycastTarget = false;

        // Ban đầu ẩn flower nếu chưa level 2
        hudFlowerGroup.SetActive(false);
    }

    // =========================================
    // Shop Panel (mở bằng phím P)
    // =========================================
    private void CreateShopPanel()
    {
        // === Panel nền ===
        shopPanel = new GameObject("SeedShopPanel");
        shopPanel.transform.SetParent(gameCanvas.transform, false);

        RectTransform panelRt = shopPanel.AddComponent<RectTransform>();
        panelRt.anchorMin = new Vector2(0.5f, 0.5f);
        panelRt.anchorMax = new Vector2(0.5f, 0.5f);
        panelRt.pivot = new Vector2(0.5f, 0.5f);
        panelRt.anchoredPosition = Vector2.zero;
        panelRt.sizeDelta = new Vector2(420, 350);

        Image panelImg = shopPanel.AddComponent<Image>();
        panelImg.color = new Color(0.08f, 0.12f, 0.2f, 0.95f);
        panelImg.raycastTarget = true;

        Outline outline = shopPanel.AddComponent<Outline>();
        outline.effectColor = new Color(0.3f, 0.7f, 0.4f, 1f);
        outline.effectDistance = new Vector2(3, 3);

        // === Tiêu đề ===
        MakeText(shopPanel.transform, new Vector2(0, 135), new Vector2(380, 45),
            "Cửa hàng Hạt giống", 24, new Color(0.4f, 1f, 0.5f), FontStyle.Bold);

        // === Hướng dẫn ===
        MakeText(shopPanel.transform, new Vector2(0, 105), new Vector2(380, 25),
            "(Nhấn P để đóng)", 14, new Color(0.6f, 0.6f, 0.6f));

        // ========== CORN ROW ==========
        // Corn label + price
        cornPriceText = MakeText(shopPanel.transform, new Vector2(-60, 50), new Vector2(220, 35),
            "Hạt Ngô - 5 Gold", 18, Color.white);

        // Corn count
        cornCountInShop = MakeText(shopPanel.transform, new Vector2(100, 50), new Vector2(100, 35),
            "Có: 0", 16, new Color(1f, 0.9f, 0.3f), FontStyle.Bold);

        // Corn buy button
        Button btnBuyCorn = MakeButton(shopPanel.transform, new Vector2(170, 50), new Vector2(80, 35),
            new Color(0.15f, 0.55f, 0.25f), "Mua");
        btnBuyCorn.onClick.AddListener(() =>
        {
            if (SeedShopManager.Instance != null)
            {
                bool success = SeedShopManager.Instance.BuySeed("Corn");
                if (!success)
                    ShowNotification("Không đủ Gold!");
            }
        });

        // ========== FLOWER ROW ==========
        flowerRow = new GameObject("FlowerRow");
        flowerRow.transform.SetParent(shopPanel.transform, false);
        RectTransform flrRt = flowerRow.AddComponent<RectTransform>();
        flrRt.anchorMin = new Vector2(0.5f, 0.5f);
        flrRt.anchorMax = new Vector2(0.5f, 0.5f);
        flrRt.sizeDelta = new Vector2(420, 40);
        flrRt.anchoredPosition = new Vector2(0, -10);

        // Flower label + price
        flowerPriceText = MakeText(flowerRow.transform, new Vector2(-60, 0), new Vector2(220, 35),
            "Hạt Hoa - 10 Gold", 18, Color.white);

        // Flower count
        flowerCountInShop = MakeText(flowerRow.transform, new Vector2(100, 0), new Vector2(100, 35),
            "Có: 0", 16, new Color(1f, 0.5f, 0.8f), FontStyle.Bold);

        // Flower buy button
        Button btnBuyFlower = MakeButton(flowerRow.transform, new Vector2(170, 0), new Vector2(80, 35),
            new Color(0.55f, 0.15f, 0.55f), "Mua");
        btnBuyFlower.onClick.AddListener(() =>
        {
            if (SeedShopManager.Instance != null)
            {
                bool success = SeedShopManager.Instance.BuySeed("Flower");
                if (!success)
                    ShowNotification("Không đủ Gold!");
            }
        });

        // ========== CLOSE BUTTON ==========
        Button btnClose = MakeButton(shopPanel.transform, new Vector2(0, -130), new Vector2(140, 45),
            new Color(0.65f, 0.15f, 0.15f), "Đóng");
        btnClose.onClick.AddListener(() =>
        {
            shopPanel.SetActive(false);
        });

        // Đặt panel lên trên cùng
        shopPanel.transform.SetAsLastSibling();
    }

    // =========================================
    // Notification Panel (thông báo ngắn)
    // =========================================
    private void CreateNotificationPanel()
    {
        notificationPanel = new GameObject("SeedNotification");
        notificationPanel.transform.SetParent(gameCanvas.transform, false);

        RectTransform nRt = notificationPanel.AddComponent<RectTransform>();
        nRt.anchorMin = new Vector2(0.5f, 0);
        nRt.anchorMax = new Vector2(0.5f, 0);
        nRt.pivot = new Vector2(0.5f, 0);
        nRt.anchoredPosition = new Vector2(0, 80);
        nRt.sizeDelta = new Vector2(350, 45);

        Image nBg = notificationPanel.AddComponent<Image>();
        nBg.color = new Color(0.8f, 0.2f, 0.2f, 0.9f);
        nBg.raycastTarget = false;

        GameObject txtGo = new GameObject("NotifText");
        txtGo.transform.SetParent(notificationPanel.transform, false);
        RectTransform tRt = txtGo.AddComponent<RectTransform>();
        tRt.anchorMin = Vector2.zero;
        tRt.anchorMax = Vector2.one;
        tRt.sizeDelta = Vector2.zero;
        notificationText = txtGo.AddComponent<Text>();
        notificationText.text = "";
        notificationText.fontSize = 18;
        notificationText.fontStyle = FontStyle.Bold;
        notificationText.color = Color.white;
        notificationText.alignment = TextAnchor.MiddleCenter;
        notificationText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        notificationText.raycastTarget = false;

        notificationPanel.SetActive(false);
    }

    // =========================================
    // Hiển thị thông báo
    // =========================================
    public void ShowNotification(string message, float duration = 2f)
    {
        if (notificationText != null)
            notificationText.text = message;
        if (notificationPanel != null)
        {
            notificationPanel.SetActive(true);
            notificationTimer = duration;
        }
    }

    // =========================================
    // Refresh UI
    // =========================================
    private void RefreshUI()
    {
        if (SeedShopManager.Instance == null) return;

        int cornCount = SeedShopManager.Instance.GetSeedCount("Corn");
        int flowerCount = SeedShopManager.Instance.GetSeedCount("Flower");

        // HUD
        if (hudCornText != null)
            hudCornText.text = "Ngô: x" + cornCount;
        if (hudFlowerText != null)
            hudFlowerText.text = "Hoa: x" + flowerCount;

        // Shop
        if (cornCountInShop != null)
            cornCountInShop.text = "Có: " + cornCount;
        if (flowerCountInShop != null)
            flowerCountInShop.text = "Có: " + flowerCount;
    }

    private void UpdateFlowerVisibility()
    {
        int currentLevel = LevelManager.Instance != null ? LevelManager.Instance.GetCurrentLevel() : 1;
        bool showFlower = currentLevel >= 2;

        if (hudFlowerGroup != null)
            hudFlowerGroup.SetActive(showFlower);
        if (flowerRow != null)
            flowerRow.SetActive(showFlower);

        // Resize HUD panel
        if (hudPanel != null)
        {
            RectTransform rt = hudPanel.GetComponent<RectTransform>();
            rt.sizeDelta = showFlower ? new Vector2(180, 70) : new Vector2(180, 40);
        }
    }

    // =========================================
    // Helper: Tạo Text
    // =========================================
    private Text MakeText(Transform parent, Vector2 pos, Vector2 size, string content,
        int fontSize, Color color, FontStyle style = FontStyle.Normal)
    {
        GameObject go = new GameObject("Text_" + content.Substring(0, Mathf.Min(8, content.Length)));
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        Text txt = go.AddComponent<Text>();
        txt.text = content;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.fontSize = fontSize;
        txt.fontStyle = style;
        txt.color = color;
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        return txt;
    }

    // =========================================
    // Helper: Tạo Button
    // =========================================
    private Button MakeButton(Transform parent, Vector2 pos, Vector2 size, Color bgColor, string label)
    {
        GameObject btn = new GameObject("Btn_" + label);
        btn.transform.SetParent(parent, false);
        RectTransform rt = btn.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;

        // Tạo solid sprite cho background
        Texture2D tex = new Texture2D(4, 4);
        Color[] pixels = new Color[16];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = bgColor;
        tex.SetPixels(pixels);
        tex.Apply();
        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f));

        Image img = btn.AddComponent<Image>();
        img.sprite = sprite;
        img.type = Image.Type.Sliced;

        Button button = btn.AddComponent<Button>();
        button.targetGraphic = img;

        ColorBlock cb = button.colors;
        cb.normalColor = Color.white;
        cb.highlightedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
        cb.pressedColor = new Color(0.7f, 0.7f, 0.7f, 1f);
        cb.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        cb.colorMultiplier = 1f;
        button.colors = cb;

        // Label
        GameObject txtGo = new GameObject("Label");
        txtGo.transform.SetParent(btn.transform, false);
        RectTransform txtRt = txtGo.AddComponent<RectTransform>();
        txtRt.anchorMin = Vector2.zero;
        txtRt.anchorMax = Vector2.one;
        txtRt.sizeDelta = Vector2.zero;
        txtRt.anchoredPosition = Vector2.zero;

        Text txt = txtGo.AddComponent<Text>();
        txt.text = label;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.fontSize = 17;
        txt.fontStyle = FontStyle.Bold;
        txt.color = Color.white;
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.raycastTarget = false;

        return button;
    }
}
