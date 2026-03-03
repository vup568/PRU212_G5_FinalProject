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

    private RecyclableInventoryManager recyclableInventoryManager;

       
    
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
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Digging");
            Vector3Int cellPos = tm_Ground.WorldToCell(transform.position); //take precise location of player with integer location
            Debug.Log("Cell position: " + cellPos);

            TileBase currentTb = tm_Grass.GetTile(cellPos);
            if(currentTb == tb_Grass)
            {
                tm_Grass.SetTile(cellPos, null);
            }
            

        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            Vector3Int cellPos = tm_Ground.WorldToCell(transform.position); //take precise location of player with integer location
            Debug.Log("Cell position: " + cellPos);

            TileBase currentTb = tm_Grass.GetTile(cellPos);
            if (currentTb == null)
            {
                tm_Forest.SetTile(cellPos, tb_Forest);
            }
        }

        if(Input.GetKeyDown(KeyCode.X))
        {
            Vector3Int cellPos = tm_Ground.WorldToCell(transform.position); //take precise location of player with integer location
            Debug.Log("Cell position: " + cellPos);

            TileBase currentTb = tm_Forest.GetTile(cellPos);

            if(currentTb != null)
            {
                tm_Grass.SetTile(cellPos, tb_Grass);

                tm_Forest.SetTile(cellPos, null);

                InvenItems itemFlower = new InvenItems();
                itemFlower.name = "Rose Flower";
                itemFlower.description = "This rose is very beautiful";

                Debug.Log(itemFlower.ToString());

                recyclableInventoryManager.AddInventoryItem(itemFlower);
            }
        }
    }
}
