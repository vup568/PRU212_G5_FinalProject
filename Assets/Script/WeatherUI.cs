using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Hiển thị thời tiết trên UI: icon + tên + hiệu ứng.
/// HUD góc trên bên trái, đổi màu theo thời tiết.
/// Gắn script này vào một GameObject trong Scene (ví dụ: "WeatherUI").
/// </summary>
public class WeatherUI : MonoBehaviour
{
    private GameObject weatherHUD;
    private Text weatherIconText;
    private Text weatherNameText;
    private Text weatherEffectText;
    private Image hudBackground;

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
            Debug.LogError("[WeatherUI] Không tìm thấy Canvas!");
            return;
        }

        // Tự tạo WeatherManager nếu chưa có
        if (WeatherManager.Instance == null)
        {
            GameObject wmObj = new GameObject("WeatherManager");
            wmObj.AddComponent<WeatherManager>();
        }

        CreateHUD();

        // Đăng ký event
        WeatherManager.Instance.OnWeatherChanged += UpdateWeatherDisplay;

        // Hiển thị thời tiết ban đầu
        UpdateWeatherDisplay(WeatherManager.Instance.CurrentWeather);
    }

    private void OnDestroy()
    {
        if (WeatherManager.Instance != null)
            WeatherManager.Instance.OnWeatherChanged -= UpdateWeatherDisplay;
    }

    /// <summary>
    /// Tạo HUD thời tiết góc trên bên trái.
    /// </summary>
    private void CreateHUD()
    {
        // === Panel container ===
        weatherHUD = new GameObject("WeatherHUD");
        weatherHUD.transform.SetParent(gameCanvas.transform, false);

        RectTransform hudRt = weatherHUD.AddComponent<RectTransform>();
        hudRt.anchorMin = new Vector2(0, 1); // top-left
        hudRt.anchorMax = new Vector2(0, 1);
        hudRt.pivot = new Vector2(0, 1);
        hudRt.anchoredPosition = new Vector2(10, -10);
        hudRt.sizeDelta = new Vector2(240, 75);

        hudBackground = weatherHUD.AddComponent<Image>();
        hudBackground.color = new Color(0.9f, 0.75f, 0.2f, 0.8f); // Vàng mặc định (Nắng)
        hudBackground.raycastTarget = false;

        // === Icon thời tiết (lớn, bên trái) ===
        GameObject iconGo = new GameObject("WeatherIcon");
        iconGo.transform.SetParent(weatherHUD.transform, false);
        RectTransform iconRt = iconGo.AddComponent<RectTransform>();
        iconRt.anchorMin = new Vector2(0, 0);
        iconRt.anchorMax = new Vector2(0, 1);
        iconRt.pivot = new Vector2(0, 0.5f);
        iconRt.anchoredPosition = new Vector2(5, 0);
        iconRt.sizeDelta = new Vector2(55, 0);

        weatherIconText = iconGo.AddComponent<Text>();
        weatherIconText.text = "☀";
        weatherIconText.fontSize = 36;
        weatherIconText.alignment = TextAnchor.MiddleCenter;
        weatherIconText.color = Color.white;
        weatherIconText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        weatherIconText.raycastTarget = false;

        // === Tên thời tiết (dòng 1) ===
        GameObject nameGo = new GameObject("WeatherName");
        nameGo.transform.SetParent(weatherHUD.transform, false);
        RectTransform nameRt = nameGo.AddComponent<RectTransform>();
        nameRt.anchorMin = new Vector2(0, 0.5f);
        nameRt.anchorMax = new Vector2(1, 1);
        nameRt.pivot = new Vector2(0, 1);
        nameRt.anchoredPosition = new Vector2(60, -5);
        nameRt.sizeDelta = new Vector2(-70, 0);

        weatherNameText = nameGo.AddComponent<Text>();
        weatherNameText.text = "Nắng";
        weatherNameText.fontSize = 20;
        weatherNameText.fontStyle = FontStyle.Bold;
        weatherNameText.alignment = TextAnchor.MiddleLeft;
        weatherNameText.color = Color.white;
        weatherNameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        weatherNameText.raycastTarget = false;

        // === Hiệu ứng (dòng 2) ===
        GameObject effectGo = new GameObject("WeatherEffect");
        effectGo.transform.SetParent(weatherHUD.transform, false);
        RectTransform effectRt = effectGo.AddComponent<RectTransform>();
        effectRt.anchorMin = new Vector2(0, 0);
        effectRt.anchorMax = new Vector2(1, 0.5f);
        effectRt.pivot = new Vector2(0, 0);
        effectRt.anchoredPosition = new Vector2(60, 5);
        effectRt.sizeDelta = new Vector2(-70, 0);

        weatherEffectText = effectGo.AddComponent<Text>();
        weatherEffectText.text = "Bình thường";
        weatherEffectText.fontSize = 15;
        weatherEffectText.alignment = TextAnchor.MiddleLeft;
        weatherEffectText.color = new Color(1f, 1f, 1f, 0.85f);
        weatherEffectText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        weatherEffectText.raycastTarget = false;
    }

    /// <summary>
    /// Cập nhật hiển thị khi thời tiết thay đổi.
    /// </summary>
    private void UpdateWeatherDisplay(WeatherManager.WeatherType weather)
    {
        if (WeatherManager.Instance == null) return;

        // Icon
        if (weatherIconText != null)
            weatherIconText.text = WeatherManager.Instance.GetWeatherIcon(weather);

        // Tên
        if (weatherNameText != null)
            weatherNameText.text = WeatherManager.Instance.GetWeatherName(weather);

        // Hiệu ứng
        if (weatherEffectText != null)
            weatherEffectText.text = WeatherManager.Instance.GetWeatherEffect(weather);

        // Đổi màu nền
        if (hudBackground != null)
        {
            switch (weather)
            {
                case WeatherManager.WeatherType.Sunny:
                    hudBackground.color = new Color(0.9f, 0.75f, 0.2f, 0.8f); // Vàng
                    break;
                case WeatherManager.WeatherType.Rain:
                    hudBackground.color = new Color(0.2f, 0.5f, 0.85f, 0.8f); // Xanh dương
                    break;
                case WeatherManager.WeatherType.Storm:
                    hudBackground.color = new Color(0.8f, 0.15f, 0.15f, 0.8f); // Đỏ
                    break;
            }
        }

        Debug.Log($"[WeatherUI] Cập nhật UI: {WeatherManager.Instance.GetWeatherIcon(weather)} {WeatherManager.Instance.GetWeatherName(weather)} - {WeatherManager.Instance.GetWeatherEffect(weather)}");
    }
}
