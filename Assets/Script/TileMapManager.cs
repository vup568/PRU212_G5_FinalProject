using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapManager : MonoBehaviour
{
    public Tilemap tm_Ground;

    private Map map;

    private FirebaseDatabaseManagement databaseManagement;
    private FirebaseUser user;




    // Start is called before the first frame update
    void Start()
    {
        

        user = FirebaseAuth.DefaultInstance.CurrentUser;

        // Kiểm tra xem GameObject này có tồn tại trong Scene không để tránh lỗi tiếp
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

        // BƯỚC 2: Kiểm tra xem User đã đăng nhập chưa
        if (user == null)
        {
            Debug.LogError("User chưa đăng nhập (user is null). Hãy đăng nhập trước khi vào Scene này!");
            return; // Dừng lại vì không có User ID để lưu
        }

        map = new Map();
        WriteAllTileMapToFirebase();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void WriteAllTileMapToFirebase()
    {
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

        databaseManagement.WriteDatabase(user.UserId + "/Map", map.ToString());
    }
}
