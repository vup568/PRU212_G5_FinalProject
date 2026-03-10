using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Quản lý panel bán item từ inventory.
/// Gắn script này vào một Panel UI trong Canvas.
/// Panel cần có: itemIconImage, itemNameText, itemDescText, sellPriceText, sellButton, closeButton.
/// </summary>
public class SellItemPanel : MonoBehaviour
{
    [Header("UI References")]
    public Image itemIconImage;
    public Text itemNameText;
    public Text itemDescText;
    public Text sellPriceText;
    public Button sellButton;
    public Button closeButton;

    private InvenItems currentItem;
    private int currentItemIndex;
    private RecyclableInventoryManager inventoryManager;
    private bool listenersRegistered = false;

    private void RegisterListeners()
    {
        if (listenersRegistered) return;
        if (sellButton != null) sellButton.onClick.AddListener(OnSellClicked);
        if (closeButton != null) closeButton.onClick.AddListener(OnCloseClicked);
        listenersRegistered = true;
    }

    /// <summary>
    /// Hiển thị panel với thông tin item.
    /// </summary>
    public void ShowPanel(InvenItems item, int itemIndex, RecyclableInventoryManager manager)
    {
        RegisterListeners(); // đảm bảo listener được gắn

        currentItem = item;
        currentItemIndex = itemIndex;
        inventoryManager = manager;

        // Điền thông tin
        if (itemIconImage != null && item.icon != null)
            itemIconImage.sprite = item.icon;

        if (itemNameText != null)
            itemNameText.text = item.name;

        if (itemDescText != null)
            itemDescText.text = item.description;

        if (sellPriceText != null)
        {
            if (item.canSell)
                sellPriceText.text = "Giá bán: " + item.sellPrice + " Gold";
            else
                sellPriceText.text = "Không thể bán";
        }

        // Chỉ bật nút Bán nếu item có thể bán
        if (sellButton != null)
            sellButton.interactable = item.canSell;

        gameObject.SetActive(true);
    }

    private void OnSellClicked()
    {
        if (currentItem == null || !currentItem.canSell) return;

        // Cộng gold
        if (GoldManager.Instance != null)
        {
            GoldManager.Instance.AddGold(currentItem.sellPrice);
            Debug.Log($"[SellItemPanel] Đã bán {currentItem.name} với giá {currentItem.sellPrice} Gold");
        }
        else
        {
            Debug.LogWarning("[SellItemPanel] GoldManager.Instance is null!");
        }

        // Xóa item khỏi inventory
        if (inventoryManager != null)
        {
            inventoryManager.RemoveInventoryItem(currentItemIndex);
        }

        // Đóng panel
        gameObject.SetActive(false);
        currentItem = null;
    }

    private void OnCloseClicked()
    {
        gameObject.SetActive(false);
        currentItem = null;
    }
}
