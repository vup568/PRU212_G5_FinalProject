using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class FirebaseLogin : MonoBehaviour
{
    //Register
    [Header("Register")]
    public InputField ipRegisterEmail;
    public InputField ipRegisterPassword;

    public Button buttonRegister;


    //Login
    [Header("Login")]
    public InputField ipLoginField;
    public InputField ipLoginPasswordField;

    public Button buttonLogin;

    //use for register and login
    private FirebaseAuth auth;

    //switch between form login and register
    [Header("Switch Form")]
    public Button buttonMovetoLogin;
    public Button buttonMovetoRegister;

    public GameObject loginForm;
    public GameObject registerForm;

    //Upload user's data to Firebase
    public FirebaseDatabaseManagement databaseManagement;


    // Start is called before the first frame update
    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;

        buttonRegister.onClick.AddListener(RegisterAccountWithFirebase);
        buttonLogin.onClick.AddListener(LoginWithFirebase);

        buttonMovetoRegister.onClick.AddListener(SwitchForm);
        buttonMovetoLogin.onClick.AddListener(SwitchForm);

        databaseManagement = GetComponent<FirebaseDatabaseManagement>();
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
                return;
                
            }
            if (task.IsCompleted)
            {

                Debug.Log("Register is successfully!");

                Map mapInGame = new Map();

                Users userInGame = new Users("", 100, mapInGame);

                FirebaseUser firebaseUser = task.Result.User;

                Debug.Log("Firebase user: " +  firebaseUser);

                string jsonData = JsonConvert.SerializeObject(userInGame);
                databaseManagement.WriteDatabase("Users/" + firebaseUser.UserId, jsonData);

                //LoadingManager.NEXT_SCENE = "PlayScene";
               // SceneManager.LoadScene("FakeLoading");

                

            }

        });
    }

    public void LoginWithFirebase()
    {
        string email = ipLoginField.text;
        string password = ipLoginPasswordField.text;

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.Log("Login is canceled");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.Log("Login is failed");
                return;

            }
            if (task.IsCompleted)
            {
                Debug.Log("Login is Complete!");
                FirebaseUser user = task.Result.User;

                //change scene after login successfull
                LoadingManager.NEXT_SCENE = "PlayScene";
                SceneManager.LoadScene("FakeLoading");
            }
        });
    }

    public void SwitchForm()
    {
        //set login form active with current value
        loginForm.SetActive(!loginForm.activeSelf);
        registerForm.SetActive(!registerForm.activeSelf);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
