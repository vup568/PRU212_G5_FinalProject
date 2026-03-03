using PolyAndCode.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecyclableInventoryManager : MonoBehaviour, IRecyclableScrollRectDataSource
{
    [SerializeField]
    RecyclableScrollRect _recyclableScrollRect;

    [SerializeField]
    private int _dataLength;

    public GameObject inventoryGameObject;



    private List<InvenItems> _invenItems = new List<InvenItems>();

    private void Awake()
    {
        _recyclableScrollRect.DataSource = this;
    }

    public int GetItemCount()
    {
        return _invenItems.Count;
    }

    public void SetCell(ICell cell, int index)
    {
        // Casting to the implemented Cell
        CellItemData item = cell as CellItemData;
        item.ConfigureCell(_invenItems[index], index);
    }

    public void Start()
    {
        List<InvenItems> listItem = new List<InvenItems>();
        for (int i = 0; i < 50; i++)
        {
            InvenItems invenItem = new InvenItems();
            invenItem.name = "Name_ " + i.ToString();
            invenItem.description = "Des_ " + i.ToString();

            listItem.Add(invenItem);
        }
        SetListItem(listItem);
        _recyclableScrollRect.ReloadData();
    }

    public void SetListItem(List<InvenItems> list)
    {
        _invenItems = list;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            InvenItems invenItemDemo = new InvenItems("Ca", "Ca");
            _invenItems.Add(invenItemDemo);
            _recyclableScrollRect.ReloadData();
        }

        if(Input.GetKeyDown(KeyCode.T)) //tui
        {
            // inventoryGameObject.SetActive(!inventoryGameObject.activeSelf); //take current value of gameobject


            Vector3 currentPosInven = inventoryGameObject.GetComponent<RectTransform>().anchoredPosition;
            inventoryGameObject.GetComponent<RectTransform>().anchoredPosition = currentPosInven.y == 1000? Vector3.zero : new Vector3(0,1000,0);



        }
    }

    public void AddInventoryItem(InvenItems item)
    {
        _invenItems.Add(item);
        Debug.Log("Add Item successfully");
        _recyclableScrollRect.ReloadData();
    }
}