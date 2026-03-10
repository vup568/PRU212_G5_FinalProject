using PolyAndCode.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellItemData : MonoBehaviour, ICell
{
    public Text nameLabel;
    public Text desLabel;
    public Image iconImage;
    public Button sellButton; // Nút click để bán - assign trong Inspector hoặc có thể là toàn bộ cell

    //Model
    private InvenItems _contactInfo;
    private int _cellIndex;
    private RecyclableInventoryManager _inventoryManager;

    private void Awake()
    {
        // Nếu sellButton chưa được assign, thử lấy Button từ chính GameObject này
        if (sellButton == null)
            sellButton = GetComponent<Button>();

        if (sellButton == null)
            sellButton = GetComponentInChildren<Button>();
    }

    public void ConfigureCell(InvenItems invenItems, int cellIndex, RecyclableInventoryManager manager)
    {
        _cellIndex = cellIndex;
        _contactInfo = invenItems;
        _inventoryManager = manager;

        nameLabel.text = invenItems.name;
        desLabel.text = invenItems.description;
        iconImage.sprite = invenItems.icon;

        // Gỡ listener cũ rồi gắn mới để tránh gọi nhiều lần
        if (sellButton != null)
        {
            sellButton.onClick.RemoveAllListeners();
            sellButton.onClick.AddListener(OnCellClicked);
        }
    }

    // Overload tương thích ngược nếu cần (không truyền manager)
    public void ConfigureCell(InvenItems invenItems, int cellIndex)
    {
        ConfigureCell(invenItems, cellIndex, null);
    }

    private void OnCellClicked()
    {
        Debug.Log($"[CellItemData] Click vào item: {_contactInfo?.name} (index {_cellIndex})");

        if (_inventoryManager != null)
        {
            _inventoryManager.ShowSellPanel(_contactInfo, _cellIndex);
        }
        else
        {
            Debug.LogWarning("[CellItemData] _inventoryManager là null!");
        }
    }
}