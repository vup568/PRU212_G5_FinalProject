using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// Singleton quản lý Gold: cộng/trừ gold và đồng bộ lên Firebase.
/// </summary>
public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance { get; private set; }

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
    /// Cộng gold vào user, lưu Firebase, và cập nhật UI.
    /// </summary>
    public void AddGold(int amount)
    {
        if (LoadDataManager.userInGame == null) return;

        LoadDataManager.userInGame.Gold += amount;
        Debug.Log($"[GoldManager] Gold mới: {LoadDataManager.userInGame.Gold}");

        // Lưu toàn bộ user data lên Firebase
        string userId = firebaseUser != null ? firebaseUser.UserId : LoadDataManager.firebaseUser?.UserId;
        if (!string.IsNullOrEmpty(userId))
        {
            string jsonData = JsonConvert.SerializeObject(LoadDataManager.userInGame);
            reference.Child("Users").Child(userId).SetRawJsonValueAsync(jsonData)
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompleted)
                        Debug.Log("[GoldManager] Lưu Gold lên Firebase thành công!");
                    else
                        Debug.LogWarning("[GoldManager] Lưu Gold thất bại: " + task.Exception);
                });
        }

        // Cập nhật UI Gold trực tiếp
        UsernameWizard wizard = FindObjectOfType<UsernameWizard>();
        if (wizard != null && wizard.gold != null)
        {
            wizard.gold.text = "Gold: " + LoadDataManager.userInGame.Gold.ToString();
        }

        // Kiểm tra level up
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.CheckLevelUp();
        }

        // Kiểm tra phá đảo (300 Gold)
        if (GameCompletionManager.Instance != null)
        {
            GameCompletionManager.Instance.CheckGameCompletion();
        }
    }

    public int GetCurrentGold()
    {
        return LoadDataManager.userInGame != null ? LoadDataManager.userInGame.Gold : 0;
    }
}
