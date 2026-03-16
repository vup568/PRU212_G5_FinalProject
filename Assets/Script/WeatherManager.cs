using System.Collections;
using UnityEngine;

/// <summary>
/// Singleton quản lý hệ thống thời tiết.
/// Random thời tiết mỗi 60 giây: Sunny, Rain, Storm.
/// Gắn script này vào một GameObject trong Scene (ví dụ: "WeatherManager").
/// </summary>
public class WeatherManager : MonoBehaviour
{
    public static WeatherManager Instance { get; private set; }

    public enum WeatherType
    {
        Sunny,  // Nắng - bình thường
        Rain,   // Mưa - cây mọc nhanh x1.5
        Storm   // Bão - 20% cây chết mỗi stage
    }

    [Header("Weather Settings")]
    [Tooltip("Thời gian giữa mỗi lần đổi thời tiết (giây)")]
    public float weatherInterval = 60f;

    [Tooltip("Hệ số tốc độ mọc khi Mưa (>1 = nhanh hơn)")]
    public float rainGrowthMultiplier = 1.5f;

    [Tooltip("Xác suất cây chết mỗi stage khi Bão (0.0 - 1.0)")]
    [Range(0f, 1f)]
    public float stormKillChance = 0.2f;

    /// <summary>
    /// Thời tiết hiện tại.
    /// </summary>
    public WeatherType CurrentWeather { get; private set; } = WeatherType.Sunny;

    /// <summary>
    /// Event được gọi khi thời tiết thay đổi. Tham số là thời tiết mới.
    /// </summary>
    public System.Action<WeatherType> OnWeatherChanged;

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
        // Bắt đầu với Nắng
        CurrentWeather = WeatherType.Sunny;
        OnWeatherChanged?.Invoke(CurrentWeather);
        Debug.Log("[Weather] ☀️ Bắt đầu: Nắng");

        // Bắt đầu cycle thời tiết
        StartCoroutine(WeatherCycle());
    }

    /// <summary>
    /// Coroutine thay đổi thời tiết mỗi weatherInterval giây.
    /// </summary>
    private IEnumerator WeatherCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(weatherInterval);

            // Random thời tiết mới (có thể trùng thời tiết cũ)
            WeatherType[] allWeathers = { WeatherType.Sunny, WeatherType.Rain, WeatherType.Storm };
            WeatherType newWeather = allWeathers[Random.Range(0, allWeathers.Length)];

            CurrentWeather = newWeather;
            Debug.Log($"[Weather] Thời tiết đổi thành: {GetWeatherName(newWeather)}");

            OnWeatherChanged?.Invoke(CurrentWeather);
        }
    }

    /// <summary>
    /// Lấy hệ số tốc độ mọc theo thời tiết hiện tại.
    /// Nắng = 1.0, Mưa = 1.5, Bão = 1.0
    /// </summary>
    public float GetGrowthMultiplier()
    {
        switch (CurrentWeather)
        {
            case WeatherType.Rain:
                return rainGrowthMultiplier;
            case WeatherType.Sunny:
            case WeatherType.Storm:
            default:
                return 1.0f;
        }
    }

    /// <summary>
    /// Lấy xác suất cây chết theo thời tiết hiện tại.
    /// Bão = 0.2 (20%), khác = 0
    /// </summary>
    public float GetStormKillChance()
    {
        if (CurrentWeather == WeatherType.Storm)
            return stormKillChance;
        return 0f;
    }

    /// <summary>
    /// Lấy tên thời tiết tiếng Việt.
    /// </summary>
    public string GetWeatherName(WeatherType weather)
    {
        switch (weather)
        {
            case WeatherType.Sunny: return "Nắng";
            case WeatherType.Rain:  return "Mưa";
            case WeatherType.Storm: return "Bão";
            default: return "Không xác định";
        }
    }

    /// <summary>
    /// Lấy mô tả hiệu ứng thời tiết.
    /// </summary>
    public string GetWeatherEffect(WeatherType weather)
    {
        switch (weather)
        {
            case WeatherType.Sunny: return "Bình thường";
            case WeatherType.Rain:  return "Cây mọc nhanh x1.5";
            case WeatherType.Storm: return "20% cây chết!";
            default: return "";
        }
    }

    /// <summary>
    /// Lấy icon emoji cho thời tiết.
    /// </summary>
    public string GetWeatherIcon(WeatherType weather)
    {
        switch (weather)
        {
            case WeatherType.Sunny: return "☀";
            case WeatherType.Rain:  return "🌧";
            case WeatherType.Storm: return "⛈";
            default: return "?";
        }
    }
}
