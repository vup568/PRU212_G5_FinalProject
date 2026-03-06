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

    private FirebaseDatabaseManagement databaseManagement;

    // Start is called before the first frame update
    void Start()
    {
        databaseManagement = GameObject.Find("DatabaseManager").GetComponent<FirebaseDatabaseManagement>();
        buttonOk.onClick.AddListener(SetNewUsername);

        usernameWizard.SetActive(false);

        LoadDataManager.OnUserDataLoaded += CheckAndDisplayUI;

        if (LoadDataManager.userInGame != null)
        {
            CheckAndDisplayUI();
        }
    }

    private void OnDestroy()
    {
        LoadDataManager.OnUserDataLoaded -= CheckAndDisplayUI;
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
