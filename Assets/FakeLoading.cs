using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FakeLoading : MonoBehaviour
{

    public Button buttonLoadDone;
    // Start is called before the first frame update
    void Start()
    {
        buttonLoadDone.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("PlayScene");
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
