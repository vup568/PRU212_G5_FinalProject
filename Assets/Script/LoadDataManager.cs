using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LoadDataManager : MonoBehaviour
{

    public static FirebaseUser firebaseUser;
    public static Users userInGame;

    public static Action OnUserDataLoaded;

    private DatabaseReference reference;

    private void Awake()
    {
        FirebaseApp app = FirebaseApp.DefaultInstance;
        reference = FirebaseDatabase.DefaultInstance.RootReference;

        firebaseUser = FirebaseAuth.DefaultInstance.CurrentUser;
        GetUserInGame();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }



    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetUserInGame()
    {
        reference.Child("Users").Child(firebaseUser.UserId).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
            {
                DataSnapshot snapshot = task.Result;

                string jsonString = snapshot.GetRawJsonValue();

                if (!string.IsNullOrEmpty(jsonString))
                {
                    userInGame = JsonConvert.DeserializeObject<Users>(jsonString);
                    Debug.Log("User in game: " + jsonString);
                }
                else
                {
                    userInGame = new Users();
                    userInGame.Name = "";
                    Debug.Log("New user, do not have any name");
                }
                OnUserDataLoaded?.Invoke();
            }
            else
            {
                Debug.Log("Load data fail: " + task.Exception);
            }
            
        });
    }
}
