using System;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// Singleton quản lý logic Level Up.
/// Khi gold đạt ngưỡng, tự động nâng level và thông báo qua event.
/// </summary>
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    /// <summary>
    /// Event được gọi khi player lên level. Tham số là level mới.
    /// </summary>
    public static event Action<int> OnLevelUp;

    [Header("Level Thresholds (Gold cần đạt để lên level)")]
    [Tooltip("Index 0 = gold cần để lên Level 2, Index 1 = gold cần để lên Level 3, ...")]
    public int[] goldThresholds = new int[] { 200 };

    private DatabaseReference reference;
    private FirebaseUser firebaseUser;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        FirebaseApp.DefaultInstance.ToString(); // ensure initialized
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        firebaseUser = FirebaseAuth.DefaultInstance.CurrentUser;
    }

    /// <summary>
    /// Kiểm tra xem gold hiện tại có đủ để lên level không.
    /// Được gọi sau mỗi lần AddGold.
    /// </summary>
    public void CheckLevelUp()
    {
        if (LoadDataManager.userInGame == null)
        {
            Debug.LogWarning("[LevelManager] userInGame is null! Không thể kiểm tra level.");
            return;
        }

        int currentGold = LoadDataManager.userInGame.Gold;
        int currentLevel = LoadDataManager.userInGame.Level;

        // Tính level mới dựa trên gold thresholds
        int newLevel = 1;
        for (int i = 0; i < goldThresholds.Length; i++)
        {
            if (currentGold >= goldThresholds[i])
            {
                newLevel = i + 2; // threshold[0] = 200 → Level 2
            }
            else
            {
                break;
            }
        }

        Debug.Log($"[LevelManager] CheckLevelUp: Gold={currentGold}, CurrentLevel={currentLevel}, NewLevel={newLevel}");

        // Nếu level tăng → cập nhật và thông báo
        if (newLevel > currentLevel)
        {
            LoadDataManager.userInGame.Level = newLevel;
            Debug.Log($"[LevelManager] 🎉 Lên Level {newLevel}! (Gold: {currentGold})");

            // Lưu lên Firebase
            SaveToFirebase();

            // Thông báo cho các listener (UI, etc.)
            OnLevelUp?.Invoke(newLevel);
        }
        else
        {
            Debug.Log($"[LevelManager] Chưa đủ điều kiện lên level. newLevel({newLevel}) <= currentLevel({currentLevel})");
        }
    }

    /// <summary>
    /// Lấy level hiện tại của player.
    /// </summary>
    public int GetCurrentLevel()
    {
        return LoadDataManager.userInGame != null ? LoadDataManager.userInGame.Level : 1;
    }

    private void SaveToFirebase()
    {
        string userId = firebaseUser != null ? firebaseUser.UserId : LoadDataManager.firebaseUser?.UserId;
        if (!string.IsNullOrEmpty(userId))
        {
            string jsonData = JsonConvert.SerializeObject(LoadDataManager.userInGame);
            reference.Child("Users").Child(userId).SetRawJsonValueAsync(jsonData)
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompleted)
                        Debug.Log("[LevelManager] Lưu Level lên Firebase thành công!");
                    else
                        Debug.LogWarning("[LevelManager] Lưu Level thất bại: " + task.Exception);
                });
        }
    }
}
