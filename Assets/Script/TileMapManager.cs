using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class TileMapManager : MonoBehaviour
{
    public Tilemap tm_Ground;
    public Tilemap tm_Grass;
    public Tilemap tm_Forest;

    public TileBase tb_Forest;

    private Map map;

    private FirebaseDatabaseManagement databaseManagement;
    private FirebaseUser user;

    private  DatabaseReference reference;




    // Start is called before the first frame update
    void Start()
    {

        user = FirebaseAuth.DefaultInstance.CurrentUser;

        GameObject dbObj = GameObject.Find("DatabaseManager");
        if (dbObj != null)
        {
            databaseManagement = dbObj.GetComponent<FirebaseDatabaseManagement>();
        }
        else
        {
            Debug.LogError("Không tìm thấy GameObject tên 'DatabaseManager' trong Scene!");
            return; // Dừng lại, không chạy tiếp để tránh lỗi
        }

        if (user == null)
        {
            Debug.LogError("User chưa đăng nhập (user is null). Hãy đăng nhập trước khi vào Scene này!");
            return; 
        }

        map = new Map();
       // WriteAllTileMapToFirebase();

        FirebaseApp app = FirebaseApp.DefaultInstance;
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        
        LoadMapForUser();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //firebase is non relationship database
    public void WriteAllTileMapToFirebase()
    {
        //this is default tilemap
        List<TilemapDetail> tilemaps = new List<TilemapDetail>();

        for (int x = tm_Ground.cellBounds.min.x; x < tm_Ground.cellBounds.max.x; x++)
        {
            for (int y = tm_Ground.cellBounds.min.y; y < tm_Ground.cellBounds.max.y; y++)
            {
                TilemapDetail tm_Detail = new TilemapDetail(x, y, TilemapState.Grass);
                tilemaps.Add(tm_Detail);
            }
        }

        map = new Map(tilemaps);

        Debug.Log(map.ToString());

        LoadDataManager.userInGame.MapInGame = map;

        databaseManagement.WriteDatabase("Users/" + user.UserId + "/Map", map.ToString());
    }

    public void LoadMapForUser()
    {
        reference.Child("Users").Child(user.UserId + "/Map").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.Log("load map is canceled");
                return;
            }
            else if (task.IsFaulted)
            {
                Debug.Log("load map is failed");

            }
            else if (task.IsCompleted)
            {
                //Deserialize map from json to tileMap
                DataSnapshot snapshot = task.Result;

                Debug.Log(snapshot.Value.ToString());

                map = JsonConvert.DeserializeObject<Map>(snapshot.Value.ToString());

                Debug.Log("load map: " + map.ToString());
                MapToUI(map);
            }
        });
    }

    public void TilemapDetailToTilebase(TilemapDetail tilemapdetail)
    {
        Vector3Int cellPos = new Vector3Int(tilemapdetail.x, tilemapdetail.y, 0);

        if(tilemapdetail.tilemapState == TilemapState.Ground)
        {
            tm_Grass.SetTile(cellPos, null);
            tm_Forest.SetTile(cellPos, null);

        }
        else if(tilemapdetail.tilemapState == TilemapState.Grass)
        {
            tm_Forest.SetTile(cellPos, null);
        }
        else if(tilemapdetail.tilemapState == TilemapState.Forest)
        {
            tm_Grass.SetTile(cellPos, null);
            tm_Forest.SetTile(cellPos, tb_Forest);
        }
    }


    //Change from json data to UI Map Game
    public void MapToUI(Map map)
    {
       
        for(int i = 0; i < map.GetLength(); i++)
        {
            TilemapDetailToTilebase(map.listTilemapDetail[i]);
            Debug.Log("Load map to UI successfully!");
        }
    }

    public void SetStateForTilemapDetail(int x, int y, TilemapState state)
    {
        for(int i = 0; i < map.GetLength(); i++)
        {
           
            if (map.listTilemapDetail[i].x == x && map.listTilemapDetail[i].y == y)
            {
                map.listTilemapDetail[i].tilemapState = state;

                databaseManagement.WriteDatabase("Users/" + user.UserId + "/Map", map.ToString());

                Debug.Log("Save to Firebase successful");
            }
        }
    }
}
