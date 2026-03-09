using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerFarmController : MonoBehaviour
{
    public Tilemap tm_Ground;
    public Tilemap tm_Grass;
    public Tilemap tm_Forest;

    public TileBase tb_Ground;
    public TileBase tb_Grass;
    public TileBase tb_Forest;

    public List<TileBase> listTilebase_Corn;
    //public List<TileBase> listTilebase_CanHarvest;
    //can harvest la co the thu hoach

    private RecyclableInventoryManager recyclableInventoryManager;

    public TileMapManager tileMapManager;

    public GameObject usernameWizard;

    // Start is called before the first frame update
    void Start()
    {
        recyclableInventoryManager = GameObject.Find("InventoryManager").GetComponent<RecyclableInventoryManager>();

    }

    // Update is called once per frame
    void Update()
    {
        HandleFarmAction();
    }


    public void HandleFarmAction()
    {
        //Digging
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Digging");
            Vector3Int cellPos = tm_Ground.WorldToCell(transform.position); //take precise location of player with integer location
            Debug.Log("Cell position: " + cellPos);

            TileBase currentTb = tm_Grass.GetTile(cellPos);
            if (currentTb == tb_Grass)
            {
                tm_Grass.SetTile(cellPos, null);
                FindObjectOfType<AudioManager>().PlayDigSound();
                tileMapManager.SetStateForTilemapDetail(cellPos.x, cellPos.y, TilemapState.Ground);

            }

        }

        //Plant tree
        if (Input.GetKeyDown(KeyCode.V))
        {
            Vector3Int cellPos = tm_Ground.WorldToCell(transform.position); //take precise location of player with integer location
            Debug.Log("Cell position: " + cellPos);

            TileBase currentTb = tm_Grass.GetTile(cellPos);
            if (currentTb == null)
            {
                //tm_Forest.SetTile(cellPos, tb_Forest);
                StartCoroutine(GrowPlant(cellPos, tm_Forest, listTilebase_Corn));
                tileMapManager.SetStateForTilemapDetail(cellPos.x, cellPos.y, TilemapState.Corn);

            }
        }

        //take tree
        if (Input.GetKeyDown(KeyCode.X))
        {
            Vector3Int cellPos = tm_Ground.WorldToCell(transform.position); //take precise location of player with integer location
            Debug.Log("Cell position: " + cellPos);

            TileBase currentTb = tm_Forest.GetTile(cellPos);

            if (currentTb == listTilebase_Corn[4])
            {
                tm_Grass.SetTile(cellPos, null);

                tm_Forest.SetTile(cellPos, null);



                InvenItems itemCorn = new InvenItems();
                itemCorn.name = "Corn";
                itemCorn.description = "Fresh Corn";

                Debug.Log(itemCorn.ToString());

                recyclableInventoryManager.AddInventoryItem(itemCorn);
                FindObjectOfType<AudioManager>().PlayCollectSound();
                tileMapManager.SetStateForTilemapDetail(cellPos.x, cellPos.y, TilemapState.Ground);

            }
        }
    }

    IEnumerator GrowPlant(Vector3Int cellPos, Tilemap tilemap, List<TileBase> listTilebase)
    {
        int currentState = 0;
        while(currentState < listTilebase.Count)
        {
            tilemap.SetTile(cellPos, listTilebase[currentState]);
            yield return new WaitForSeconds(5);
            currentState++;
        }
    }
}
