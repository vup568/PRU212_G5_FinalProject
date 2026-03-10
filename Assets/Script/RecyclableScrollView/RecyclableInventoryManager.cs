using PolyAndCode.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecyclableInventoryManager : MonoBehaviour, IRecyclableScrollRectDataSource
{
    [SerializeField]
    RecyclableScrollRect _recyclableScrollRect;

    [SerializeField]
    private int _dataLength;

    public GameObject inventoryGameObject;

    public Sprite demoIcon;

    [Header("Sell Feature (auto-created if null)")]
    public SellItemPanel sellItemPanel;

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
        CellItemData item = cell as CellItemData;
        item.ConfigureCell(_invenItems[index], index, this);
    }

    public void Start()
    {
        // Tự tạo GoldManager nếu chưa có
        if (GoldManager.Instance == null)
        {
            GameObject gmObj = new GameObject("GoldManager");
            gmObj.AddComponent<GoldManager>();
        }

        // Tự tạo SellPanel nếu chưa được assign trong Inspector
        if (sellItemPanel == null)
        {
            CreateSellPanel();
        }

        List<InvenItems> listItem = new List<InvenItems>();
        for (int i = 0; i < 50; i++)
        {
            InvenItems invenItem = new InvenItems();
            invenItem.name = "Name_ " + i.ToString();
            invenItem.description = "Des_ " + i.ToString();
            invenItem.icon = demoIcon;
            listItem.Add(invenItem);
        }
        SetListItem(listItem);
        _recyclableScrollRect.ReloadData();
    }

    private Sprite MakeSolidSprite(Color color)
    {
        Texture2D tex = new Texture2D(4, 4);
        Color[] pixels = new Color[16];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
        tex.SetPixels(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f));
    }

    private Button MakeButton(Transform parent, Vector2 pos, Vector2 size, Color bgColor, string label)
    {
        // Root
        GameObject btn = new GameObject("Btn_" + label);
        btn.transform.SetParent(parent, false);
        RectTransform rt = btn.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;

        // Background Image – dùng sprite solid để button hoạt động đúng
        Image img = btn.AddComponent<Image>();
        img.sprite = MakeSolidSprite(bgColor);
        img.type = Image.Type.Sliced;

        // Button component – BẮT BUỘC set targetGraphic mới nhận click
        Button button = btn.AddComponent<Button>();
        button.targetGraphic = img;

        ColorBlock cb = button.colors;
        cb.normalColor = Color.white;
        cb.highlightedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
        cb.pressedColor = new Color(0.7f, 0.7f, 0.7f, 1f);
        cb.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        cb.colorMultiplier = 1f;
        button.colors = cb;

        // Label Text
        GameObject txtGo = new GameObject("Label");
        txtGo.transform.SetParent(btn.transform, false);
        RectTransform txtRt = txtGo.AddComponent<RectTransform>();
        txtRt.anchorMin = Vector2.zero;
        txtRt.anchorMax = Vector2.one;
        txtRt.sizeDelta = Vector2.zero;
        txtRt.anchoredPosition = Vector2.zero;

        Text txt = txtGo.AddComponent<Text>();
        txt.text = label;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.fontSize = 20;
        txt.fontStyle = FontStyle.Bold;
        txt.color = Color.white;
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.raycastTarget = false; // QUAN TRỌNG: text không chặn raycast của button

        return button;
    }

    private Text MakeText(Transform parent, Vector2 pos, Vector2 size, string content, int fontSize, Color color, FontStyle style = FontStyle.Normal)
    {
        GameObject go = new GameObject("Text_" + content.Substring(0, Mathf.Min(8, content.Length)));
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        Text txt = go.AddComponent<Text>();
        txt.text = content;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.fontSize = fontSize;
        txt.fontStyle = style;
        txt.color = color;
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        return txt;
    }

    private void CreateSellPanel()
    {
        // Lấy Canvas từ inventoryGameObject (game canvas, không phải login canvas)
        Canvas canvas = inventoryGameObject != null
            ? inventoryGameObject.GetComponentInParent<Canvas>()
            : null;
        if (canvas == null)
            canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("[Inventory] Không tìm thấy Canvas!");
            return;
        }

        // ---- Background Panel ----
        GameObject panelGo = new GameObject("SellPanel");
        panelGo.transform.SetParent(canvas.transform, false);

        RectTransform panelRt = panelGo.AddComponent<RectTransform>();
        panelRt.anchorMin = new Vector2(0.5f, 0.5f);
        panelRt.anchorMax = new Vector2(0.5f, 0.5f);
        panelRt.pivot = new Vector2(0.5f, 0.5f);
        panelRt.anchoredPosition = Vector2.zero;
        panelRt.sizeDelta = new Vector2(380, 310);

        Image panelImg = panelGo.AddComponent<Image>();
        panelImg.sprite = MakeSolidSprite(new Color(0.12f, 0.12f, 0.18f, 0.97f));
        panelImg.raycastTarget = true; // block click-through

        Outline outline = panelGo.AddComponent<Outline>();
        outline.effectColor = new Color(0.35f, 0.75f, 0.35f, 1f);
        outline.effectDistance = new Vector2(3, 3);

        // ---- Icon ----
        GameObject iconGo = new GameObject("ItemIcon");
        iconGo.transform.SetParent(panelGo.transform, false);
        RectTransform iconRt = iconGo.AddComponent<RectTransform>();
        iconRt.anchorMin = new Vector2(0.5f, 0.5f);
        iconRt.anchorMax = new Vector2(0.5f, 0.5f);
        iconRt.anchoredPosition = new Vector2(0, 90);
        iconRt.sizeDelta = new Vector2(72, 72);
        Image iconImg = iconGo.AddComponent<Image>();

        // ---- Texts ----
        Text nameText  = MakeText(panelGo.transform, new Vector2(0,  27), new Vector2(340, 36), "Item Name",     22, Color.white, FontStyle.Bold);
        Text descText  = MakeText(panelGo.transform, new Vector2(0,  -8), new Vector2(340, 28), "Description",   15, new Color(0.8f,0.8f,0.8f));
        Text priceText = MakeText(panelGo.transform, new Vector2(0, -44), new Vector2(340, 32), "Giá bán: 0 Gold", 17, new Color(1f,0.85f,0.2f), FontStyle.Bold);

        // ---- Buttons ----
        Button btnSell  = MakeButton(panelGo.transform, new Vector2(-85, -108), new Vector2(145, 52), new Color(0.15f, 0.6f, 0.25f), "Bán");
        Button btnClose = MakeButton(panelGo.transform, new Vector2( 85, -108), new Vector2(145, 52), new Color(0.65f, 0.15f, 0.15f), "Đóng");

        // ---- Attach SellItemPanel script ----
        SellItemPanel panel = panelGo.AddComponent<SellItemPanel>();
        panel.itemIconImage = iconImg;
        panel.itemNameText  = nameText;
        panel.itemDescText  = descText;
        panel.sellPriceText = priceText;
        panel.sellButton    = btnSell;
        panel.closeButton   = btnClose;

        // Đặt panel lên trên cùng để không bị che
        panelGo.transform.SetAsLastSibling();

        // Ẩn lúc đầu
        panelGo.SetActive(false);

        sellItemPanel = panel;
        Debug.Log("[Inventory] SellPanel tạo tự động xong!");
    }

    public void SetListItem(List<InvenItems> list)
    {
        _invenItems = list;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            InvenItems invenItemDemo = new InvenItems("Ca", "Ca", demoIcon);
            _invenItems.Insert(0, invenItemDemo);
            _recyclableScrollRect.ReloadData();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            Vector3 currentPosInven = inventoryGameObject.GetComponent<RectTransform>().anchoredPosition;
            inventoryGameObject.GetComponent<RectTransform>().anchoredPosition =
                currentPosInven.y == 1000 ? Vector3.zero : new Vector3(0, 1000, 0);
        }
    }

    public void AddInventoryItem(InvenItems item)
    {
        _invenItems.Insert(0, item);
        Debug.Log("Add Item successfully");
        _recyclableScrollRect.ReloadData();
    }

    public void RemoveInventoryItem(int index)
    {
        if (index >= 0 && index < _invenItems.Count)
        {
            string itemName = _invenItems[index].name;
            _invenItems.RemoveAt(index);
            Debug.Log($"[Inventory] Đã xóa '{itemName}' tại index {index}");
            _recyclableScrollRect.ReloadData();
        }
    }

    public void ShowSellPanel(InvenItems item, int index)
    {
        if (sellItemPanel != null)
        {
            sellItemPanel.ShowPanel(item, index, this);
        }
        else
        {
            Debug.LogWarning("[Inventory] sellItemPanel null!");
        }
    }
}