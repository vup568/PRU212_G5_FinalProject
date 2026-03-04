using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Extensions;

public class FirebaseDatabaseManagement : MonoBehaviour
{
    private DatabaseReference reference;

    //this function run before game initialize
    private void Awake()
    {
        FirebaseApp app = FirebaseApp.DefaultInstance;
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    

    public void WriteDatabase(string id, string message)
    {
        reference.Child("Users").Child(id).SetValueAsync(message).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Data wrote succesfully!");
            }
            else
            {
                Debug.Log("Data wrote fail!");
            }
        });
    }

    public void ReadDatabase(string id)
    {
        reference.Child("Users").Child(id).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if(task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                Debug.Log("Read data successfull: " + snapshot.Value.ToString());
            }
            else
            {
                Debug.Log("Wrote data fail" + task.Exception);
            }
        });

    }
}
