using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class UsernameWizard : MonoBehaviour
{
    public Text username;
    public GameObject usernameWizard;
    public InputField ipUsername;
    public Button buttonOk;

    public Text gold;
    public Text levelText;

    private FirebaseDatabaseManagement databaseManagement;

    // Start is called before the first frame update
    void Start()
    {
        databaseManagement = GameObject.Find("DatabaseManager").GetComponent<FirebaseDatabaseManagement>();
        buttonOk.onClick.AddListener(SetNewUsername);

        usernameWizard.SetActive(false);

        LoadDataManager.OnUserDataLoaded += CheckAndDisplayUI;
        LevelManager.OnLevelUp += OnLevelUp;

        if (LoadDataManager.userInGame != null)
        {
            CheckAndDisplayUI();
        }
    }

    private void OnDestroy()
    {
        LoadDataManager.OnUserDataLoaded -= CheckAndDisplayUI;
        LevelManager.OnLevelUp -= OnLevelUp;
        TutorialManager.OnTutorialCompleted -= OnTutorialCompleted;
    }

    private void CheckAndDisplayUI()
    {
        if(LoadDataManager.userInGame.Name == "")
        {
            // Người chơi mới: KHÔNG hiện wizard ngay, đợi tutorial hoàn thành
            usernameWizard.SetActive(false);
            TutorialManager.OnTutorialCompleted += OnTutorialCompleted;
            Debug.Log("[UsernameWizard] Người chơi mới, đợi tutorial hoàn thành để hiện ô nhập tên.");
        }
        else
        {
            usernameWizard.SetActive(false);
            UpdateAllUI();
        }
    }

    /// <summary>
    /// Được gọi khi tutorial hoàn thành → hiện wizard nhập tên.
    /// </summary>
    private void OnTutorialCompleted()
    {
        TutorialManager.OnTutorialCompleted -= OnTutorialCompleted;
        Debug.Log("[UsernameWizard] Tutorial hoàn thành! Hiện ô nhập tên.");
        usernameWizard.SetActive(true);
    }

    private void UpdateAllUI()
    {
        username.text = LoadDataManager.userInGame.Name;
        gold.text = "Gold: " + LoadDataManager.userInGame.Gold.ToString();

        // Hiển thị level
        if (levelText != null)
        {
            int level = LoadDataManager.userInGame.Level > 0 ? LoadDataManager.userInGame.Level : 1;
            levelText.text = "Level: " + level;
        }
    }

    /// <summary>
    /// Callback khi player lên level - cập nhật UI.
    /// </summary>
    private void OnLevelUp(int newLevel)
    {
        if (levelText != null)
        {
            levelText.text = "Level: " + newLevel;
        }
        Debug.Log($"[UsernameWizard] UI cập nhật Level: {newLevel}");
    }

    public void SetNewUsername()
    {
        if(ipUsername.text != "")
        {
            LoadDataManager.userInGame.Name = ipUsername.text;

            UpdateAllUI();




            string jsonData = JsonConvert.SerializeObject(LoadDataManager.userInGame);
            databaseManagement.WriteDatabase("Users/" + LoadDataManager.firebaseUser.UserId, jsonData);

            usernameWizard.SetActive(false);
        }
    }
}
