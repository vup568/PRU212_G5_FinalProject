using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase.Extensions;

public class FirebaseLogin : MonoBehaviour
{

    public InputField ipRegisterEmail;
    
    public InputField ipRegisterPassword;

    public Button buttonRegister;

    //use for register and login
    private FirebaseAuth auth;



    // Start is called before the first frame update
    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;

        buttonRegister.onClick.AddListener(RegisterAccountWithFirebase);
    }

    public void RegisterAccountWithFirebase()
    {
        string email = ipRegisterEmail.text;
        string password = ipRegisterPassword.text;

        //take email and password of user then use with main thread of game 
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task => 
        { 
            if(task.IsCanceled)
            {
                Debug.Log("Register is canceled");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.Log("Register is failed");
                
            }
            if (task.IsCompleted)
            {
                Debug.Log("Register is Complete!");
               
            }
        
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
