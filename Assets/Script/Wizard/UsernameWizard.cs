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
    public Text diamond;
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
    }

    private void CheckAndDisplayUI()
    {
        if(LoadDataManager.userInGame.Name == "")
        {
            usernameWizard.SetActive(true);
        }
        else
        {
            usernameWizard.SetActive(false);
            UpdateAllUI();
        }
    }

    private void UpdateAllUI()
    {
        username.text = LoadDataManager.userInGame.Name;
        gold.text = "Gold: " + LoadDataManager.userInGame.Gold.ToString();
        diamond.text = "Diamond: " + LoadDataManager.userInGame.Diamond.ToString();

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
